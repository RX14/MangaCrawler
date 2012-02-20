﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MangaCrawlerLib;

namespace MangaCrawler
{
    public class WorkGridRow
    {
        public Work Work { get; private set; }

        public WorkGridRow(Work a_work)
        {
            Work = a_work;
        }

        public string Progress
        {
            get
            {
                ChapterState s = Work.Chapter.State;

                switch (s)
                {
                    case ChapterState.Error:
                    return MangaCrawler.Properties.Resources.WorkProgressError;
                    case ChapterState.Aborted:
                    return MangaCrawler.Properties.Resources.WorkProgressAborted;
                    case ChapterState.Waiting:
                    return MangaCrawler.Properties.Resources.WorkProgressWaiting;
                    case ChapterState.Deleting:
                    return MangaCrawler.Properties.Resources.WorkProgressDeleting;
                    case ChapterState.Downloaded:
                    return MangaCrawler.Properties.Resources.WorkProgressDownloaded;
                    case ChapterState.Zipping:
                    return MangaCrawler.Properties.Resources.WorkProgressZipping;
                    case ChapterState.Downloading:
                    return String.Format("{0}/{1}", Work.Chapter.DownloadedPages, Work.Chapter.Pages.Count());
                    default: throw new NotImplementedException();
                }
            }
        }

        public string Info
        {
            get
            {
                return String.Format(MangaCrawler.Properties.Resources.DownloadingChapterInfo,
                     Work.Chapter.Serie.Server.Name, Work.Chapter.Serie.Title, Work.Chapter.Title);
            }
        }
    }
}