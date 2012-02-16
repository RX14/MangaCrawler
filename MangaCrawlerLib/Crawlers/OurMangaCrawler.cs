﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading;
using TomanuExtensions;

namespace MangaCrawlerLib
{
    internal class OurMangaCrawler : Crawler
    {
        public override string Name
        {
            get 
            {
                return "OurManga";
            }
        }

        public override void DownloadSeries(ServerInfo a_info, Action<int, IEnumerable<SerieInfo>> a_progress_callback)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            var series = doc.DocumentNode.SelectNodes("//div[@class='m_s_title']/a");

            var result = from serie in series.Skip(1)
                         select new SerieInfo(a_info,
                                              serie.GetAttributeValue("href", "").RemoveFromRight(1),
                                              serie.InnerText);

            a_progress_callback(100, result);
        }

        public override void DownloadChapters(SerieInfo a_info, Action<int, IEnumerable<ChapterInfo>> a_progress_callback)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            var chapters = doc.DocumentNode.SelectNodes("//div[@id='manga_nareo']/div").Skip(1);

            var chs = (from ch in chapters
                       where !ch.SelectSingleNode("div[3]").InnerText.ToLower().Contains("soon")
                       select ch.SelectSingleNode("div[1]/a")).ToArray();

            List<ChapterInfo> result = new List<ChapterInfo>();
            foreach (var ch in chs)
            {
                if (ch != null)
                    result.Add(new ChapterInfo(a_info, ch.GetAttributeValue("href", ""), ch.InnerText));
            }

            a_progress_callback(100, result);
        }

        public override IEnumerable<PageInfo> DownloadPages(TaskInfo a_info)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            var url = doc.DocumentNode.SelectSingleNode("//div[@id='Summary']/p[2]/a[2]");

            doc = DownloadDocument(a_info.Server, url.GetAttributeValue("href", ""));

            if (a_info.Token.IsCancellationRequested)
            {
                Loggers.Cancellation.InfoFormat(
                    "Pages - token cancelled, a_url: {0}",
                    a_info.URL);

                a_info.Token.ThrowIfCancellationRequested();
            }


            var pages = doc.DocumentNode.SelectNodes("//div[@class='inner_heading_right']/h3/select[2]/option");

            int index = 0;
            foreach (var page in pages)
            {
                index++;

                PageInfo pi = new PageInfo(a_info, page.GetAttributeValue("value", ""), index, page.NextSibling.InnerText);

                yield return pi;
            }
        }

        public override string GetImageURL(PageInfo a_info)
        {
            HtmlDocument doc = DownloadDocument(a_info.TaskInfo.Server,
                a_info.TaskInfo.URLPart + "/" + a_info.URLPart);

            var node = doc.DocumentNode.SelectSingleNode("//div[@class='inner_full_view']/h3/a/img");

            if (node == null)
                node = doc.DocumentNode.SelectSingleNode("//div[@class='inner_full_view']/h3/img");

            return node.GetAttributeValue("src", "");
        }

        public override string GetServerURL()
        {
            return "http://www.ourmanga.com/directory/";
        }
    }
}
