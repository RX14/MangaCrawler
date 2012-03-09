﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading;
using System.Diagnostics;
using TomanuExtensions;
using TomanuExtensions.Utils;
using System.Collections.ObjectModel;
using Ionic.Zip;
using System.Threading.Tasks;

namespace MangaCrawlerLib
{
    public class Chapter : Entity 
    {
        private CancellationTokenSource m_cancellation_token_source;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ChapterState m_state;

        public List<Page> Pages { get; private set; }
        public bool CBZ { get; private set; }
        public string ChapterDir { get; private set; }
        public Serie Serie { get; private set; }
        public string Title { get; private set; }

        internal Chapter(Serie a_serie, string a_url, string a_title) 
        {
            Pages = new List<Page>();
            Serie = a_serie;
            URL = HttpUtility.HtmlDecode(a_url);

            a_title = a_title.Trim();
            a_title = a_title.Replace("\t", " ");
            while (a_title.IndexOf("  ") != -1)
                a_title = a_title.Replace("  ", " ");
            Title = HttpUtility.HtmlDecode(a_title);
        }

        public int PagesDownloaded
        {
            get
            {
                return Pages.Count(p => (p.State == PageState.Veryfied) ||
                                        (p.State == PageState.Downloaded));
            }
        }

        public Server Server
        {
            get
            {
                return Serie.Server;
            }
        }

        internal override Crawler Crawler
        {
            get
            {
                return Serie.Crawler;
            }
        }

        public bool IsWorking
        {
            get
            {
                return (State == ChapterState.Waiting) ||
                       (State == ChapterState.DownloadingPages) ||
                       (State == ChapterState.DownloadingPagesList) ||
                       (State == ChapterState.Deleting) ||
                       (State == ChapterState.Zipping);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Serie, Title);
        }

        public void DeleteWork()
        {
            if (State == ChapterState.Deleting)
                return;

            if (IsWorking)
            {
                Loggers.MangaCrawler.InfoFormat("Deleting, chapter: {0}, state: {1}", this, State);
                CancellationTokenSource.Cancel();
                State = ChapterState.Deleting;
            }
        }

        private string GetChapterDirectory(string a_images_base_dir)
        {
            if (a_images_base_dir.Last() == Path.DirectorySeparatorChar)
                a_images_base_dir = a_images_base_dir.RemoveFromRight(1);

            return a_images_base_dir +
                   Path.DirectorySeparatorChar +
                   FileUtils.RemoveInvalidFileDirectoryCharacters(Serie.Server.Name) +
                   Path.DirectorySeparatorChar +
                   FileUtils.RemoveInvalidFileDirectoryCharacters(Serie.Title) +
                   Path.DirectorySeparatorChar +
                   FileUtils.RemoveInvalidFileDirectoryCharacters(Title) +
                   Path.DirectorySeparatorChar;
        }

        internal void DownloadPagesList(string a_manga_root_dir, bool a_cbz)
        {
            var pages = Crawler.DownloadPages(this);

            IList<Page> added;
            IList<Page> removed;
            Sync(pages, Pages, page => (page.Name + page.URL),
                    true, out added, out removed);

            ChapterDir = GetChapterDirectory(a_manga_root_dir);
            CBZ = a_cbz;
            State = ChapterState.DownloadingPages;

            bool changed = added.Any() || removed.Any();

            foreach (var page in Pages)
            {
                if (changed)
                    page.State = PageState.WaitingForDownloading;
                else
                    page.State = PageState.WaitingForVerifing;
            }
        }

        internal void DownloadPages(string a_manga_root_dir, bool a_cbz)
        {
            bool error = false;

            try
            {
                Limiter.BeginChapter(this);

                DownloadPagesList(a_manga_root_dir, a_cbz);

                Parallel.ForEach(Pages,

                    new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = Crawler.MaxConnectionsPerServer, 
                        TaskScheduler = Limiter.Scheduler
                    },
                    (page, state) =>
                    {
                        try
                        {
                            if (!page.DownloadRequired())
                                return;
 
                            page.DownloadAndSavePageImage();
                        }
                        catch (OperationCanceledException ex1)
                        {
                            Loggers.Cancellation.InfoFormat(
                                "OperationCanceledException #2, chapter: {0} state: {1}, {2}",
                                this, State, ex1);

                            state.Break();
                        }
                        catch (Exception ex2)
                        {
                            Loggers.MangaCrawler.InfoFormat(
                                "Exception #1, chapter: {0} state: {1}, {2}",
                                this, State, ex2);

                            state.Break();
                            error = true;
                        }
                    });

                if (CBZ)
                    CreateCBZ();
            }
            catch (OperationCanceledException ex1)
            {
                Loggers.Cancellation.InfoFormat(
                    "OperationCanceledException #2, chapter: {0} state: {1}, {2}",
                    this, State, ex1);
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.InfoFormat(
                    "Exception #2, chapter: {0} state: {1}, {2}",
                    this, State, ex);
                error = true;
            }
            finally
            {
                Loggers.MangaCrawler.InfoFormat("Chapter: {0}, state: {1}, error: {2}",
                    this, State, error);

                if (error)
                    State = ChapterState.Error;
                else if (CancellationTokenSource.IsCancellationRequested)
                    State = ChapterState.Aborted;
                else if (PagesDownloaded != Pages.Count)
                    State = ChapterState.Error;
                else if (Pages.Any(p => (p.State != PageState.Downloaded) && (p.State != PageState.Veryfied)))
                    State = ChapterState.Error;
                else
                    State = ChapterState.Downloaded;

                Limiter.EndChapter(this);
            }
        }

