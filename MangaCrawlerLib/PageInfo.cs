﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Threading;
using TomanuExtensions.Utils;

namespace MangaCrawlerLib
{
    public class PageInfo
    {
        private string m_imageURL;
        private string m_name;
        private string m_url;
        private string m_imageFilePath;

        internal ChapterInfo ChapterInfo { get; private set; }
        internal int Index { get; private set; }
        internal string URLPart { get; private set; }
        internal bool Downloaded { get; private set; }

        internal PageInfo(ChapterInfo a_chapterInfo, string a_urlPart, int a_index, string a_name = null)
        {
            ChapterInfo = a_chapterInfo;
            URLPart = a_urlPart;
            Index = a_index;

            if (a_name != null)
            {
                a_name = a_name.Trim();
                a_name = a_name.Replace("\t", " ");
                while (a_name.IndexOf("  ") != -1)
                    a_name = a_name.Replace("  ", " ");
                m_name = HttpUtility.HtmlDecode(m_name);
                m_name = FileUtils.RemoveInvalidFileDirectoryCharacters(a_name);
            }
        }

        internal string Name
        {
            get
            {
                if (m_name == null)
                    return Index.ToString();
                else
                    return m_name;
            }
        }

        internal string URL
        {
            get
            {
                if (m_url == null)
                    m_url = HttpUtility.HtmlDecode(ChapterInfo.SerieInfo.ServerInfo.Crawler.GetPageURL(this));

                return m_url;
            }
        }

        internal string GetImageURL()
        {
            if (m_imageURL == null)
                m_imageURL = HttpUtility.HtmlDecode(ChapterInfo.SerieInfo.ServerInfo.Crawler.GetImageURL(this));

            return m_imageURL;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}/{2}",
                    ChapterInfo, Index, ChapterInfo.Pages.Count());
        }

        internal MemoryStream GetImageStream()
        {
            return ConnectionsLimiter.GetImageStream(this);
        }

        public string GetImageFilePath()
        {
            return m_imageFilePath;
        }

        public void DownloadAndSavePageImage(string a_dir)
        {
            m_imageFilePath = a_dir +
                FileUtils.RemoveInvalidFileDirectoryCharacters(Name) +
                FileUtils.RemoveInvalidFileDirectoryCharacters(
                    Path.GetExtension(GetImageURL()).ToLower());

            FileInfo image_file = new FileInfo(m_imageFilePath);

            new DirectoryInfo(a_dir).Create();

            FileInfo temp_file = new FileInfo(Path.GetTempFileName());

            try
            {
                using (FileStream file_stream = new FileStream(temp_file.FullName, FileMode.Create))
                {
                    Stream ims = null;

                    try
                    {
                        ims = GetImageStream();
                    }
                    catch (WebException)
                    {
                        // Some images are unavailable, if we get null pernamently tests
                        // will detect this.
                        return;
                    }

                    try
                    {
                        System.Drawing.Image.FromStream(ims);
                        ims.Position = 0;
                    }
                    catch
                    {
                        // Some junks.
                        return;
                    }

                    ims.CopyTo(file_stream);
                }

                if (image_file.Exists)
                    image_file.Delete();
                temp_file.MoveTo(image_file.FullName);
            }
            catch
            {
                temp_file.Delete();
                throw;
            }

            Downloaded = true;
        }
    }
}
