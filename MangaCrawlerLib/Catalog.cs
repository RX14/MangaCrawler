﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MangaCrawlerLib.Crawlers;
using System.Xml.Linq;
using TomanuExtensions;
using System.IO;
using TomanuExtensions.Utils;
using System.Xml;
using System.Threading;
using System.Diagnostics;

namespace MangaCrawlerLib
{
    public static class Catalog
    {
        private static string CATALOG_XML = "catalog.xml";
        private static string WORKS_XML = "works.xml";

        #region XML Nodes

        private static string CATALOG_NODE = "Catalog";

        private static string GLOBAL_ID_COUNTER_NODE = "IDCounter";

        private static string SERVERS_NODE = "Servers";
        private static string SERVERS_SERVER_NODE = "Server";
        private static string SERVERS_SERVER_ID_NODE = "ID";
        private static string SERVERS_SERVER_NAME_NODE = "Name";
        private static string SERVERS_SERVER_STATE_NODE = "State";
        private static string SERVERS_SERVER_URL_NODE = "URL";

        private static string SERIES_NODE = "ServerSeries";
        private static string SERIES_SERVER_ID_NODE = "ID";
        private static string SERIES_SERVER_NAME_NODE = "ServerName";
        private static string SERIES_SERVER_URL_NODE = "URL";
        private static string SERIES_SERIES_NODE = "Series";
        private static string SERIES_SERIE_ID_NODE = "ID";
        private static string SERIES_SERIE_NODE = "Serie";
        private static string SERIES_SERIE_TITLE_NODE = "Title";
        private static string SERIES_SERIE_STATE_NODE = "State";
        private static string SERIES_SERIE_URL_NODE = "URL";

        private static string CHAPTERS_NODE = "SerieChapters";
        private static string CHAPTERS_SERIE_TITLE_NODE = "Title";
        private static string CHAPTERS_SERIE_SERVER_ID_NODE = "ServerID";
        private static string CHAPTERS_SERIE_URL_NODE = "URL";
        private static string CHAPTERS_SERIE_ID_NODE = "ID";
        private static string CHAPTERS_CHAPTERS_NODE = "Chapters";
        private static string CHAPTERS_CHAPTER_NODE = "Chapter";
        private static string CHAPTERS_CHAPTER_ID_NODE = "ID";
        private static string CHAPTERS_CHAPTER_STATE_NODE = "State";
        private static string CHAPTERS_CHAPTER_TITLE_NODE = "Title";
        private static string CHAPTERS_CHAPTER_LIMITER_ORDER_NODE = "LimiterOrder";
        private static string CHAPTERS_CHAPTER_URL_NODE = "URL";

        private static string PAGES_NODE = "ChapterPages";
        private static string PAGES_CHAPTER_TITLE_NODE = "Title";
        private static string PAGES_CHAPTER_URL_NODE = "URL";
        private static string PAGES_CHAPTER_SERIE_ID_NODE = "SerieID";
        private static string PAGES_CHAPTER_ID_NODE = "ID";
        private static string PAGES_PAGES_NODE = "Pages";
        private static string PAGES_PAGE_NODE = "Page";
        private static string PAGES_PAGE_ID_NODE = "ID";
        private static string PAGES_PAGE_INDEX_NODE = "Index";
        private static string PAGES_PAGE_NAME_NODE = "Name";
        private static string PAGES_PAGE_URL_NODE = "URL";
        private static string PAGES_PAGE_HASH_NODE = "Hash";
        private static string PAGES_PAGE_STATE_NODE = "State";
        private static string PAGES_PAGE_IMAGEFILEPATH_NODE = "ImageFilePath";

        private static string WORKS_NODE = "Works";
        private static string WORKS_WORK_NODE = "Work";
        private static string WORKS_CHAPTER_ID_NODE = "ChapterID";
        private static string WORKS_SERIE_ID_NODE = "SerieID";
        private static string WORKS_SERVER_ID_NODE = "ServerID";

        #endregion

        public const double COMPACT_RATIO = 0.75;
        private static Object m_lock = new Object();

        #if TEST_SERVERS
        private static string CATALOG_DIR = "Catalog_Test\\";
        #else
        private static string CATALOG_DIR = "Catalog\\";
        #endif

        private static ulong IDCounter = 0;

        internal static ulong NextID()
        {
            lock (m_lock)
            {
                IDCounter++;
                return IDCounter;
            }
        }

