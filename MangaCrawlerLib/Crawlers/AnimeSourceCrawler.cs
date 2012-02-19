﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using TomanuExtensions;

namespace MangaCrawlerLib
{
    internal class AnimeSourceCrawler : Crawler
    {
        public override string Name
        {
            get
            {
                return "Anime-Source";
            }
        }

        public override void DownloadSeries(ServerInfo a_info, 
            Action<int, IEnumerable<SerieInfo>> a_progress_callback)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            var series = doc.DocumentNode.SelectNodes(
                "/html/body/center/table/tr/td/table[5]/tr/td/table/tr/td/table/tr/td/table/tr/td[2]");

            var result = from serie in series
                         where (serie.ChildNodes[7].InnerText.Trim() != "2")
                         orderby serie.SelectSingleNode("font").FirstChild.InnerText
                         select new SerieInfo(a_info,
                                              "http://www.anime-source.com/banzai/" + 
                                              serie.SelectSingleNode("a[2]").GetAttributeValue("href", ""),
                                              serie.SelectSingleNode("font").FirstChild.InnerText);

            a_progress_callback(100, result);
        }

        public override void DownloadChapters(SerieInfo a_info, Action<int, IEnumerable<ChapterInfo>> a_progress_callback)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            var chapters = doc.DocumentNode.SelectNodes(
                "/html/body/center/table/tr/td/table[5]/tr/td/table/tr/td/table/tr/td/blockquote/a");

            var result = from chapter in chapters.Skip(1)
                         select new ChapterInfo(a_info, 
                                                "http://www.anime-source.com/banzai/" + chapter.GetAttributeValue("href", ""), 
                                                chapter.InnerText);

            a_progress_callback(100, result.Reverse());
        }

        public override IEnumerable<PageInfo> DownloadPages(TaskInfo a_info)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            var pages = doc.DocumentNode.SelectNodes("//select[@name='pageid']/option");

            if (pages == null)
            {
                string pages_str = doc.DocumentNode.SelectSingleNode(
                    "/html/body/center/table/tr/td/table[5]/tr/td/table/tr/td/table/tr/td/font[2]").ChildNodes[4].InnerText;

                int pages_count = Int32.Parse(pages_str.Split(new char[] { '/' }).Last());

                for (int page = 1; page <= pages_count; page++)
                {
                    PageInfo pi = new PageInfo(a_info, a_info.URL + "&page=" + page, page);

                    yield return pi;
                }
            }
            else
            {
                int index = 0;
                foreach (var page in pages)
                {
                    index++;

                    PageInfo pi = new PageInfo(a_info, 
                                               "http://www.anime-source.com/banzai/" + page.GetAttributeValue("value", ""),
                                               index);

                    yield return pi;
                }
            }
        }

        public override string GetImageURL(PageInfo a_info)
        {
            HtmlDocument doc = DownloadDocument(a_info);

            string xpath;
            if (a_info.TaskInfo.Pages.Count() == a_info.Index)
                xpath = "/html/body/center/table/tr/td/table[5]/tr/td/div/img";
            else
                xpath = "/html/body/center/table/tr/td/table[5]/tr/td/div/a/img";

            var node = doc.DocumentNode.SelectSingleNode(xpath);

            if (node == null)
            {
                node = doc.DocumentNode.SelectSingleNode(
                    "/html/body/center/table/tr/td/table[5]/tr/td/table/tr/td/table/tr/td/font[2]/p[2]/img");

                if (node == null)
                    node = doc.DocumentNode.SelectSingleNode(
                        "/html/body/center/table/tr/td/table[5]/tr/td/table/tr/td/table/tr/td/font[2]/p/img");

                return node.GetAttributeValue("src", "");
            }
            else
                return "http://www.anime-source.com" + node.GetAttributeValue("src", "");
        }
        
        public override string GetServerURL()
        {
            return "http://www.anime-source.com/banzai/modules.php?name=Manga";
        }
    }
}
