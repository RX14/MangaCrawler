﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using MangaCrawlerLib;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using MangaCrawler.Properties;
using System.Diagnostics;
using System.Media;
using HtmlAgilityPack;
using System.Threading;
using log4net.Core;
using log4net.Layout;
using log4net.Config;
using log4net;
using System.Runtime.InteropServices;
using TomanuExtensions;
using System.Drawing.Drawing2D;
using System.Security.AccessControl;

namespace MangaCrawler
{
    /* 
     * TESTY
     * 
     * usuniecie serii dodanej do bookmark - odswiezenie w series tab  jak i w bookmark tab
     *   w series tab po zmianie serwera powinna zniknac, w bookmarks powina zniknac po okresowym sprawdzeniu 
     *   dostepnosci nowych chapterow, podobnie w zakladce serie tab jesli nie przeklikamy serwerow powinna zniknac 
     *   po okresowym sprawdzeniu bookmarks
     *   
     * usuniecie rozdzialu z bookmark serii ktory jest nowy powinno zdjac znacznik new
     * 
     * dodawanie do bookmark serii ma nie dzialac podczas pobierania serii.
     * 
     * usuniecie z bookmark zachowuje zaznaczenie w liscie
     * 
     * dodanie do bookmark dwa razy tego samego to blad
     * 
     * download (oba przyciski) juz pobieranych rzeczy to blad
     * 
     * dodanie nowego chapteru powinno spowodowac pojawienie sie nowego rozdzialu w bookmarks
     * 
     * dodanie nowego chapteru w minimalizacji powinno pokazac tooltip
     * 
     * klikniecie w tooltip powinno pokazac zkladke bookmarks, serie i nowy rozdzial, 
     *   kiedy cos tam jest wybrane powinno zniknac
     *   
     * pojawienie sie czegos nowego niezaleznie od stanu minimalizajci zakladki, a takze znikniecie tego 
     * powinno zmieniac ikone 
     * 
     * przetestowac elementy puste i elementy generujace bledy przy probie dostepu, sprobowac je dodac do 
     * bookmarks i odswiezyc
     * 
     * zamkniecie na downloading powinno wznowic pobieranie chapteru od poczatku
     * 
     * usuwanie wpisow z downloading: deleted - automatycznie, error, 
     *   downloaded - na rzadanie albo podczas nastepnego uruchomienia, 
     *   downloading - mozna klika tak dlugo jak wejdzie w stan deleting, pozniej dzwiek
     *   
     * wpisy na itemach z listboxach - deleted- trwale zostaje, 
     * downloaded - trwale zostaje, error - trwale zostaje, wszystkie ingi znikaja
     * 
     * priorytety - zalaczyc do pobierania serie - wiele na raz, rozdzialy z jednej serii - wiele na raz, 
     * chaptery dla kazdego serwera - w kolejnosci dodania pojedynczo, dla kazdego chapteru wiele stron na raz, 
     * serie, chaptery maja pierwszenstwo nad stronami, pobieranie chapterow wstrzymuje pobierania serii
     * 
     * ponowic pobranie anulowane rozdzialu
     * 
     * masowe anulowanie rozdzialow z dwoch serwerow, ich ponowne zalaczenie do pobierania
     * 
     * przetestowanie timera na sprawdzanie bookmarka, przetestowanie czasu zanim ponownie sprawdzimy juz pobrane
     * 
     * testy na manga dir ze i bez slasha na koncu
     * 
     * przetestowac detekcje nowej wersji
     * 
     * przetestowanie pamietania i dzialania opcji
     * 
     * dodanie ponownie tego samego pobierania nie wstawia nowego work, pozycja work nie zmienia sie
     * 
     * jesli cbz juz istnieje powinien zostac nadpisany
     * 
     * dzwiek o zakonczeniu pobierania takze jesli pobieramy od startu
     * 
     * zaznaczenie elementu w chapte, przejscie na inna serie, zaznaczenie innego indeksu, przejscie spowrotem - w chapter 
     * powinien byc tylko jeden chapter
     * 
     * usuwanie nieistniejacej zdalnie rzeczy, a zaznaczenie, w bookmark co sie dzieje jesli jednoczesnie jest bookmark, 
     * czy bookmark.xml sie czysci, co jesli zaznaczenie to ostatni item, 
     * 
     * upewnic sie ze uzywane w gui enumeracje sa stale, utrwalane w momencie pobrania, tak by nic nie zmienialo sie 
     * podczas enumeracji, jesli jakies zmiany wprowadza watki pobierajace, dotyczy chapterow dla ktorych jest multiselekcja
     * 
     * przetestowac dzialanie wszystkich przyciskow dla selekcji i multiselekcj, brak wybrania, 
     *   juz zostalo zrobione (dodane, uruchomione)
     *   
     * menu debug nie dziala i nie ma logowania w release
     * 
     * zmiana nazwy, zmiana url - nie tracimy powiazania, zmiana obu - trudno
     * 
     * klikniecie prawym przyciskiem w liste zaznacza element przed pojawieniem sie menu kontekstowego
     * 
     * brak dostepu do katalogu - nie mozna utworzyc katalogu, nie mozna zapisac pliku w katalogu, nie mozna 
     * zapisac cbz, nie mozna podmienic pliku image, nie mozna podmienic pliku cbz
     * 
     * ktos klika w element ktory nie istnieje, pojawia sie error, albo jest on w trakcie sciagania, w tym 
     * czasie nastepuje jego odswiezenie i znika on z listy
     * 
     * zalaczenie do pobierania mnostwa chapterow, serii, itp zobaczyc czy nie bledow pamieciowych
     * 
     * wiele chapterow z jednego chaptera do sciagania, zamkniecie aplikacji, ponowne uruchomienie, 
     * czy wszystkie sa wznawiane, powinny sie wznawiac w kolejnosci dodania
     * 
     * dodac przycisk check now
     * 
     * dla nowych rozdzialow, visit page ma je oznaczac jako odwiedzone dla wszystkich miejsc z ktorych
     * mozemy to zrobic
     * 
     * usuniecie chapteru ktory jest w downloading jako downloaded albo pobierany - test na przyciski - jak to dziala, 
     * takze usuniecie serii
     * 
     * usuniecie serii ktora jest w bookmarks, co sie dzieje 
     * 
     * ogranicz liczbe otwieranych folderow i obrazkow do 10
     * 
     * uruchomienie aplikacji jesli jest uruchomiana przywraca ono juz uruchomionej
     * 
     * uruchomienie aplikacji na czysto - sprawdzanie czy wszystk sie dobrze laduje
     * 
     * dzialanie tab order
     * 
     * minimalizacja, maksymalizacja, przywrocenie - na wszystkich kartach, zwlaszcza tych co maja splitter
     * 
     * skasowanie elementu w downloading przelacza na nastepne zaznaczenie, szczegolnie ostatni, to samo tyczy sie listy 
     * ulubionych
     * 
     * czy dziala autostart, czy po uruchomieniu w autostart jest zminimalizowana
     * 
     * w bookmarked elementy error, cancelled pokazuja takze new
     * 
     * dodanie nowego serwera nie moze niszczyc katalogu, chwilowo nie ma na to zabezpieczen tylko ostrzezenie
     * 
     * przetestowac obluge podwojnych nazw i urli, tylko podwojnych nazw - specjalne testowe serwery
     * 
     * utrwalanie danych w katalogu, a zmiana adresu url, odswiezenie informacji w katalogu
     */