        private static string CatalogFile
        {
            get
            {
                return CatalogDir + CATALOG_XML;
            }
        }

        private static string WorksFile
        {
            get
            {
                return CatalogDir + WORKS_XML;
            }
        }

        private static string CatalogDir
        {
            get
            {
                return DownloadManager.Instance.MangaSettings.GetMangaRootDir(true) + 
                    Path.DirectorySeparatorChar + CATALOG_DIR;
            }
        }

        private static IEnumerable<Server> GetServers()
        {
            return from c in CrawlerList.Crawlers
                   select new Server(c.GetServerURL(), c.Name);
        }

        internal static Server[] LoadCatalog()
        {
            var doc = LoadXml(CatalogFile);

            if (doc == null)
            {
                ClearCatalog();
                IDCounter = 0;
                return GetServers().ToArray();
            }

            Debug.Assert(IDCounter == 0);
            List<Server> servers = GetServers().ToList();

            try
            {
                var root = doc.Element(CATALOG_NODE);

                IDCounter = UInt64.Parse(root.Element(GLOBAL_ID_COUNTER_NODE).Value);

                var catalog_servers = (from server in root.Element(SERVERS_NODE).Elements(SERVERS_SERVER_NODE)
                                       select new Server(
                                           server.Element(SERVERS_SERVER_URL_NODE).Value, 
                                           server.Element(SERVERS_SERVER_NAME_NODE).Value,
                                           UInt64.Parse(server.Element(SERVERS_SERVER_ID_NODE).Value), 
                                           EnumExtensions.Parse<ServerState>(
                                            server.Element(SERVERS_SERVER_STATE_NODE).Value)
                                       )).ToList();

                if (!catalog_servers.Select(s => s.ID).Unique())
                    throw new XmlException();

                

                catalog_servers = (from server in catalog_servers
                                   where servers.Select(s => s.ID).Contains(server.ID)
                                   select server).ToList();

                for (int i=0; i<servers.Count; i++)
                {
                    Server cs = catalog_servers.FirstOrDefault(s => s.ID == servers[i].ID);

                    if (cs == null)
                        continue;

                    servers[i] = new Server(servers[i].URL, servers[i].Name, servers[i].ID, cs.State);
                }

                return servers.ToArray();
            }
            catch (Exception ex1)
            {
                Loggers.MangaCrawler.Error("Exception #1", ex1);
                ClearCatalog();
                IDCounter = 0;
                return GetServers().ToArray();
            }
        }

