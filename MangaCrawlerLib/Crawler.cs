﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using HtmlAgilityPack;

namespace MangaCrawlerLib
{
    public abstract class Crawler
    {
        public abstract string Name { get; }

        internal abstract void DownloadSeries(Server a_server, Action<int, IEnumerable<Serie>> a_progress_callback);
        internal abstract void DownloadChapters(Serie a_serie, Action<int, IEnumerable<Chapter>> a_progress_callback);
        internal abstract IEnumerable<Page> DownloadPages(Chapter a_chapter);
        public abstract string GetServerURL();
        internal abstract string GetImageURL(Page a_page);

        internal static T DownloadWithRetry<T>(Func<T> a_func)
        {
            WebException ex1 = null;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return a_func();
                }
                catch (WebException ex)
                {
                    Loggers.MangaCrawler.Info("exception, {0}", ex);

                    ex1 = ex;
                    continue;
                }
            }

            throw ex1;
        }

        internal HtmlDocument DownloadDocument(Server a_server, string a_url = null)
        {
            return DownloadDocument(
                (a_url == null) ? a_server.URL : a_url,
                () => a_server.State = ServerState.Downloading,
                () => Limiter.Aquire(a_server),
                () => Limiter.Release(a_server));
        }

        internal HtmlDocument DownloadDocument(Serie a_serie, string a_url = null)
        {
            return DownloadDocument(
                (a_url == null) ? a_serie.URL : a_url, 
                () => a_serie.State  = SerieState.Downloading, 
                () => Limiter.Aquire(a_serie),
                () => Limiter.Release(a_serie));
        }

        internal HtmlDocument DownloadDocument(Chapter a_chapter, string a_url = null)
        {
            return DownloadDocument(
                (a_url == null) ? a_chapter.URL : a_url, 
                () => a_chapter.State = ChapterState.DownloadingPagesList, 
                () => Limiter.Aquire(a_chapter),
                () => Limiter.Release(a_chapter));
        }

        internal HtmlDocument DownloadDocument(Page a_page, string a_url = null)
        {
            return DownloadDocument(
                (a_url == null) ? a_page.URL : a_url, 
                () => a_page.State = PageState.Downloading, 
                () => Limiter.Aquire(a_page),
                () => Limiter.Release(a_page));
        }

        internal virtual HtmlDocument DownloadDocument(string a_url, Action a_started, 
            Action a_aquire, Action a_release)
        {
            return DownloadWithRetry(() =>
            {
                a_aquire();

                if (a_started != null)
                    a_started();

                try
                {
                    var web = new HtmlWeb();
                    var page = web.Load(a_url);

                    if (web.StatusCode == HttpStatusCode.NotFound)
                    {
                        Loggers.MangaCrawler.InfoFormat(
                            "ConnectionsLimiter.DownloadDocument - series - page was not found, url: {0}",
                            a_url);

                        return null;
                    }

                    return page;
                }
                finally
                {
                    a_release();
                }
            });
        }

        internal virtual MemoryStream GetImageStream(Page a_page)
        {
            return DownloadWithRetry(() =>
            {
                try
                {
                    Limiter.Aquire(a_page);

                    HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(
                        a_page.ImageURL);

                    myReq.UserAgent = DownloadManager.Instance.MangaSettings.UserAgent;
                    myReq.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    myReq.Referer = a_page.URL;

                    byte[] buffer = new byte[4*1024];

                    using (Stream image_stream = myReq.GetResponse().GetResponseStream())
                    {
                        MemoryStream mem_stream = new MemoryStream();

                        for (;;)
                        {
                            int readed = image_stream.Read(buffer, 0, buffer.Length);

                            if (readed == 0)
                                break;

                            if (a_page.Chapter.Token.IsCancellationRequested)
                            {
                                Loggers.Cancellation.InfoFormat(
                                    "cancellation requested, work: {0} state: {1}",
                                    this, a_page.Chapter.State);

                                a_page.Chapter.Token.ThrowIfCancellationRequested();
                            }

                            mem_stream.Write(buffer, 0, readed);
                        }

                        mem_stream.Position = 0;
                        return mem_stream;
                    }
                }
                finally
                {
                    Limiter.Release(a_page);
                }
            });
        }

        public virtual int MaxConnectionsPerServer
        {
            get
            {
                return DownloadManager.Instance.MangaSettings.MaximumConnectionsPerServer;
            }
        }
    }
}