    /* 
     * TODO:
     * 
     * wolne, przejechanie sie po wielu seriach mangafox
     * 
     * dodac do testowych serwerow takie ktore generuja podwojne nazwy i urle, tlyko podwojne nazwy
     * 
     * testy masowego pobierania cala noc
     * testy gui, testy typowego dzialania
     * uruchomic na x86 
     * 
     * wbudowany browser
     * widok wspolny dla wszystkich serwisow, scalac jakos serie,
     *   gdzie najlepsza jakosc, gdzie duplikaty
     * wpf
     * 
     * nowe serwisy:
     * 
     * 6m http://manga.animea.net/browse.html
     * 5m http://mangastream.com
     * 
     * http://www.anymanga.com/directory/all/
     * http://mangable.com/manga-list/
     * http://www.readmangaonline.net/
     * http://www.mangamonger.com/
     */

    public partial class MangaCrawlerForm : Form
    {
        private Dictionary<Server, ListBoxVisualState> m_series_visual_states =
            new Dictionary<Server, ListBoxVisualState>();
        private Dictionary<Serie, ListBoxVisualState> m_chapters_visual_states =
            new Dictionary<Serie, ListBoxVisualState>();
        private Dictionary<Serie, ListBoxVisualState> m_chapter_bookmarks_visual_states =
            new Dictionary<Serie, ListBoxVisualState>();
        private Dictionary<Tabs, Control> m_focus = new Dictionary<Tabs, Control>();

        private Tabs m_front_panel;
        private bool m_resizing = true;

        private static Color BAD_DIR = Color.Red;

        [DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string a_name);

        private uint WM_TASKBARCREATED;

        private MangaCrawlerFormCommands Commands;
        private MangaCrawlerFormGUI GUI;

        private Color ActiveLabelColor;
        private Color InactiveLabelColor;

        public enum Tabs
        {
            Series,
            Downloadings,
            Bookmarks,
            Options,
            Log
        }

        public MangaCrawlerForm()
        {
            Commands = new MangaCrawlerFormCommands();
            GUI = new MangaCrawlerFormGUI();
            GUI.Form = this;
            GUI.Commands = Commands;
            Commands.GUI = GUI;

            InitializeComponent();
            Settings.Instance.FormState.Init(this);
        }