        private static void ClearCatalog()
        {
            try
            {
                new DirectoryInfo(CatalogDir).DeleteContent();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }

        internal static void SaveCatalog()
        {
            lock (m_lock)
            {
                try
                {
                    new DirectoryInfo(CatalogDir).Create();

                    var xml =
                        new XElement(CATALOG_NODE,
                            new XElement(GLOBAL_ID_COUNTER_NODE, IDCounter),
                            new XElement(SERVERS_NODE, from s in DownloadManager.Instance.Servers
                                                       select new XElement(SERVERS_SERVER_NODE,
                                                           new XElement(SERVERS_SERVER_ID_NODE, s.ID),
                                                           new XElement(SERVERS_SERVER_NAME_NODE, s.Name),
                                                           new XElement(SERVERS_SERVER_STATE_NODE, s.State),
                                                           new XElement(SERVERS_SERVER_URL_NODE, s.URL))));
                    xml.Save(CatalogFile);
                }
                catch (Exception ex)
                {
                    Loggers.MangaCrawler.Error("Exception", ex);
                }
            }
        }

        private static string GetCatalogFile(ulong a_id)
        {
            return CatalogDir + a_id.ToString() + ".xml";
        }

        private static void DeleteCatalogFile(ulong a_id)
        {
            DeleteFile(GetCatalogFile(a_id));
        }

        private static void DeleteFile(string a_path)
        {
            try
            {
                new FileInfo(a_path).Delete();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }

        private static XDocument LoadCatalogXml(ulong a_id)
        {
            if (!new FileInfo(GetCatalogFile(a_id)).Exists)
                return null;

            try
            {
                return XDocument.Load(GetCatalogFile(a_id));
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Fatal("Exception", ex);

                DeleteCatalogFile(a_id);
                return null;
            }
        }

        private static XDocument LoadXml(string a_path)
        {
            if (!new FileInfo(a_path).Exists)
                return null;

            try
            {
                return XDocument.Load(a_path);
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Fatal("Exception", ex);

                DeleteFile(a_path);
                return null;
            }
        }

        internal static List<Serie> LoadServerSeries(Server a_server)
        {
            XDocument xml = LoadCatalogXml(a_server.ID);

            if (xml == null)
                return new List<Serie>();

            try
            {
                XElement root = xml.Element(SERIES_NODE);

                string server_name = root.Element(SERIES_SERVER_NAME_NODE).Value;
                string server_url = root.Element(SERIES_SERVER_URL_NODE).Value;
                ulong server_id = UInt64.Parse(root.Element(SERIES_SERVER_ID_NODE).Value);

                if ((server_name != a_server.Name) ||
                    (server_url != a_server.URL) ||
                    (server_id != a_server.ID))
                {
                    DeleteCatalogFile(a_server.ID);
                    return new List<Serie>();
                }

                var series = from serie in root.Element(SERIES_SERIES_NODE).Elements(SERIES_SERIE_NODE)
                             select new
                             {
                                 ID =  UInt64.Parse(serie.Element(SERIES_SERIE_ID_NODE).Value),
                                 Title = serie.Element(SERIES_SERIE_TITLE_NODE).Value,
                                 URL = serie.Element(SERIES_SERIE_URL_NODE).Value,
                                 State = EnumExtensions.Parse<SerieState>(
                                    serie.Element(SERIES_SERIE_STATE_NODE).Value)
                             };

                return (from serie in series
                        select new Serie(a_server, serie.URL, serie.Title, serie.ID, serie.State)).ToList();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);

                DeleteCatalogFile(a_server.ID);
                return new List<Serie>();
            }
        }

        private static void SaveServerSeries(Server a_server)
        {
            try
            {
                lock (m_lock)
                {
                    new DirectoryInfo(CatalogDir).Create();

                    var xml = new XElement(SERIES_NODE,
                        new XElement(SERIES_SERVER_ID_NODE, a_server.ID),
                        new XElement(SERIES_SERVER_NAME_NODE, a_server.Name),
                        new XElement(SERIES_SERVER_URL_NODE, a_server.URL),
                        new XElement(SERIES_SERIES_NODE,
                            from s in a_server.Series
                            select new XElement(SERIES_SERIE_NODE,
                                new XElement(SERIES_SERIE_ID_NODE, s.ID),
                                new XElement(SERIES_SERIE_TITLE_NODE, s.Title),
                                new XElement(SERIES_SERIE_STATE_NODE, s.State),
                                new XElement(SERIES_SERIE_URL_NODE, s.URL))));

                    xml.Save(GetCatalogFile(a_server.ID));
                }
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }

        internal static List<Chapter> LoadSerieChapters(Serie a_serie)
        {
            XDocument xml = LoadCatalogXml(a_serie.ID);

            if (xml == null)
                return new List<Chapter>();

            try
            {
                XElement root = xml.Element(CHAPTERS_NODE);

                string serie_title = root.Element(CHAPTERS_SERIE_TITLE_NODE).Value;
                string serie_url = root.Element(CHAPTERS_SERIE_URL_NODE).Value;
                ulong server_id = UInt64.Parse(root.Element(CHAPTERS_SERIE_SERVER_ID_NODE).Value);
                ulong serie_id = UInt64.Parse(root.Element(CHAPTERS_SERIE_ID_NODE).Value);

                if ((serie_title != a_serie.Title) ||
                    (serie_url != a_serie.URL) ||
                    (server_id != a_serie.Server.ID) ||
                    (serie_id != a_serie.ID))
                {
                    DeleteCatalogFile(a_serie.ID);
                    return new List<Chapter>();
                }

                var chapters = from chapter in root.Element(CHAPTERS_CHAPTERS_NODE).Elements(CHAPTERS_CHAPTER_NODE)
                               select new
                               {
                                   ID = UInt64.Parse(chapter.Element(CHAPTERS_CHAPTER_ID_NODE).Value),
                                   Title = chapter.Element(CHAPTERS_CHAPTER_TITLE_NODE).Value,
                                   LimiterOrder = UInt64.Parse(chapter.Element(CHAPTERS_CHAPTER_LIMITER_ORDER_NODE).Value), 
                                   URL = chapter.Element(CHAPTERS_CHAPTER_URL_NODE).Value,
                                   State = EnumExtensions.Parse<ChapterState>(
                                       chapter.Element(CHAPTERS_CHAPTER_STATE_NODE).Value)
                               };

                return (from chapter in chapters
                        select new Chapter(a_serie, chapter.URL, chapter.Title, 
                            chapter.ID, chapter.State, chapter.LimiterOrder)).ToList();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);

                DeleteCatalogFile(a_serie.ID);
                return new List<Chapter>();
            }
        }

        private static void SaveSerieChapters(Serie a_serie)
        {
            try
            {
                lock (m_lock)
                {
                    new DirectoryInfo(CatalogDir).Create();

                    var xml = new XElement(CHAPTERS_NODE,
                        new XElement(CHAPTERS_SERIE_ID_NODE, a_serie.ID),
                        new XElement(CHAPTERS_SERIE_TITLE_NODE, a_serie.Title),
                        new XElement(CHAPTERS_SERIE_SERVER_ID_NODE, a_serie.Server.ID),
                        new XElement(CHAPTERS_SERIE_URL_NODE, a_serie.URL),
                        new XElement(CHAPTERS_CHAPTERS_NODE,
                            from c in a_serie.Chapters
                            select new XElement(CHAPTERS_CHAPTER_NODE,
                                new XElement(CHAPTERS_CHAPTER_ID_NODE, c.ID),
                                new XElement(CHAPTERS_CHAPTER_TITLE_NODE, c.Title),
                                new XElement(CHAPTERS_CHAPTER_LIMITER_ORDER_NODE, c.LimiterOrder),
                                new XElement(CHAPTERS_CHAPTER_STATE_NODE, c.State),
                                new XElement(CHAPTERS_CHAPTER_URL_NODE, c.URL))));

                    xml.Save(GetCatalogFile(a_serie.ID));
                }
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }

        internal static List<Page> LoadChapterPages(Chapter a_chapter)
        {
            XDocument xml = LoadCatalogXml(a_chapter.ID);

            if (xml == null)
                return new List<Page>();

            try
            {
                XElement root = xml.Element(PAGES_NODE);

                string chapter_title = root.Element(PAGES_CHAPTER_TITLE_NODE).Value;
                string chapter_url = root.Element(PAGES_CHAPTER_URL_NODE).Value;
                ulong chapter_id = UInt64.Parse(root.Element(PAGES_CHAPTER_ID_NODE).Value);
                ulong serie_id = UInt64.Parse(root.Element(PAGES_CHAPTER_SERIE_ID_NODE).Value);

                if ((chapter_title != a_chapter.Title) ||
                    (chapter_url != a_chapter.URL) ||
                    (serie_id != a_chapter.Serie.ID) ||
                    (chapter_id != a_chapter.ID))
                {
                    DeleteCatalogFile(a_chapter.ID);
                    return new List<Page>();
                }

                var pages = from page in root.Element(PAGES_PAGES_NODE).Elements(PAGES_PAGE_NODE)
                            select new
                            {
                                ID = UInt64.Parse(page.Element(PAGES_PAGE_ID_NODE).Value),
                                Name = page.Element(PAGES_PAGE_NAME_NODE).Value,
                                Index = page.Element(PAGES_PAGE_INDEX_NODE).Value.ToInt(),
                                URL = page.Element(PAGES_PAGE_URL_NODE).Value,
                                Hash = ConvertHexStringToBytes(page.Element(PAGES_PAGE_HASH_NODE).Value),
                                ImageFilePath = page.Element(PAGES_PAGE_IMAGEFILEPATH_NODE).Value,
                                State = EnumExtensions.Parse<PageState>(
                                    page.Element(PAGES_PAGE_STATE_NODE).Value)
                            };

                return (from page in pages
                        select new Page(a_chapter, page.URL, page.Index, page.ID, page.Name, 
                            page.Hash, page.ImageFilePath, page.State)).ToList();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);

                DeleteCatalogFile(a_chapter.ID);
                return new List<Page>();
            }
        }

        private static byte[] ConvertHexStringToBytes(string a_hash)
        {
            if (a_hash.Length == 0)
                return null;

            return Converters.ConvertHexStringToBytes(a_hash);
        }

        private static void SaveChapterPages(Chapter a_chapter)
        {
            try
            {
                lock (m_lock)
                {
                    new DirectoryInfo(CatalogDir).Create();

                    var xml = new XElement(PAGES_NODE,
                        new XElement(PAGES_CHAPTER_ID_NODE, a_chapter.ID),
                        new XElement(PAGES_CHAPTER_TITLE_NODE, a_chapter.Title),
                        new XElement(PAGES_CHAPTER_SERIE_ID_NODE, a_chapter.Serie.ID),
                        new XElement(PAGES_CHAPTER_URL_NODE, a_chapter.URL),
                        new XElement(PAGES_PAGES_NODE,
                            from p in a_chapter.Pages
                            select new XElement(PAGES_PAGE_NODE,
                                new XElement(PAGES_PAGE_ID_NODE, p.ID),
                                new XElement(PAGES_PAGE_NAME_NODE, p.Name),
                                new XElement(PAGES_PAGE_INDEX_NODE, p.Index),
                                new XElement(PAGES_PAGE_HASH_NODE, 
                                    (p.Hash != null) ? Converters.ConvertBytesToHexString(p.Hash, true) : ""),
                                new XElement(PAGES_PAGE_IMAGEFILEPATH_NODE, p.ImageFilePath),
                                new XElement(PAGES_PAGE_STATE_NODE, p.State),
                                new XElement(PAGES_PAGE_URL_NODE, p.URL))));

                    xml.Save(GetCatalogFile(a_chapter.ID));
                }
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }

        internal static void Save(Serie a_serie)
        {
            SaveCatalog();
            SaveSerieChapters(a_serie);
            SaveServerSeries(a_serie.Server);

        }

        internal static void Save(Server a_server)
        {
            SaveCatalog();
            SaveServerSeries(a_server);

        }

        internal static void Save(Chapter a_chapter)
        {
            SaveCatalog();
            SaveChapterPages(a_chapter);
            SaveSerieChapters(a_chapter.Serie);

        }

        internal static void Save(Page a_page)
        {
            SaveCatalog();
            SaveChapterPages(a_page.Chapter);

        }

        private static IEnumerable<FileInfo> GetCatalogFiles()
        {
            if (!new FileInfo(CatalogDir).Exists)
                return new FileInfo[] { };

            return new DirectoryInfo(CatalogDir).EnumerateFiles("*.xml");
        }

        public static long GetCatalogSize()
        {
            return GetCatalogFiles().Sum(f => f.Length);
        }

        private static bool IsCompacted(long a_max_catalog_size)
        {
            long desire_catalog_size = (long)(a_max_catalog_size * COMPACT_RATIO);
            long catalog_size = GetCatalogSize();

            return (catalog_size < desire_catalog_size) ;
        }

        private static void DeleteOrphans(List<ulong> a_chapters = null)
        {
            Server[] servers = LoadCatalog();
            List<ulong> ids = new List<ulong>();

            foreach (var file in GetCatalogFiles())
            {
                ulong id;
                if (UInt64.TryParse(Path.GetFileNameWithoutExtension(file.Name), out id))
                    ids.Add(id);
            }

            foreach (Server server in servers)
            {
                ids.Remove(server.ID);

                foreach (var serie in LoadServerSeries(server))
                {
                    ids.Remove(serie.ID);

                    foreach (var chapter in LoadSerieChapters(serie))
                    {
                        ids.Remove(chapter.ID);

                        if (a_chapters != null)
                            a_chapters.Add(chapter.ID);
                    }
                }
            }

            foreach (var id in ids)
                DeleteCatalogFile(id);
        }

        public static void Compact(long a_max_catalog_size, Func<bool> a_cancel)
        {
            if (HashWorks())
                return;
            
            try
            {
                List<ulong> ids = new List<ulong>();

                foreach (var file in GetCatalogFiles())
                {
                    ulong id;
                    if (!UInt64.TryParse(Path.GetFileNameWithoutExtension(file.Name), out id))
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            Loggers.MangaCrawler.Error("Exception", ex);
                        }
                    }
                }

                List<ulong> chapters = new List<ulong>();
                DeleteOrphans(chapters);
                DeleteChaptersWithoutImages(chapters);
                DeleteSeriesWithoutChapters();
                CompactBruteForce(a_max_catalog_size);
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }

        private static void CompactBruteForce(long a_max_catalog_size)
        {
            List<ulong> ids = new List<ulong>();

            foreach (var file in GetCatalogFiles())
            {
                ulong id;
                if (UInt64.TryParse(Path.GetFileNameWithoutExtension(file.Name), out id))
                    ids.Add(id);
            }

            for (;;)
            {
                if (IsCompacted(a_max_catalog_size))
                    break;

                var oldest = from id in ids
                             orderby new FileInfo(GetCatalogFile(id)).LastWriteTimeUtc
                             select id;
                var to_remove = ids.Take(100).ToList();
                ids.RemoveRange(to_remove);

                foreach (var id in to_remove)
                    DeleteCatalogFile(id);
                DeleteOrphans();
            }
        }

        private static void DeleteSeriesWithoutChapters()
        {
            Server[] servers = LoadCatalog();
            List<ulong> ids = new List<ulong>();

            foreach (var file in GetCatalogFiles())
            {
                ulong id;
                if (UInt64.TryParse(Path.GetFileNameWithoutExtension(file.Name), out id))
                    ids.Add(id);
            }

            foreach (Server server in servers)
            {
                foreach (var serie in LoadServerSeries(server))
                {
                    var chapters = LoadSerieChapters(serie);

                    bool empty_serie = chapters.All(ch => !ids.Contains(ch.ID));

                    if (empty_serie)
                        DeleteCatalogFile(serie.ID);
                }
            }
        }

        private static void DeleteChaptersWithoutImages(List<ulong> a_chapters)
        {
            var oldest = from id in a_chapters
                         orderby new FileInfo(GetCatalogFile(id)).LastWriteTimeUtc
                         select id;

            foreach (var old in oldest)
            {
                var image_files = LoadCatalogXml(old).Element(PAGES_NODE).Element(PAGES_PAGES_NODE).
                    Elements(PAGES_PAGE_NODE).Select(el => el.Element(PAGES_PAGE_IMAGEFILEPATH_NODE).Value);

                Func<bool> no_images = () =>
                {
                    foreach (var image in image_files)
                    {
                        try
                        {
                            if (new FileInfo(image).Exists)
                                return false;
                        }
                        catch (Exception ex)
                        {
                            Loggers.MangaCrawler.Error("Exception", ex);
                            return true;
                        }
                    }

                    return true;
                };

                if (no_images())
                    DeleteCatalogFile(old);
            }
        }

        internal static List<Chapter> LoadWorks(IEnumerable<Server> a_servers)
        {
            if (!new FileInfo(WorksFile).Exists)
                return new List<Chapter>();

            try
            {
                XElement root = XDocument.Load(WorksFile).Element(WORKS_NODE);

                List<Chapter> works = new List<Chapter>();

                foreach (var work in root.Elements(WORKS_WORK_NODE))
                {
                    ulong server_id = UInt64.Parse(work.Element(WORKS_SERVER_ID_NODE).Value);
                    Server server = a_servers.First(s => s.ID == server_id);

                    ulong serie_id = UInt64.Parse(work.Element(WORKS_SERIE_ID_NODE).Value);
                    Serie serie = server.Series.First(s => s.ID == serie_id);

                    ulong chapter_id = UInt64.Parse(work.Element(WORKS_CHAPTER_ID_NODE).Value);
                    Chapter chapter = serie.Chapters.First(c => c.ID == chapter_id);

                    works.Add(chapter);
                }

                return works;
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
                DeleteFile(WorksFile);
                return new List<Chapter>();
            }
        }

        private static bool HashWorks()
        {
            XDocument doc = LoadXml(WorksFile);

            if (doc == null)
                return false;

            try
            {
                return doc.Element(WORKS_NODE).Elements(WORKS_WORK_NODE).Any();
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
                return false;
            }
        }

        internal static void SaveWorks(IEnumerable<Chapter> a_works)
        {
            try
            {
                lock (m_lock)
                {
                    new DirectoryInfo(CatalogDir).Create();

                    var xml = new XElement(WORKS_NODE,
                        from work in a_works
                        select new XElement(WORKS_WORK_NODE,
                            new XElement(WORKS_SERVER_ID_NODE, work.Server.ID),
                            new XElement(WORKS_SERIE_ID_NODE, work.Serie.ID),
                            new XElement(WORKS_CHAPTER_ID_NODE, work.ID)));
                       
                    xml.Save(WorksFile);
                }
            }
            catch (Exception ex)
            {
                Loggers.MangaCrawler.Error("Exception", ex);
            }
        }
    }
}