        private void CreateCBZ()
        {
            Loggers.MangaCrawler.InfoFormat(
                "Chapter: {0} state: {1}",
                this, State);

            if (Pages.Count == 0)
            {
                Loggers.MangaCrawler.InfoFormat("Pages.Count = 0 - nothing to zip");
                return;
            }

            State = ChapterState.Zipping;

            var dir = new DirectoryInfo(Pages.First().ImageFilePath).Parent;

            var zip_file = dir.FullName + ".cbz";

            int counter = 1;
            while (new FileInfo(zip_file).Exists)
            {
                zip_file = String.Format("{0} ({1}).cbz", dir.FullName, counter);
                counter++;
            }

            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.UseUnicodeAsNecessary = true;

                    foreach (var page in Pages)
                    {
                        zip.AddFile(page.ImageFilePath, "");

                        if (Token.IsCancellationRequested)
                        {
                            Loggers.Cancellation.InfoFormat(
                                "cancellation requested, chapter: {0} state: {1}",
                                this, State);

                            Token.ThrowIfCancellationRequested();
                        }
                    }

                    zip.Save(zip_file);
                }
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Fatal("Exception #1", ex);
            }

            try
            {
                foreach (var page in Pages)
                    new FileInfo(page.ImageFilePath).Delete();

                if ((dir.GetFiles().Count() == 0) && (dir.GetDirectories().Count() == 0))
                    dir.Delete();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Fatal("Exception #2", ex);
            }
        }

        private CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (m_cancellation_token_source == null)
                    m_cancellation_token_source = new CancellationTokenSource();
                return m_cancellation_token_source;
            }
        }

        internal CancellationToken Token
        {
            get
            {
                return CancellationTokenSource.Token;
            }
        }

        public ChapterState State
        {
            get
            {
                return m_state;
            }
            internal set
            {
                switch (value)
                {
                    case ChapterState.Initial:
                    {
                        break;
                    }
                    case ChapterState.Waiting:
                    {
                        Debug.Assert((State == ChapterState.Aborted) ||
                                     (State == ChapterState.Downloaded) ||
                                     (State == ChapterState.Error) ||
                                     (State == ChapterState.Initial));
                        break;
                    }
                    case ChapterState.DownloadingPagesList:
                    {
                        Debug.Assert(State == ChapterState.Waiting);
                        break;
                    }
                    case ChapterState.DownloadingPages:
                    {
                        Debug.Assert(State == ChapterState.DownloadingPagesList);
                        break;
                    }
                    case ChapterState.Zipping:
                    {
                        Debug.Assert(State == ChapterState.DownloadingPages);
                        break;
                    }
                    case ChapterState.Aborted:
                    {
                        Debug.Assert(State == ChapterState.Deleting);
                        break;
                    }
                    case ChapterState.Deleting:
                    {
                        Debug.Assert((State == ChapterState.DownloadingPages) ||
                                     (State == ChapterState.DownloadingPagesList) ||
                                     (State == ChapterState.Waiting) ||
                                     (State == ChapterState.Zipping));
                        break;
                    }
                    case ChapterState.Error:
                    {
                        Debug.Assert((State == ChapterState.DownloadingPages) ||
                                     (State == ChapterState.DownloadingPagesList) ||
                                     (State == ChapterState.Waiting) ||
                                     (State == ChapterState.Error) ||
                                     (State == ChapterState.Zipping));
                        break;
                    }
                    case ChapterState.Downloaded:
                    {
                        Debug.Assert((State == ChapterState.DownloadingPages) ||
                                     (State == ChapterState.Zipping));
                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException(String.Format("Unknown state: {0}", value));
                    }
                }

                m_state = value;
            }
        }
    }
}