        private void MangaShareCrawlerForm_Load(object sender, EventArgs e)
        {
            SetupLog4NET();

            ActiveLabelColor = seriesTabLabel.ForeColor;
            InactiveLabelColor = downloadingsTabLabel.ForeColor;

            ListenForRestoreEvent();

            WM_TASKBARCREATED = RegisterWindowMessage("TaskbarCreated");

            Text = String.Format("{0} {1}.{2}", Text,
                Assembly.GetAssembly(GetType()).GetName().Version.Major, 
                Assembly.GetAssembly(GetType()).GetName().Version.Minor);

            DownloadManager.Create(
                Settings.Instance.MangaSettings, 
                Settings.GetSettingsDir());

            GUI.Init();

            MovePanelFromTabControl(seriesTabPanel);
            MovePanelFromTabControl(downloadingsTabPanel);
            MovePanelFromTabControl(bookmarksTabPanel);
            MovePanelFromTabControl(optionsTabPanel);
            MovePanelFromTabControl(logTabPanel);
            tabControl.Hide();

            seriesListBox.Focus();
            m_focus[Tabs.Series] = seriesListBox;
            m_focus[Tabs.Downloadings] = downloadingsGridView;
            m_focus[Tabs.Bookmarks] = bookmarkedSeriesListBox;
            m_focus[Tabs.Options] = mangaRootDirChooseButton;
            m_focus[Tabs.Log] = logRichTextBox;

            FrontTab = Tabs.Log;
            FrontTab = Tabs.Series;

            versionLinkLabel.TabStop = false;

            mangaRootDirTextBox.Text = Settings.Instance.MangaSettings.GetMangaRootDir(false);
            seriesSearchTextBox.Text = Settings.Instance.SeriesFilter;
            cbzCheckBox.Checked = Settings.Instance.MangaSettings.UseCBZ;
            minimizeOnCloseCheckBox.Checked = Settings.Instance.MinimizeOnClose;
            if (!minimizeOnCloseCheckBox.Checked)
                showBaloonTipsCheckBox.Enabled = false;
            showBaloonTipsCheckBox.Checked = Settings.Instance.ShowBaloonTips;

            if (Settings.Instance.MangaSettings.PageNamingStrategy == PageNamingStrategy.DoNotChange)
                pageNamingStrategyComboBox.SelectedIndex = 0;
            else if (Settings.Instance.MangaSettings.PageNamingStrategy == PageNamingStrategy.PrefixToPreserverOrder)
                pageNamingStrategyComboBox.SelectedIndex = 1;
            else if (Settings.Instance.MangaSettings.PageNamingStrategy == PageNamingStrategy.IndexToPreserveOrder)
                pageNamingStrategyComboBox.SelectedIndex = 2;
            else if (Settings.Instance.MangaSettings.PageNamingStrategy == PageNamingStrategy.AlwaysUsePrefix)
                pageNamingStrategyComboBox.SelectedIndex = 3;
            else if (Settings.Instance.MangaSettings.PageNamingStrategy == PageNamingStrategy.AlwaysUseIndex)
                pageNamingStrategyComboBox.SelectedIndex = 4;
            else
                Loggers.GUI.Error("Invalid PageNamingStrategy");

            playSoundWhenDownloadedCheckBox.Checked = Settings.Instance.PlaySoundWhenDownloaded;

            Task.Factory.StartNew(() => Commands.CheckNewVersion(), TaskCreationOptions.LongRunning);

            if (!Loggers.Log())
            {
                logTabLabel.Hide();
                LogManager.Shutdown();
                ContextMenuStrip = null;
            }

            // Flicker-free.
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, downloadingsGridView, new object[] { true });

            downloadingsGridView.AutoGenerateColumns = false;
            downloadingsGridView.DataSource = new BindingList<DownloadingGridRow>();

            Commands.CheckNowBookmarks();

            refreshTimer.Enabled = true;

            bookmarksTimer.Interval = (int)Settings.Instance.MangaSettings.CheckTimePeriod.TotalMilliseconds / 10;
            bookmarksTimer.Enabled = true;

            ResizeToolStripImages();
            ResizeContextMenuStripImages();

            // VS designer bug
            // this post in from 2006, no comment...
            // http://social.msdn.microsoft.com/Forums/ar/winformsdesigner/thread/6f56b963-df4d-4f26-8dc3-0244d129f07c
            foreach (var ts in FindAll<ToolStrip>())
                ts.Visible = true;

            GUI.UpdateAll();
        }

        private void MovePanelFromTabControl(Panel a_panel)
        {
            int MARGIN_LEFT_RIGHT = 8;
            int MARGIN_BOTTOM = 8;
            int MARGIN_TOP = 30;

            a_panel.Parent = this;
            a_panel.Anchor = tabControl.Anchor;
            a_panel.Width = ClientSize.Width - 2 * MARGIN_LEFT_RIGHT;
            a_panel.Left = MARGIN_LEFT_RIGHT;
            a_panel.Top = MARGIN_TOP;
            a_panel.Height = ClientSize.Height - MARGIN_TOP - MARGIN_BOTTOM;
        }

