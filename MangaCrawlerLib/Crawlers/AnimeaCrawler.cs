﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using TomanuExtensions;
using System.Threading;

namespace MangaCrawlerLib.Crawlers
{
    internal class AnimeaCrawler : Crawler
    {
        public override string Name
        {
            get 
            {
                return "Animea";
            }
        }

        internal override void DownloadSeries(Server a_server, Action<int, IEnumerable<Serie>> a_progress_callback)
        {
            HtmlDocument doc = DownloadDocument(a_server);

            var last_page = Int32.Parse(
                doc.DocumentNode.SelectNodes("//ul[@class='paging']//li/a").Reverse().
                    Skip(1).First().InnerText);

            ConcurrentBag<Tuple<int, int, string, string>> series =
                new ConcurrentBag<Tuple<int, int, string, string>>();

            int series_progress = 0;

            Action<int> update = (progress) =>
            {
                var result = from serie in series
                             orderby serie.Item1, serie.Item2
                             select new Serie(a_server, serie.Item4, serie.Item3);

                a_progress_callback(progress, result.ToArray());
            };

            Parallel.For(0, last_page,
                new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxConnectionsPerServer,
                    TaskScheduler = Limiter.Scheduler
                },
                (page, state) =>
                {
                    try
                    {
                        string url = GetServerURL();
                        if (page > 0)
                            url += String.Format("?page={0}", page);

                        HtmlDocument page_doc = DownloadDocument(
                            a_server, url);

                        var page_series = page_doc.DocumentNode.SelectNodes(
                            "//ul[@class='mangalist']/li/div/a");
                       
                        for (int i = 0; i < page_series.Count; i++)
                        {
                            Tuple<int, int, string, string> s = new Tuple<int, int, string, string>(
                                page, 
                                i, 
                                page_series[i].InnerText, 
                                "http://manga.animea.net" + page_series[i].GetAttributeValue("href", ""));

                            series.Add(s);
                        }

                        Interlocked.Increment(ref series_progress);
                        update(series_progress * 100 / last_page);
                    }
                    catch
                    {
                        state.Break();
                        throw;
                    }
                });

            update(100);
        }

        internal override void DownloadChapters(Serie a_serie, Action<int, IEnumerable<Chapter>> a_progress_callback)
        {
            HtmlDocument doc = DownloadDocument(a_serie);

            var chapters = doc.DocumentNode.SelectNodes("//ul[@class='chapters_list']/li/a");

            if (chapters == null)
            {
                var skip_link = doc.DocumentNode.SelectSingleNode("//li[@class='notice']/strong/a");
                doc = DownloadDocument(a_serie, a_serie.URL + 
                    skip_link.GetAttributeValue("href", ""));
                chapters = doc.DocumentNode.SelectNodes("//ul[@class='chapters_list']/li/a");
            }

            var result = from chapter in chapters
                         select new Chapter(
                             a_serie,
                             "http://manga.animea.net" + chapter.GetAttributeValue("href", ""), 
                             chapter.InnerText);

            a_progress_callback(100, result);
        }

        internal override IEnumerable<Page> DownloadPages(Chapter a_chapter)
        {
            HtmlDocument doc = DownloadDocument(a_chapter);

            var pages = doc.DocumentNode.SelectSingleNode("//select[@name='page']").SelectNodes("option");

            foreach (var page in pages)
            {
                var url =  a_chapter.URL.RemoveFromRight(5) + "-page-" +
                    page.GetAttributeValue("value", "") + ".html";

                yield return new Page(
                    a_chapter,
                    url, 
                    pages.IndexOf(page) + 1,
                    page.NextSibling.InnerText);
            }
        }

        public override string GetServerURL()
        {
            return "http://manga.animea.net/browse.html";
        }

        internal override string GetImageURL(Page a_page)
        {
            HtmlDocument doc = DownloadDocument(a_page);
            var image = doc.DocumentNode.SelectSingleNode("//img[@class='mangaimg']");
            return image.GetAttributeValue("src", "");
        }
    }
}