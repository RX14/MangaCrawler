﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading;

namespace MangaCrawlerLib
{
    internal class MangaShareCrawler : Crawler
    {
        internal override string Name
        {
            get
            {
                return "MangaShare";
            }
        }

        internal override void DownloadSeries(ServerInfo a_info, Action<int, IEnumerable<SerieInfo>> a_progress_callback)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info);

            var series = doc.DocumentNode.SelectNodes("//table[@class='datalist']/tr[@class='datarow']");

            var result = from serie in series 
                         select new SerieInfo(a_info, 
                                              serie.SelectSingleNode("td[@class='datarow-0']/a").GetAttributeValue("href", "").
                                                  Split(new char[] { '/' }).Last(), 
                                              serie.SelectSingleNode("td[@class='datarow-1']/text()").InnerText);

            a_progress_callback(100, result);
        }

        internal override void DownloadChapters(SerieInfo a_info, Action<int, IEnumerable<ChapterInfo>> a_progress_callback)
        {
            string url = String.Format("{0}/chapter-001/page001.html", a_info.URL);
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info);

            var chapters = doc.DocumentNode.SelectNodes("//select[@name='chapterjump']/option");

            var result = from chapter in chapters 
                         select new ChapterInfo(a_info, chapter.GetAttributeValue("Value", ""), chapter.NextSibling.InnerText);

            a_progress_callback(100, result);
        }

        internal override IEnumerable<PageInfo> DownloadPages(ChapterInfo a_info, CancellationToken a_token)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info, a_token);

            var pages = doc.DocumentNode.SelectNodes("//select[@name='pagejump']/option");

            int index = 0;
            foreach (var page in pages)
            {
                index++;

                PageInfo pi = new PageInfo(a_info, page.GetAttributeValue("Value", ""), index);

                yield return pi;
            }
        }

        internal override string GetImageURL(PageInfo a_info, CancellationToken a_token)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info, a_token);

            HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@id='page']/a/img");

            if (node != null)
                return node.GetAttributeValue("src", "");

            return doc.DocumentNode.SelectSingleNode("//div[@id='page']/img").GetAttributeValue("src", "");
        }

        internal override string GetServerURL()
        {
            return "http://read.mangashare.com/dir";
        }

        internal override string GetSerieURL(SerieInfo a_info)
        {
            return "http://read.mangashare.com/" + a_info.URLPart;
        }

        internal override string GetChapterURL(ChapterInfo a_info)
        {
            return String.Format("http://read.mangashare.com/{0}/chapter-{1}/page001.html",
                a_info.SerieInfo.URLPart, a_info.URLPart);
        }

        internal override string GetPageURL(PageInfo a_info)
        {
            return String.Format("http://read.mangashare.com/{0}/chapter-{1}/page{2}.html",
                                 a_info.ChapterInfo.SerieInfo.URLPart, a_info.ChapterInfo.URLPart, a_info.URLPart);
        }
    }
}