        private void ListenForRestoreEvent()
        {
            Thread thread = new Thread(() =>
            {
                for (; ; )
                {
                    if (Program.RestoreEvent.WaitOne())
                    {
                        Invoke(new Action(() =>
                        {
                            GUI.MinimizeToTray(false);
                            Settings.Instance.FormState.RestoreFormState(this);
                            Activate();
                        }));
                    }
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        private void ResizeContextMenuStripImages()
        {
            foreach (var c in FindAll<Control>())
            {
                if (c.ContextMenuStrip == null)
                    continue;

                foreach (var item in c.ContextMenuStrip.Items.OfType<ToolStripMenuItem>())
                {
                    if (item.Image == null)
                        continue;

                    var image = ResizeImage(item.Image, c.ContextMenuStrip.ImageScalingSize);
                    item.Image.Dispose();
                    item.Image = image;
                }
            }
        }

        private void ResizeToolStripImages()
        {
            foreach (var toolstrip in FindAll<ToolStrip>())
            {
                foreach (var button in toolstrip.Items.OfType<ToolStripButton>())
                {
                    if (button.Image == null)
                        continue;

                    var image = ResizeImage(button.Image, toolstrip.ImageScalingSize);
                    button.Image.Dispose();
                    button.Image = image;
                }
            }
        }

        private Bitmap ResizeImage(Image a_image, Size a_size)
        {
            Bitmap bmp = new Bitmap(a_size.Width, a_size.Height, a_image.PixelFormat);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(a_image, 0, 0, bmp.Width, bmp.Height);
            }

            return bmp;
        }

        private IEnumerable<T> FindAll<T>(Control a_control = null)
        {
            List<T> list = new List<T>();

            if (a_control == null)
                a_control = this;

            foreach (var c in a_control.Controls.OfType<T>())
                list.Add(c);

            foreach (var c in a_control.Controls.Cast<Control>())
                list.AddRange(FindAll<T>(c));

            return list;
        }

        protected override void WndProc(ref Message a_msg)
        {
            if (a_msg.Msg == WM_TASKBARCREATED)
            {
                if (notifyIcon.Visible)
                    notifyIcon.Visible = true;
            }

            base.WndProc(ref a_msg);
        }

        private void SetupLog4NET()
        {
            RichTextBoxAppender rba = new RichTextBoxAppender(logRichTextBox);
            rba.Threshold = Level.All;
            rba.Layout = new PatternLayout(
                "%date{yyyy-MM-dd HH:mm:ss,fff} %-7level %-14logger %thread %class.%method - %message %newline");

            LevelTextStyle ilts = new LevelTextStyle();
            ilts.Level = Level.Info;
            ilts.TextColor = Color.Black;
            rba.AddMapping(ilts);

            LevelTextStyle dlts = new LevelTextStyle();
            dlts.Level = Level.Debug;
            dlts.TextColor = Color.LightBlue;
            rba.AddMapping(dlts);

            LevelTextStyle wlts = new LevelTextStyle();
            wlts.Level = Level.Warn;
            wlts.TextColor = Color.Yellow;
            rba.AddMapping(wlts);

            LevelTextStyle elts = new LevelTextStyle();
            elts.Level = Level.Error;
            elts.TextColor = Color.Red;
            rba.AddMapping(elts);

            BasicConfigurator.Configure(rba);
            rba.ActivateOptions();
        }

        public Tabs FrontTab
        {
            get
            {
                return m_front_panel;
            }
            set
            {
                if (FrontTab == value)
                    return;

                m_focus[m_front_panel] = ActiveControl;

                m_front_panel = value;

                GetTabPanel(FrontTab).Show();
                GetTabPanel(FrontTab).Focus();

                Control control;
                if (m_focus.TryGetValue(FrontTab, out control))
                {
                    if (control != null)
                        control.Focus();
                }

                foreach (var tab in Enum.GetValues(typeof(Tabs)).Cast<Tabs>())
                {
                    if (tab == FrontTab)
                        continue;

                    GetTabPanel(tab).Hide();
                }

                GUI.UpdateAll();

                foreach (var tab in Enum.GetValues(typeof(Tabs)).Cast<Tabs>())
                    DeactivateTabLabel(GetTabLabel(tab));

                ActivateTabLabel(GetTabLabel(FrontTab));
            }
        }

        private void ActivateTabLabel(Label a_link_label)
        {
            a_link_label.ForeColor = ActiveLabelColor;
        }

        private void DeactivateTabLabel(Label a_link_label)
        {
            a_link_label.ForeColor = InactiveLabelColor;
        }

        private Label GetTabLabel(Tabs a_tab)
        {
            if (a_tab == Tabs.Series)
                return seriesTabLabel;
            else if (a_tab == Tabs.Downloadings)
                return downloadingsTabLabel;
            else if (a_tab == Tabs.Bookmarks)
                return bookmarksTabLabel;
            else if (a_tab == Tabs.Options)
                return optionsTabLabel;
            else if (a_tab == Tabs.Log)
                return logTabLabel;
            else
            {
                Debug.Fail("");
                return seriesTabLabel;
            }
        }

        public Panel GetTabPanel(Tabs a_tab)
        {
            if (a_tab == Tabs.Series)
                return seriesTabPanel;
            else if (a_tab == Tabs.Downloadings)
                return downloadingsTabPanel;
            else if (a_tab == Tabs.Bookmarks)
                return bookmarksTabPanel;
            else if (a_tab == Tabs.Options)
                return optionsTabPanel;
            else if (a_tab == Tabs.Log)
                return logTabPanel;
            {
                Debug.Fail("");
                return seriesTabPanel;
            }
        }

        public void SeriesSplitterAdjust()
        {
            if (WindowState == FormWindowState.Minimized)
                return;

            if (splitPanel.Width - seriesSplitter.SplitPosition < chaptersPanel.MinimumSize.Width)
                seriesSplitter.SplitPosition = splitPanel.Width - chaptersPanel.MinimumSize.Width;

            if (!m_resizing)
                Settings.Instance.SeriesSplitterDistance = seriesSplitter.SplitPosition;
        }

        public void BookmarksSplitterAdjust()
        {
            if (WindowState == FormWindowState.Minimized)
                return;

            if (bookmarksTabPanel.Width - bookmarksSplitter.SplitPosition <
                chapterBookmarksPanel.MinimumSize.Width)
            {
                bookmarksSplitter.SplitPosition =
                    bookmarksTabPanel.Width - chapterBookmarksPanel.MinimumSize.Width;
            }

            if (!m_resizing)
                Settings.Instance.BookmarksSplitterDistance = bookmarksSplitter.SplitPosition;
        }

        private void mangaRootDirChooseButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = mangaRootDirTextBox.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                mangaRootDirTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void seriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GUI.SelectedServer != null)
                m_series_visual_states[GUI.SelectedServer] = 
                    new ListBoxVisualState(seriesListBox);

            Commands.DownloadChapterForSelectedSerie();
        }

        private void chaptersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GUI.SelectedSerie != null)
                m_chapters_visual_states[GUI.SelectedSerie] = 
                    new ListBoxVisualState(chaptersListBox);

