﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading;
using TomanuExtensions;
using System.Diagnostics;

namespace MangaCrawlerLib
{
    internal class SpectrumNexusCrawler : Crawler
    {
        internal override string Name
        {
            get 
            {
                return "Spectrum Nexus";
            }
        }

        internal override void DownloadSeries(ServerInfo a_info, Action<int, 
            IEnumerable<SerieInfo>> a_progress_callback)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info);

            var series = doc.DocumentNode.SelectNodes("//div[@class='mangaJump']/select").Elements().ToList();


            for (int i = series.Count - 1; i >= 0; i--)
            {
                if (series[i].NodeType != HtmlNodeType.Text)
                    continue;
                string str = series[i].InnerText;
                str = str.Trim();
                str = str.Replace("\n", "");
                if (str == "")
                    series.RemoveAt(i);
            }
            
            var splitter = series.FirstOrDefault(s => s.InnerText.Contains("---"));
            if (splitter != null)
            {
                int splitter_index = series.IndexOf(splitter);
                series.RemoveRange(0, splitter_index + 1);
            }

            List<SerieInfo> result = new List<SerieInfo>();

            for (int i = 0; i < series.Count; i += 2)
            {
                SerieInfo si = new SerieInfo(a_info,
                    series[i].GetAttributeValue("value", ""), series[i + 1].InnerText);
                result.Add(si);
            }

            a_progress_callback(100, result);
        }

        internal override void DownloadChapters(SerieInfo a_info, Action<int, IEnumerable<ChapterInfo>> a_progress_callback)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info);

            var link_strong_nodes = doc.DocumentNode.SelectNodes("//a");
            var begin_reading_strong_node = link_strong_nodes.Where(
                n => n.InnerText.StartsWith("Begin Reading"));
            var href = begin_reading_strong_node.First().GetAttributeValue("href", "");

            doc = ConnectionsLimiter.DownloadDocument(a_info, href);

            var chapters = doc.DocumentNode.SelectNodes("//select[@name='ch']/option");

            var result = from chapter in chapters
                            select new ChapterInfo(a_info, href + "?ch=" +
                                chapter.GetAttributeValue("value", "").Replace(" ", "+"),
                                chapter.NextSibling.InnerText);

            a_progress_callback(100, result.Reverse());
        }

        internal override IEnumerable<PageInfo> DownloadPages(TaskInfo a_info)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info);

            var pages = doc.DocumentNode.SelectNodes("//select[@name='page']/option");

            int index = 0;
            foreach (var page in pages)
            {
                index++;

                PageInfo pi = new PageInfo(a_info, a_info.URLPart + "&page=" + 
                    page.GetAttributeValue("value", ""), 
                    index, page.NextSibling.InnerText);

                yield return pi;
            }
        }

        internal override string GetImageURL(PageInfo a_info)
        {
            HtmlDocument doc = ConnectionsLimiter.DownloadDocument(a_info);

            var img = doc.DocumentNode.SelectSingleNode("//div[@class='imgContainer']/a/img");

            if (a_info.URL.ToLower().Contains("view.thespectrum.net"))
            {
                return "http://view.thespectrum.net/" + img.GetAttributeValue("src", "").
                    RemoveFromLeft(1);
            }
            else
            {
                return "http://view.mangamonger.com/" + img.GetAttributeValue("src", "").
                    RemoveFromLeft(1);
            }
        }

        internal override string GetChapterURL(ChapterInfo a_info)
        {
            return "http://www.thespectrum.net" + a_info.URLPart + "&page=1";
        }

        internal override string GetSerieURL(SerieInfo a_info)
        {
            return "http://www.thespectrum.net" + a_info.URLPart;
        }

        internal override string GetServerURL()
        {
            return "http://www.thespectrum.net/";
        }
    }
}