            GUI.UpdateButtons();
        }

        private void seriesSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance.SeriesFilter = seriesSearchTextBox.Text;
            GUI.UpdateSeries();
        }

        private void mangaRootDirTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance.MangaSettings.SetMangaRootDir(mangaRootDirTextBox.Text);

            if (!Settings.Instance.MangaSettings.IsMangaRootDirValid)
                mangaRootDirTextBox.BackColor = BAD_DIR;
            else
                mangaRootDirTextBox.BackColor = SystemColors.Window;
        }

        private void chaptersListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Commands.DownloadPagesForSelectedChapters();

            if ((e.KeyCode == Keys.A) && (e.Control))
                chaptersListBox.SelectAll();
        }

        private void seriesListBox_VerticalScroll(object a_sender, bool a_tracking)
        {
            if (GUI.SelectedServer != null)
                m_series_visual_states[GUI.SelectedServer] = 
                    new ListBoxVisualState(seriesListBox);
        }

        private void chaptersListBox_VerticalScroll(object a_sender, bool a_tracking)
        {
            if (GUI.SelectedSerie != null)
                m_chapters_visual_states[GUI.SelectedSerie] = 
                    new ListBoxVisualState(chaptersListBox);
        }

        private void cbzCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.MangaSettings.UseCBZ = cbzCheckBox.Checked;
        }

        private void versionLinkLabel_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.HomePage);
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            GUI.Refresh();
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            logRichTextBox.Clear();
        }

        private void serversListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ((sender as ListBox).Items[e.Index] as ListItem).DrawItem(e);
        }

        private void seriesListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ((sender as ListBox).Items[e.Index] as ListItem).DrawItem(e);

        }

        private void chaptersListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ((sender as ListBox).Items[e.Index] as ListItem).DrawItem(e);
        }

        private void MangaCrawlerForm_Shown(object sender, EventArgs e)
        {
            seriesSplitter.SplitPosition = Settings.Instance.SeriesSplitterDistance;
            bookmarksSplitter.SplitPosition = Settings.Instance.BookmarksSplitterDistance;
            m_resizing = false;

            if (Environment.GetCommandLineArgs().Contains(Autostart.MINIMIZE_ARGUMENT))
                GUI.MinimizeToTray(true);
        }

        private void pageNamingStrategyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pageNamingStrategyComboBox.SelectedIndex == 0)
                Settings.Instance.MangaSettings.PageNamingStrategy = PageNamingStrategy.DoNotChange;
            else if (pageNamingStrategyComboBox.SelectedIndex == 1)
                Settings.Instance.MangaSettings.PageNamingStrategy = PageNamingStrategy.PrefixToPreserverOrder;
            else if (pageNamingStrategyComboBox.SelectedIndex == 2)
                Settings.Instance.MangaSettings.PageNamingStrategy = PageNamingStrategy.IndexToPreserveOrder;
            else if (pageNamingStrategyComboBox.SelectedIndex == 3)
                Settings.Instance.MangaSettings.PageNamingStrategy = PageNamingStrategy.AlwaysUsePrefix;
            else if (pageNamingStrategyComboBox.SelectedIndex == 4)
                Settings.Instance.MangaSettings.PageNamingStrategy = PageNamingStrategy.AlwaysUseIndex;
            else
                Loggers.GUI.Error("Invalid PageNamingStrategy");
        }

        private void downloadingsGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                Commands.CancelClearSelectedDownloadings();
        }

        private void splitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            SeriesSplitterAdjust();
        }

        private void MangaCrawlerForm_ResizeEnd(object sender, EventArgs e)
        {
            SeriesSplitterAdjust();
            BookmarksSplitterAdjust();
        }

        private void splitterBookmarks_SplitterMoved(object sender, SplitterEventArgs e)
        {
            BookmarksSplitterAdjust();
        }

        private void serieBookmarksListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ((sender as ListBox).Items[e.Index] as ListItem).DrawItem(e);
        }

        private void chapterBookmarksListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ((sender as ListBox).Items[e.Index] as ListItem).DrawItem(e);
        }

        private void chapterBookmarksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GUI.SelectedBookmarkedSerie != null)
                m_chapter_bookmarks_visual_states[GUI.SelectedBookmarkedSerie] =
                    new ListBoxVisualState(bookmarkedchaptersListBox);

            GUI.UpdateButtons();
        }

        private void chapterBookmarksListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Commands.DownloadPagesForSelectedBookmarkedChapters();

            if ((e.KeyCode == Keys.A) && (e.Control))
                bookmarkedchaptersListBox.SelectAll();
        }

        private void chapterBookmarksListBox_VerticalScroll(object a_sender, bool a_tracking)
        {
            if (GUI.SelectedBookmarkedSerie != null)
                m_chapter_bookmarks_visual_states[GUI.SelectedBookmarkedSerie] =
                    new ListBoxVisualState(bookmarkedchaptersListBox);
        }

        private void serieBookmarksListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                Commands.RemoveBookmarkFromBookmarks();
        }

        private void resetCheckDatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUI.LastBookmarkCheck = DateTime.MinValue;
            DownloadManager.Instance.Debug_ResetCheckDate();
        }

        private void addSerieFirsttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_InsertSerie(0, GUI.SelectedServer);
        }

        private void addSerieMiddleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_InsertSerie(
                new Random().Next(1, GUI.SelectedServer.Series.Count - 1), GUI.SelectedServer);
        }

        private void addSerieLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_InsertSerie(GUI.SelectedServer.Series.Count, GUI.SelectedServer);
        }

        private void removeSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_RemoveSerie(GUI.SelectedServer, GUI.SelectedSerie);
        }

        private void addChapterFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_InsertChapter(0, GUI.SelectedSerie);
        }

        private void addChapterMiddleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_InsertChapter(
                new Random().Next(1, GUI.SelectedSerie.Chapters.Count - 1), GUI.SelectedSerie);
        }

        private void addChapterLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_InsertChapter(GUI.SelectedSerie.Chapters.Count, GUI.SelectedSerie);
        }

        private void removeChapterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var chapter in GUI.SelectedChapters)
                DownloadManager.Instance.Debug_RemoveChapter(chapter);
        }

        private void renameSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_RenameSerie(GUI.SelectedSerie);
        }

        private void renameChapterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var chapter in GUI.SelectedChapters)
                DownloadManager.Instance.Debug_RenameChapter(chapter);
        }

        private void changeSerieURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadManager.Instance.Debug_ChangeSerieURL(GUI.SelectedSerie);
        }

        private void changeChapterURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var chapter in GUI.SelectedChapters)
                DownloadManager.Instance.Debug_ChangeChapterURL(chapter);
        }

        private void minimizeOnCloseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.MinimizeOnClose = minimizeOnCloseCheckBox.Checked;
            showBaloonTipsCheckBox.Enabled = minimizeOnCloseCheckBox.Checked;
        }

        private void exitTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUI.Close(true);
        }

        private void MangaCrawlerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = GUI.MinimizeOrClose();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                GUI.MinimizeToTray(false);
        }

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            GUI.MinimizeToTray(false);

            FrontTab = Tabs.Bookmarks;

            var serie = DownloadManager.Instance.Bookmarks.List.FirstOrDefault(b => b.GetNewChapters().Any());

            GUI.SelectBookmarkedSerie(serie);
            GUI.SelectBookmarkedChapter(serie.GetNewChapters().FirstOrDefault());
        }

        private void notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (notifyIcon.Visible && Visible)
                GUI.MinimizeToTray(false);
        }

        private void showBaloonTipsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.ShowBaloonTips = showBaloonTipsCheckBox.Checked;
        }

        private void bookmarksTimer_Tick(object sender, EventArgs e)
        {
            GUI.RefreshBookmarks();
        }

        private void forceBookmarksCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Task(() => 
            {
                Thread.Sleep(5000);

                Invoke(new Action(() =>
                {
                    SystemSounds.Exclamation.Play();
                    Commands.CheckNowBookmarks();
                }));
            }).Start();
        }

        private void playSoundWhenDownloadedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.PlaySoundWhenDownloaded = playSoundWhenDownloadedCheckBox.Checked;
        }

        private void downloadingsGridView_MouseDown(object sender, MouseEventArgs e)
        {
            var hti = downloadingsGridView.HitTest(e.X, e.Y);

            if (e.Button == MouseButtons.Right)
            {
                if (hti.Type == DataGridViewHitTestType.Cell)
                {
                    if (!downloadingsGridView.Rows[hti.RowIndex].Selected)
                    {
                        downloadingsGridView.ClearSelection();
                        downloadingsGridView.Rows[hti.RowIndex].Selected = true;
                    }
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                if (hti.Type == DataGridViewHitTestType.None)
                {
                    if (!Control.ModifierKeys.HasFlag(Keys.Shift) &&
                        !Control.ModifierKeys.HasFlag(Keys.Control) &&
                        !Control.ModifierKeys.HasFlag(Keys.Alt))
                    {
                        downloadingsGridView.ClearSelection();
                    }
                }
            }
        }

        private void loadAllFromCatalogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int servers = 0;
            int series = 0;
            int chapters = 0;
            int pages = 0;
            DownloadManager.Instance.Debug_LoadAllFromCatalog(ref servers, ref series, ref chapters, ref pages);

            Loggers.GUI.InfoFormat("servers: {0}, series: {1}, chapters: {2}, pages: {3}", servers, series, chapters, pages);
        }

        private void checkNowForServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.UpdateNowForSelectedServer();
        }

        private void visitPageForServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedServer();
        }

        private void openFolderForServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedServer();
        }

        private void checkNowForSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.UpdateNowForSelectedSerie();
        }

        private void bookmarkSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.BookmarkSelectedSerie();
        }

        private void openFolderForSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedSerie();
        }

        private void visitPageForSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedSerie();
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedChapters();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedChapters();
        }

        private void visitPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedChapters();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedChapters();
        }

        private void downloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedDownloadings();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.CancelClearSelectedDownloadings();
        }

        private void showInSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUI.ShowInSeriesFromDownloadings();
        }

        private void openFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedDownloadings();
        }

        private void visitPageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedDownloadings();
        }

        private void viewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedDownloadings();
        }

        private void openFolderForSeriestoolStripButton_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedServer();
        }

        private void visitPageForSeriesToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedServer();
        }

        private void checkNowForSeriesToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.UpdateNowForSelectedServer();
        }

        private void bookmarkSerietoolStripButton_Click(object sender, EventArgs e)
        {
            Commands.BookmarkSelectedSerie();
        }

        private void openFolderForSerieToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedSerie();
        }

        private void visitPageForSerietoolStripButton_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedSerie();
        }

        private void checkNowForSelectedSerieToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.UpdateNowForSelectedSerie();
        }

        private void downloadPagesForChapersToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedChapters();
        }

        private void openFolderForChaptersToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedChapters();
        }

        private void visitPageForChaptersToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedChapters();
        }

        private void viewPagesForSelectedChapterToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedChapters();
        }

        private void downloadDownloadingToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedDownloadings();
        }

        private void showInSeriesToolStripButton_Click(object sender, EventArgs e)
        {
            GUI.ShowInSeriesFromDownloadings();
        }

        private void openFolderForDownloadingsToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedDownloadings();
        }

        private void downloadingsTabPage_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedDownloadings();
        }

        private void viewDownloadingToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedDownloadings();
        }

        private void openFolderBookmarkedSerieToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedBookmarkSerie();
        }

        private void visitPageBookmarkedSerieToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedBookmarkedSerie();
        }

        private void removeBookmarkToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.RemoveBookmarkFromBookmarks();
        }

        private void checkNowBookmarkedSerieToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.CheckNowBookmarks();
        }

        private void downloadBookmarkedChapterToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedBookmarkedChapters();
        }

        private void visitPageForBookmarkedChaptertoolStripButton_Click(object sender, EventArgs e)
        {
            Commands.VisitBookmarkedPagesForSelectedChapters();
        }

        private void openFolderForBookmarkedChapterToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedBookmarkedChapters();
        }

        private void viewBookmarkedChapterToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedBookmarkedChapters();
        }

        private void visitPageForDownloadingsToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedDownloadings();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedServer();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedServer();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Commands.UpdateNowForSelectedServer();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedSerie();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedSerie();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            Commands.UpdateNowForSelectedSerie();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Commands.BookmarkSelectedSerie();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedChapters();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedChapters();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedChapters();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedChapters();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            Commands.CancelClearSelectedDownloadings();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedDownloadings();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedDownloadings();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedDownloadings();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedDownloadings();
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            GUI.ShowInSeriesFromDownloadings();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            Commands.RemoveBookmarkFromBookmarks();
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            Commands.CheckNowBookmarks();
        }

        private void checkNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.CheckNowBookmarks();
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedBookmarkSerie();
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedBookmarkedSerie();
        }

        private void toolStripButton25_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedBookmarkedChapters();
        }

        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            Commands.VisitPagesForSelectedBookmarkedChapters();
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedBookmarkedChapters();
        }

        private void toolStripButton22_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedBookmarkedChapters();
        }

        private void openFolderToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedBookmarkedChapters();
        }

        private void viisitPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.VisitPagesForSelectedBookmarkedChapters();
        }

        private void downloadToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedBookmarkedChapters();
        }

        private void readMangaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedBookmarkedChapters();
        }

        private void serversListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Commands.DownloadSeriesForSelectedServer();
        }

        private void chaptersListBox_DoubleClick(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedChapters();
        }

        private void serieBookmarksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Commands.DownloadSeriesForSelectedBookmarkSerie();
        }

        private void chapterBookmarksListBox_DoubleClick(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedBookmarkedChapters();
        }

        private void downloadingsGridView_SelectionChanged(object sender, EventArgs e)
        {
            GUI.UpdateButtons();
        }

        private void showInSeriesForSelectedDownloadingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUI.ShowInSeriesFromDownloadings();
        }

        private void deleteForSelectedDownloadingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.CancelClearSelectedDownloadings();
        }

        private void visitPageForSelectedBookmarkedSerieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.VisitPageForSelectedBookmarkedSerie();
        }

        private void openFolderForSelectedBookmarkedChaptersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.OpenFolderForSelectedBookmarkedChapters();
        }

        private void visitPageForSelectedBookmarkedChaptersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.VisitPagesForSelectedBookmarkedChapters();
        }

        private void downloadForSelectedBookmarkedChaptersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.DownloadPagesForSelectedBookmarkedChapters();
        }

        private void readMangaForSelectedBookmarkedChaptersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.ReadMangaForSelectedBookmarkedChapters();
        }

        private void seriesTabLabel_Click(object sender, EventArgs e)
        {
            FrontTab = Tabs.Series;
        }

        private void downloadingsTabLabel_Click(object sender, EventArgs e)
        {
            FrontTab = Tabs.Downloadings;
        }

        private void bookmarksTabLabel_Click(object sender, EventArgs e)
        {
            FrontTab = Tabs.Bookmarks;
        }

        private void optionsTabLabel_Click(object sender, EventArgs e)
        {
            FrontTab = Tabs.Options;
        }

        private void logTabLabel_Click(object sender, EventArgs e)
        {
            FrontTab = Tabs.Log;
        }

        private void tabLabel_MouseEnter(object sender, EventArgs e)
        {
            ActivateTabLabel(sender as Label);
        }

        private void tabLabel_MouseLeave(object sender, EventArgs e)
        {
            if (sender != GetTabLabel(FrontTab))
                DeactivateTabLabel(sender as Label);
        }

        private void MangaCrawlerForm_Resize(object sender, EventArgs e)
        {
            SeriesSplitterAdjust();
            BookmarksSplitterAdjust();
        }

        private void seriesSplitter_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = seriesSplitter.ClientRectangle;
            r = new Rectangle(r.Left, r.Top, r.Width, chaptersToolStrip.Height);
            e.Graphics.FillRectangle(new SolidBrush(splitPanel.BackColor), r);
        }

        private void bookmarksSplitter_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = bookmarksSplitter.ClientRectangle;
            r = new Rectangle(r.Left, r.Top, r.Width, bookmarkedChaptersToolStrip.Height);
            e.Graphics.FillRectangle(new SolidBrush(splitPanel.BackColor), r);
        }

        private void autostartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autostartCheckBox.Checked)
                Autostart.Enable();
            else
                Autostart.Disable();
        }

        private void ignoreNewForSelectedBookmarkedChaptersToolStripButton_Click(object sender, EventArgs e)
        {
            Commands.IgnoreForSelectedBookmarkedChapters();
        }

        private void ignoreNewForSelectedBookmarkedChaptersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.IgnoreForSelectedBookmarkedChapters();
        }
    }
}