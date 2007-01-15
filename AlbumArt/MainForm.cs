/*  This file is part of Album Art Downloader.
 *  CoverDownloader is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  CoverDownloader is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CoverDownloader; if not, write to the Free Software             
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA  */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using System.Windows.Forms.Layout;
using System.IO;

namespace AlbumArtDownloader
{
    public delegate void DelegateUpdateStatus(string status, Task t, bool savedone, Thread from);
    public delegate void DelegateUpdateProgress(int prog);
    public delegate void DelegateUpdateTaskStatus(Task t, Script s, string stat);
    public delegate void DelegateAddThumb(AlbumArtDownloader.ArtDownloader.ThumbRes res);
    public delegate void DelegateRemoveThumbs();
    public delegate void DelegateAddBrowse(string[,] items);
    public delegate void DelegateTaskDone(Task t);
    public delegate void DelegateThreadsEnded();
    public delegate void DelegatePreviewGot(Image i, AlbumArtDownloader.ArtDownloader.ThumbRes thumb);
    public delegate void DelegateDoHTML(bool b, string d);

    public partial class MainForm : Form
    {


        public ArtDownloader a;
        bool waitingtoclose;
        public DelegateUpdateStatus statupdate;
        public DelegateUpdateProgress produpdate;
        public DelegateAddThumb thumbupdate;
        public DelegateRemoveThumbs removethumbs;
        public DelegatePreviewGot previewgot;
        public DelegateUpdateTaskStatus taskupdate;
        public DelegateDoHTML dohtml;
        public DelegateAddBrowse addbrowse;
        public DelegateTaskDone taskdone;
        bool stopped;
        public DelegateThreadsEnded threadended;
        private SaveWithPreset savedlg;

        public Size ThumbNailSize = new Size((int)Properties.Settings.Default.ThumbnailWidth, (int)Properties.Settings.Default.ThumbnailHeight);

        private ATL.AudioReaders.AudioFileReader ATLReader;

        string[] args;
        ListViewItem preview;
        ListViewGroup existing;
        ListViewGroup folder_existing;
        private List<BrowserItem> itemstoadd, itemsadded;

        System.Collections.Generic.List<BrowserItem> albarts;
        int counter;
        string ext;
        int processed = 0;
        int trackcount = 0;

        public MainForm(string[] argsn)
        {
            a = new ArtDownloader(this);
            a.CompileScripts();
            aborting = false;
            stopped = false;
            waitingtoclose = false;
            statupdate = new DelegateUpdateStatus(this.UpdateStatus);
            produpdate = new DelegateUpdateProgress(this.UpdateProgess);
            thumbupdate = new DelegateAddThumb(this.callbackAddThumb);
            removethumbs = new DelegateRemoveThumbs(this.RemoveThumbs);
            previewgot = new DelegatePreviewGot(this.PreviewGot);
            threadended = new DelegateThreadsEnded(this.ThreadsEnded);
            taskupdate = new DelegateUpdateTaskStatus(this.TaskUpdate);
            taskdone = new DelegateTaskDone(this.TaskDone);
            dohtml = new DelegateDoHTML(this.DoHTML);
            itemstoadd = new List<BrowserItem>();
            itemsadded = new List<BrowserItem>();
            InitializeComponent();
            lvwColumnSorterCOM = new ListViewColumnSorter();
            lvwColumnSorterPath = new ListViewColumnSorter();
            megaListBrowserCOM.ListViewItemSorter = lvwColumnSorterCOM;
            megaListBrowserPath.ListViewItemSorter = lvwColumnSorterPath;
            args = argsn;

        }
        public void DoHTML(bool b, string d)
        {
            new DebugForm(b, d).Show();
        }
        public unsafe struct COPYDATASTRUCT
        {
            public int dwData;
            public int cbData;
            public int lpData;
        }
        protected override void WndProc(ref Message m)
        {

            if (m.Msg == 74)
            {
                unsafe
                {
                    COPYDATASTRUCT data = *((COPYDATASTRUCT*)m.LParam);
                    if (data.dwData == 7)
                    {
                        string s = new string((char*)(data.lpData), 0, data.cbData / sizeof(char));
                        ProcessArgs(s.Split('\0'), false);
                    }
                }
            }
            base.WndProc(ref m);
        }
        public void TaskUpdate(Task t, Script s, string stat)
        {

            t.listviewitem.SubItems[s.listcol.Index].Text = stat;

        }
        public void TaskDone(Task t)
        {
            lock (t.results)
            {
                if (t.results.Count == 0)
                    t.listviewitem.ImageIndex = 2;
                else
                    t.listviewitem.ImageIndex = 1;
            }
        }
        public void RemoveThumbs()
        {
            megaListTiles.Items.Clear();
            imageListTile.Images.Clear();
            if (Properties.Settings.Default.PreviewSavedArt)
            {
                ArtDownloader.ThumbRes r = new ArtDownloader.ThumbRes();
                Int32 width = new Int32();
                Int32 height = new Int32();
                r.Thumb = UpdateThumb(ref width, ref height);
                if (r.Thumb != null)
                {
                    r.Width = width;
                    r.Height = height;
                    r.Name = "---Existing---" + testBoxFileSave.Text;
                    r.ScriptOwner = null;
                    preview = AddThumb(r);
                }
                else preview = null;

            }
            else preview = null;

            if (Properties.Settings.Default.ShowFolderPictures)
            {

                string albumPath = System.IO.Path.GetDirectoryName(selectedtask.FileSave);

                DirectoryInfo dirinfo = new DirectoryInfo(albumPath);

                foreach (FileInfo fileInfo in dirinfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (fileInfo.FullName != selectedtask.FileSave)
                    {
                        if (fileInfo.Extension == ".bmp" || fileInfo.Extension == ".jpg" || fileInfo.Extension == ".jpeg" || fileInfo.Extension == ".png" || fileInfo.Extension == ".gif")
                        {
                            ArtDownloader.ThumbRes r = new ArtDownloader.ThumbRes();
                            Int32 width = new Int32();
                            Int32 height = new Int32();
                            r.Thumb = UpdateThumbFolder(fileInfo.FullName, ref width, ref height);
                            if (r.Thumb != null)
                            {
                                r.Width = width;
                                r.Height = height;
                                r.Name = "---LocalFolder---" + fileInfo.FullName;
                                r.ScriptOwner = null;
                                preview = AddThumb(r);
                            }
                            else preview = null;
                        }
                    }
                }

            }
            else preview = null;

        }

        private void getFolderImages(string FolderName)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(FolderName);

            foreach (FileInfo fileInfo in dirinfo.GetFiles("", SearchOption.AllDirectories))
            {

            }
                   
        }

        bool aborting;
        public object CoolInvoke(Delegate method, params object[] args)
        {
            if (aborting || stopped) return null;
            else
                return this.Invoke(method, args);
        }
        public object CoolBeginInvoke(Delegate method, params object[] args)
        {
            if (aborting || stopped) return null;
            else
                return this.BeginInvoke(method, args);

        }
        public object CoolNoThrowBeginInvoke(Delegate method, params object[] args)
        {
            if (!aborting)
                return this.BeginInvoke(method, args);
            else return null;
        }
        public Bitmap UpdateThumb(ref Int32 width, ref Int32 height)
        {
            string s = selectedtask.FileSave;
            if (s.Length == 0 || !Properties.Settings.Default.PreviewSavedArt)
                return null;
            try
            {
                Bitmap b = new Bitmap(testBoxFileSave.Text);
                if (b != null)
                {
                    Bitmap b2 = ResizeBitmap(b, (int)Properties.Settings.Default.ThumbnailWidth, (int)Properties.Settings.Default.ThumbnailHeight);
                    width = b.Width;
                    height = b.Height;
                    b.Dispose();
                    return b2;

                }
            }
            catch
            {
            }
            return null;
        }

        public Bitmap UpdateThumbFolder(string FilePath, ref Int32 width, ref Int32 height)
        {
            string s = selectedtask.FileSave;
            if (s.Length == 0 || !Properties.Settings.Default.PreviewSavedArt)
                return null;
            try
            {
                Bitmap b = new Bitmap(FilePath);
                if (b != null)
                {
                    Bitmap b2 = ResizeBitmap(b, (int)Properties.Settings.Default.ThumbnailWidth, (int)Properties.Settings.Default.ThumbnailHeight);
                    width = b.Width;
                    height = b.Height;
                    b.Dispose();
                    return b2;

                }
            }
            catch
            {
            }
            return null;
        }
             
        public void UpdateSize(bool refresh)
        {
            Size s = new Size((int)Properties.Settings.Default.ThumbnailWidth, (int)Properties.Settings.Default.ThumbnailHeight);
            if (imageListTile.ImageSize != s)
            {
                imageListTile.ImageSize = s;
                imageListTile.Images.Clear();
                if (refresh)
                {
                    foreach (ListViewItem thumb in megaListTiles.Items)
                    {
                        imageListTile.Images.Add(((ArtDownloader.ThumbRes)thumb.Tag).Thumb);
                    }
                    megaListTiles.Refresh();
                }
            }
        }
        public void callbackAddThumb(ArtDownloader.ThumbRes r)
        {
            AddThumb(r);
        }
        public ListViewItem AddThumb(AlbumArtDownloader.ArtDownloader.ThumbRes res)
        {
            int i = imageListTile.Images.Count;

            if (Properties.Settings.Default.ShowSizeOverlay)
            {
                if (res.Width > 0 && res.Height > 0)
                {
                    string caption = System.String.Format("{0} x {1}", res.Width, res.Height);

                    Graphics g = Graphics.FromImage(res.Thumb);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    Font f = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
                    g.DrawString(caption, f, Brushes.White, 1, 1);
                    g.DrawString(caption, f, Brushes.Black, 0, 0);
                    f.Dispose();
                    g.Dispose();
                }
            }

            imageListTile.Images.Add(res.Thumb);
            
            ListViewItem x;
            
            if (res.ScriptOwner != null)
            {
                megaListTiles.Items.Add(x = new ListViewItem(res.Name, i, res.ScriptOwner.group));
            }
            else if (res.Name.IndexOf("---Existing---") >= 0)
            {
                res.Name = res.Name.Replace("---Existing---", "");

                FileInfo fileInfo = new FileInfo(res.Name);

                megaListTiles.Items.Add(x = new ListViewItem(fileInfo.Name, i, existing));
            }
            else if (res.Name.IndexOf("---LocalFolder---") >= 0)
            {
                res.Name = res.Name.Replace("---LocalFolder---", "");

                FileInfo fileInfo = new FileInfo(res.Name);

                megaListTiles.Items.Add(x = new ListViewItem(fileInfo.Name, i, folder_existing));
            }
            else
            {
                megaListTiles.Items.Add(x = new ListViewItem());
            }

            if (res.Exact && Properties.Settings.Default.ExactMatchBold)
                x.Font = new Font(x.Font, FontStyle.Bold);

            x.Tag = res;

            megaListTiles.Update();
            return x;
        }
        public ListViewGroup AddGroup(AlbumArtDownloader.Script s)
        {
            ListViewGroup x = new ListViewGroup(s.Name);
            megaListTiles.Groups.Add(x);
            return x;
        }
        public void ThumbClicked(AlbumArtDownloader.ArtDownloader.ThumbRes res, bool usedefsave)
        {
            if (testBoxFileSave.Text.Length == 0 || !usedefsave)
            {
                if (getFileName())
                {
                    if (res.ScriptOwner != null)
                    {
                        a.SaveArt(res, testBoxFileSave.Text);
                    }
                    else
                    {
                        if (res.Name != testBoxFileSave.Text)
                            File.Copy(res.Name, testBoxFileSave.Text, true);
                    }
                }
            }
            else
            {
                if (res.ScriptOwner != null)
                {
                    a.SaveArt(res, testBoxFileSave.Text);
                }
                else
                {
                    if (res.Name != testBoxFileSave.Text)
                        File.Copy(res.Name, testBoxFileSave.Text, true);
                }
            }
        }
        public void UpdateProgess(int prog)
        {
            if (prog == -1) MainProgress.Visible = false;
            else
            {
                System.Diagnostics.Debug.WriteLine(prog.ToString() + "%");
                MainProgress.Visible = true;
                MainProgress.Minimum = 0;
                MainProgress.Maximum = 100;
                MainProgress.Value = prog;
            }
        }
        public void PreviewGot(Image i, AlbumArtDownloader.ArtDownloader.ThumbRes s)
        {
            UpdateStatus("Image Retrieved", null, false, null);
            ShowPreview(i, s);
        }
        public void ThreadsEnded()
        {
            if (waitingtoclose && !a.ThreadsActive()) this.Close();
        }
        public void UpdateStatus(string status, Task t, bool savedone, Thread from)
        {
            MainStatus.Text = status;
            if (savedone)
            {
                MainProgress.Visible = false;
                if (t != null)
                {
                    PictureChosen(t);
                }
                /*       if (tiles.Count != 0)
                       {
                           if (preview != null)
                           {
                               Int32 width = new Int32();
                               Int32 height = new Int32();
                               Bitmap b = UpdateThumb(ref width, ref height);
                               //preview.SetThumb(b,width,height);
                           }
                       }*/

            }
        }

        private void PictureChosen(Task t)
        {
            if (Properties.Settings.Default.CloseAfterSaving)
            {
                a.RemoveTask(t);
                if (listViewQueue.Items.Count == 0)
                    this.Close();
            }
        }

        private void buttonAddTask_Click(object sender, EventArgs e)
        {
            String art = textBoxArtist.Text;
            String alb = textBoxAlbum.Text;
            string fz = testBoxFileSave.Text;
            textBoxArtist.Focus();
            Task t;
            a.AddTask(t = new Task(a, art, alb, fz, (Control.ModifierKeys == Keys.Control)));
            SwitchToTask(t);
        }
       
        public void ScriptsLoaded(List<Script> scripts)
        {
            string[] strings;
            lock (scripts)
            {
                strings = new string[scripts.Count];
                int i = 0;
                foreach (Script s in scripts)
                {
                    strings[i++] = "0";
                }
            }
            foreach (ListViewItem i in listViewQueue.Items)
            {
                i.SubItems.AddRange(strings);
            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BrowserWorkerCOM.CancelAsync();
            BrowserWorkerPath.CancelAsync();

            if (a.ThreadsActive() || BrowserWorkerCOM.IsBusy || BrowserWorkerPath.IsBusy)
            {
                e.Cancel = true;
                a.Abort();
                aborting = true;
                MainStatus.Text = "Waiting for threads...";
                waitingtoclose = true;
                timerClose.Enabled = true;
                this.Enabled = false;
            }
            else
            {
                Properties.Settings.Default.MainWindowState = WindowState;
                if (WindowState != FormWindowState.Normal)
                    WindowState = FormWindowState.Normal;
                Properties.Settings.Default.MainPosX = DesktopBounds.X;
                Properties.Settings.Default.MainPosY = DesktopBounds.Y;
                Properties.Settings.Default.MainPosW = DesktopBounds.Width;
                Properties.Settings.Default.MainPosH = DesktopBounds.Height;
                Properties.Settings.Default.ShowQueue = queueToolStripMenuItem.Checked;
                Properties.Settings.Default.ShowBrowser = browserToolStripMenuItem.Checked;
                if (!Properties.Settings.Default.ShowQueue)
                {
                    queueToolStripMenuItem.Checked = true;

                }
                if (!Properties.Settings.Default.ShowBrowser)
                {
                    browserToolStripMenuItem.Checked = true;

                }

                Properties.Settings.Default.SplitPos = splitContainer4.SplitterDistance;
                Properties.Settings.Default.BrowseSplitPos = splitContainer3.SplitterDistance;
                Properties.Settings.Default.QueueCols = SaveListViewSizeToString(listViewQueue);
                Properties.Settings.Default.BrowserColsCOM = SaveListViewSizeToString(megaListBrowserCOM);
                Properties.Settings.Default.BrowserColsPath = SaveListViewSizeToString(megaListBrowserPath);
                Properties.Settings.Default.Save();
            }
        }

        internal void SizeListViewFromString(string str, ListView list)
        {
            string[] colwidths = str.Split(';');
            int i = 0;
            foreach (string s in colwidths)
            {
                int width;
                if (Int32.TryParse(s, out width))
                {
                    if (i < list.Columns.Count)
                        list.Columns[i++].Width = width;
                }
            }
        }
        internal string SaveListViewSizeToString(ListView list)
        {
            string[] colwidths = new string[list.Columns.Count];
            int i = 0;
            foreach (ColumnHeader c in list.Columns)
            {
                colwidths[i++] = c.Width.ToString();
            }
            return string.Join(";", colwidths);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            bool forceminimise = false;
            a.TaskAdded += new TaskEventHandler(a_TaskAdded);
            a.TaskRemoved += new TaskEventHandler(a_TaskRemoved);
            a.ThreadCountChanged += new ThreadEventHandler(a_ThreadCountChanged);
            savedlg = new SaveWithPreset();
            savedlg.DefaultExt = "jpg";
            megaListTiles.SetExStyle();
            megaListBrowserCOM.SetExStyleNoBorderSelect();
            UpdateSize(false);
            megaListTiles.Groups.Add(existing = new ListViewGroup("Existing"));
            megaListTiles.Groups.Add(folder_existing = new ListViewGroup("Pictures in folder"));

            a.LoadScripts();
            
            SizeListViewFromString(Properties.Settings.Default.QueueCols, listViewQueue);
            SizeListViewFromString(Properties.Settings.Default.BrowserColsCOM, megaListBrowserCOM);
            SizeListViewFromString(Properties.Settings.Default.BrowserColsPath, megaListBrowserPath);
            a.SetThreads();
            forceminimise = ProcessArgs(args, true);
            if (Properties.Settings.Default.MainPosX != 0 || Properties.Settings.Default.MainPosY != 0 || Properties.Settings.Default.MainPosW != 0 || Properties.Settings.Default.MainPosH != 0)
            {
                Rectangle r = new Rectangle();
                r.X = Properties.Settings.Default.MainPosX;
                r.Y = Properties.Settings.Default.MainPosY;
                r.Width = Properties.Settings.Default.MainPosW;
                r.Height = Properties.Settings.Default.MainPosH;
                DesktopBounds = r;
                splitContainer4.SplitterDistance = Properties.Settings.Default.SplitPos;
                splitContainer3.SplitterDistance = Properties.Settings.Default.BrowseSplitPos;
            }
            if (!forceminimise && WindowState != Properties.Settings.Default.MainWindowState)
                WindowState = Properties.Settings.Default.MainWindowState;
            queueToolStripMenuItem.Checked = Properties.Settings.Default.ShowQueue;
            browserToolStripMenuItem.Checked = Properties.Settings.Default.ShowBrowser;
            UpdateBottomPanel();
            megaListTiles.Focus();
            if (Properties.Settings.Default.FirstRun)
            {
                (new LicenseForm()).ShowDialog();
                (new SettingsForm(this)).ShowDialog();
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.Save();
            }
        }

        private bool ProcessArgs(string[] theargs, bool fromself)
        {
            bool forceminimise = false;
            if (theargs.Length > 0)
            {
                if (theargs.Length != 3)
                {
                    MessageBox.Show("Usage: albumart.exe \"<artist>\" \"<album>\" \"<path to save image>\"\r\nExample: albumart.exe \"Iron Maiden\" \"Powerslave\" \"D:\\Music\\Iron Maiden\\Powerslave\\folder.jpg\"");
                    if (fromself)
                        Close();
                }
                else
                {
                    /*if (Properties.Settings.Default.CmdLineHideTillDone && fromself)
                    {
                        WindowState = FormWindowState.Minimized;
                        forceminimise = true;
                    }*/
                    if (fromself)
                    {
                        textBoxArtist.Text = theargs[0];
                        textBoxAlbum.Text = theargs[1];
                        testBoxFileSave.Text = theargs[2];
                        this.buttonAddTask_Click(this, new EventArgs());
                    }
                    else
                    {
                        a.AddTask(new Task(a, theargs[0], theargs[1], theargs[2], false));
                    }
                }
            }
            return forceminimise;
        }



        void a_ThreadCountChanged(object o, ThreadEventArgs e)
        {
            if (waitingtoclose && e.count == 0)
                this.Close();
        }

        void a_TaskRemoved(object o, TaskEventArgs e)
        {
            listViewQueue.Items.Remove(e.task.listviewitem);
            if (!BlockUpdates)
            {
                if (e.task == selectedtask)
                {
                    RemoveThumbs();
                    if (listViewQueue.Items.Count > 0)
                    {
                        SwitchToTask(listViewQueue.Items[0].Tag as Task);
                    }
                }
            }

        }

        void a_TaskAdded(object o, TaskEventArgs e)
        {
            ListViewItem i = new ListViewItem(e.task.Artist);
            i.SubItems.Add(e.task.Album);
            lock (a.scripts)
            {
                foreach (Script ss in a.scripts)
                {
                    i.SubItems.Add("0");
                }
            }

            listViewQueue.Items.Add(i);
            i.Tag = e.task;
            i.ImageIndex = 0;
            lock (e.task)
            {
                e.task.listviewitem = i;
            }
            if (listViewQueue.Items.Count == 1)
                SwitchToTask(e.task);
        }

        private bool getFileName()
        {
            /*        System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                    sfd.AddExtension = true;
                    sfd.FileName = Properties.Settings.Default.SaveFileName;
                    sfd.Filter = "JPEG Files|*.jpg;*.jpeg";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        filesave.Text = sfd.FileName;
                        return true;
                    }
                    else return false;*/
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                testBoxFileSave.Text = savedlg.FileName;
                return true;
            }
            return false;


        }


        private void buttonSaveToBrowse_Click(object sender, EventArgs e)
        {
            getFileName();
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            if (waitingtoclose && !a.ThreadsActive() && !BrowserWorkerCOM.IsBusy && !BrowserWorkerPath.IsBusy) this.Close();
        }


        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm d = new SettingsForm(this);
            d.ShowDialog(this);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DoActivateClickThingy(false);
        }

        private void DoActivateClickThingy(bool customfname)
        {
            if (megaListTiles.SelectedItems.Count != 0)
            {
                AlbumArtDownloader.ArtDownloader.ThumbRes t = megaListTiles.SelectedItems[0].Tag as AlbumArtDownloader.ArtDownloader.ThumbRes;
                if (t != null)
                    ThumbClicked(t, customfname);
                PictureChosen(t.OwnerTask);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OnPreviewClick();
        }

        private void OnPreviewClick()
        {
            if (megaListTiles.SelectedItems.Count != 0)
            {
                AlbumArtDownloader.ArtDownloader.ThumbRes res = (ArtDownloader.ThumbRes)megaListTiles.SelectedItems[0].Tag;
                if (res.ScriptOwner == null)
                {
                    ShowPreview(new Bitmap(res.Name), res);
                }
                else
                {
                    a.SaveArt(res, null);
                }
            }
        }

        private void ShowPreview(Image image, AlbumArtDownloader.ArtDownloader.ThumbRes title)
        {
            PreviewForm p = new PreviewForm(this, image, title);
            p.Show(this);
        }

        private void megaListTiles_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                ListViewItem item = megaListTiles.HitTest(e.Location).Item;
                if (item != null)
                {
                    megaListTiles.SelectedItems.Clear();
                    item.Selected = true;
                    OnPreviewClick();
                }
            }
        }

        private void megaListTiles_ItemActivate(object sender, EventArgs e)
        {
            DoActivateClickThingy(true);
        }

        private void checkBoxQueue_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBottomPanel();
        }

        private void UpdateBottomPanel()
        {
            splitContainer4.Panel2Collapsed = !queueToolStripMenuItem.Checked;
            splitContainer3.Panel2Collapsed = !browserToolStripMenuItem.Checked;
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewQueue.SelectedItems.Count == 1)
            {
                SwitchToTask(listViewQueue.SelectedItems[0].Tag as Task);
            }
        }
        Task selectedtask;
        private void SwitchToTask(Task task)
        {
            if (selectedtask != null)
            {
                selectedtask.listviewitem.Font = new Font(selectedtask.listviewitem.Font, FontStyle.Regular);
            }
            selectedtask = task;
            selectedtask.listviewitem.Font = new Font(selectedtask.listviewitem.Font, FontStyle.Bold);
            textBoxArtist.Text = task.Artist;
            textBoxAlbum.Text = task.Album;
            testBoxFileSave.Text = task.FileSave;
            RemoveThumbs();
            a.SelectedTask = task;
        }

        private void cancelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveSelectedQueueItems();
        }

        internal ColumnHeader AddCol(Script script)
        {
            return listViewQueue.Columns.Add(script.Name, 50);
        }

        private void queue_ItemActivate(object sender, EventArgs e)
        {
            SwitchToTask(listViewQueue.SelectedItems[0].Tag as Task);
        }

        internal void SetThreads()
        {
            a.SetThreads();
        }

        private void album_Enter(object sender, EventArgs e)
        {
            textBoxAlbum.SelectAll();
        }

        private void artist_Enter(object sender, EventArgs e)
        {
            textBoxArtist.SelectAll();
        }

        private void listViewQueue_ItemActivate(object sender, EventArgs e)
        {
            SwitchToTask(listViewQueue.SelectedItems[0].Tag as Task);
        }

        private void checkBoxBrowser_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBottomPanel();
        }

        struct BrowserItem
        {
            public string artist;
            public string album;
            public string path;
            public bool hasart;
        }

        private void BrowserWorkerCOM_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Foobar2000Helper.ApplicationHelper07 helper;
                Foobar2000.Application07 application = null;
                helper = new Foobar2000Helper.ApplicationHelper07Class();
                if (helper != null)
                {
                    application = (Foobar2000.Application07)helper.Server;
                }
                if (application == null)
                    application = new Foobar2000.Application07Class();
                Foobar2000.Tracks07 tracks = application.MediaLibrary.GetTracks("1");
                int trackcount = tracks.Count;
                System.Collections.Generic.List<BrowserItem> albarts = new List<BrowserItem>();
                int counter = 0;
                string ext = "\\" + e.Argument as string;
                int processed = 0;
                foreach (Foobar2000.Track07 track in tracks)
                {
                    if (BrowserWorkerCOM.CancellationPending) break;
                    BrowserItem sp;
                    sp.artist = track.FormatTitle("%artist%");
                    sp.album = track.FormatTitle("%album%");
                    sp.path = System.IO.Path.GetDirectoryName(track.FormatTitle("%path%").ToLowerInvariant()) + ext;
                    sp.hasart = false;
                    ++processed;
                    if (!albarts.Contains(sp))
                    {
                        albarts.Add(sp);
                        bool b = System.IO.File.Exists(sp.path);
                        sp.hasart = b;
                        lock (itemstoadd)
                        {
                            itemstoadd.Add(sp);
                        }

                        if (counter++ == 10)
                        {
                            BrowserWorkerCOM.ReportProgress(processed * 100 / trackcount);
                            counter = 0;
                        }
                    }

                }
                if (!e.Cancel)
                    BrowserWorkerCOM.ReportProgress(100);
                helper = null;
                application = null;

            }
            catch (System.Runtime.InteropServices.COMException c)
            {
                MessageBox.Show(c.Message,"Error accessing Foobar2000");
            }
        }

        private void getFileCount(string Path)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(Path);

            FileInfo[] fileInfo = dirinfo.GetFiles("*", SearchOption.AllDirectories);

            trackcount = fileInfo.Length;
        }

        private void checkFolder(string FolderName)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(FolderName);

            foreach (FileInfo fileInfo in dirinfo.GetFiles("*", SearchOption.AllDirectories))
            {
                if (BrowserWorkerPath.CancellationPending) break;

                ATLReader = new ATL.AudioReaders.AudioFileReader(fileInfo.FullName);

                if (ATLReader.Artist != "" && ATLReader.Album != "")
                {
                    BrowserItem sp;
                    sp.artist = ATLReader.Artist;
                    sp.album = ATLReader.Album;
                    sp.path = fileInfo.DirectoryName + ext;
                    sp.hasart = false;
                    ++processed;
                    if (!albarts.Contains(sp))
                    {
                        albarts.Add(sp);
                        bool b = System.IO.File.Exists(sp.path);
                        sp.hasart = b;
                        lock (itemstoadd)
                        {
                            itemstoadd.Add(sp);
                        }

                        if (counter++ == 10)
                        {
                            BrowserWorkerPath.ReportProgress(processed * 100 / trackcount);
                            counter = 0;
                        }
                    }
                }
            }
           
        }

        private void BrowserWorkerPath_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                albarts = new List<BrowserItem>();
                counter = 0;
                ext = "\\" + ((string[])e.Argument)[0];
                processed = 0;
                trackcount = 0;

                getFileCount(((string[])e.Argument)[1]);

                checkFolder(((string[])e.Argument)[1]);

                if (!e.Cancel)
                    BrowserWorkerPath.ReportProgress(100);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error getting folder information");
            }
        }

        private void BrowserWorkerCOM_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (itemstoadd)
            {
                megaListBrowserCOM.BeginUpdate();
                foreach (BrowserItem b in itemstoadd)
                {
                    ListViewItem i = megaListBrowserCOM.Items.Add(b.artist);
                    i.SubItems.Add(b.album);
                    i.SubItems.Add(b.hasart ? "Yes" : "No");
                    i.ImageIndex = b.hasart ? 0 : 1;
                    i.Tag = b;
                    itemsadded.Add(b);
                }
                megaListBrowserCOM.EndUpdate();
                itemstoadd.Clear();
            }
            BrowserCOMProgress.Value = e.ProgressPercentage;
        }

        private void BrowserWorkerCOM_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BrowserCOMProgress.Visible = false;
            BrowserCOMRefresh.Enabled = true;
            BrowserCOMCancel.Enabled = false;
            BrowserCOMAddArtless.Enabled = true;
            if (waitingtoclose && !a.ThreadsActive() && !BrowserWorkerCOM.IsBusy) this.Close();
        }

        private void BrowserWorkerPath_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (itemstoadd)
            {
                megaListBrowserPath.BeginUpdate();
                foreach (BrowserItem b in itemstoadd)
                {
                    ListViewItem i = megaListBrowserPath.Items.Add(b.artist);
                    i.SubItems.Add(b.album);
                    i.SubItems.Add(b.hasart ? "Yes" : "No");
                    i.ImageIndex = b.hasart ? 0 : 1;
                    i.Tag = b;
                    itemsadded.Add(b);
                }
                megaListBrowserPath.EndUpdate();
                itemstoadd.Clear();
            }
            BrowserPathProgress.Value = e.ProgressPercentage;
        }

        private void BrowserWorkerPath_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BrowserPathProgress.Visible = false;
            BrowserPathRefresh.Enabled = true;
            BrowserPathCancel.Enabled = false;
            BrowserPathAddArtless.Enabled = true;
            if (waitingtoclose && !a.ThreadsActive() && !BrowserWorkerPath.IsBusy) this.Close();
        }

        private ListViewColumnSorter lvwColumnSorterCOM;
        private ListViewColumnSorter lvwColumnSorterPath;

        private void megaListBrowserCOM_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!BrowserWorkerCOM.IsBusy)
            {
                // Determine if clicked column is already the column that is being sorted.
                if (e.Column == lvwColumnSorterCOM.SortColumn)
                {
                    // Reverse the current sort direction for this column.
                    if (lvwColumnSorterCOM.Order == SortOrder.Ascending)
                    {
                        lvwColumnSorterCOM.Order = SortOrder.Descending;
                    }
                    else
                    {
                        lvwColumnSorterCOM.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // Set the column number that is to be sorted; default to ascending.
                    lvwColumnSorterCOM.SortColumn = e.Column;
                    lvwColumnSorterCOM.Order = SortOrder.Ascending;
                }

                // Perform the sort with these new sort options.
                megaListBrowserCOM.Sort();
            }
        }

        private void megaListBrowserCOM_ItemActivate(object sender, EventArgs e)
        {
            BrowserItem b = (BrowserItem)megaListBrowserCOM.SelectedItems[0].Tag;
            a.AddTask(new Task(a, b.artist, b.album, b.path, false));
        }

        private void BrowserCOMRefresh_Click(object sender, EventArgs e)
        {
            if (BrowserCOMRefresh.Enabled)
            {
                megaListBrowserCOM.Items.Clear();
                lock (itemstoadd)
                {
                    itemstoadd.Clear();
                    itemsadded.Clear();
                }

                BrowserWorkerCOM.RunWorkerAsync(Properties.Settings.Default.SaveFileName);
                
                BrowserCOMProgress.Value = 0;
                BrowserCOMProgress.Visible = true;
                BrowserCOMRefresh.Enabled = false;
                BrowserCOMCancel.Enabled = true;
                BrowserCOMAddArtless.Enabled = false;
            }
        }

        private void BrowserCOMCancel_Click(object sender, EventArgs e)
        {
            BrowserWorkerCOM.CancelAsync();
        }

        private void BrowserCOMAddArtless_Click(object sender, EventArgs e)
        {
            foreach (BrowserItem b in itemsadded)
            {
                if (!b.hasart)
                {
                    a.AddTask(new Task(a, b.artist, b.album, b.path, false));
                }
            }
        }

        private void listViewQueue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedQueueItems();
            }
        }
        private bool _BlockUpdates;
        public bool BlockUpdates
        {
            get { return _BlockUpdates; }
            set { _BlockUpdates = value; }
        }

        private void RemoveSelectedQueueItems()
        {
            bool removedselected = false;
            BlockUpdates = true;
            foreach (ListViewItem i in listViewQueue.SelectedItems)
            {
                if (i.Tag as Task == selectedtask)
                    removedselected = true;
                a.RemoveTask(i.Tag as Task);
            }
            BlockUpdates = false;
            if (removedselected)
            {
                if (listViewQueue.Items.Count > 0)
                {
                    SwitchToTask(listViewQueue.Items[0].Tag as Task);
                }
            }
        }

        private void BrowserPathRefresh_Click(object sender, EventArgs e)
        {
            if (BrowserPathRefresh.Enabled)
            {
                megaListBrowserPath.Items.Clear();
                lock (itemstoadd)
                {
                    itemstoadd.Clear();
                    itemsadded.Clear();
                }

                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    BrowserWorkerPath.RunWorkerAsync(new string[] { Properties.Settings.Default.SaveFileName, folderBrowserDialog.SelectedPath });
                }

                BrowserPathProgress.Value = 0;
                BrowserPathProgress.Visible = true;
                BrowserPathRefresh.Enabled = false;
                BrowserPathCancel.Enabled = true;
                BrowserPathAddArtless.Enabled = false;
            }
        }

        private void BrowserPathCancel_Click(object sender, EventArgs e)
        {
            BrowserWorkerPath.CancelAsync();
        }

        private void BrowserPathAddArtless_Click(object sender, EventArgs e)
        {
            foreach (BrowserItem b in itemsadded)
            {
                if (!b.hasart)
                {
                    a.AddTask(new Task(a, b.artist, b.album, b.path, false));
                }
            }
        }

        private void megaListBrowserPath_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!BrowserWorkerPath.IsBusy)
            {
                // Determine if clicked column is already the column that is being sorted.
                if (e.Column == lvwColumnSorterPath.SortColumn)
                {
                    // Reverse the current sort direction for this column.
                    if (lvwColumnSorterPath.Order == SortOrder.Ascending)
                    {
                        lvwColumnSorterPath.Order = SortOrder.Descending;
                    }
                    else
                    {
                        lvwColumnSorterPath.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // Set the column number that is to be sorted; default to ascending.
                    lvwColumnSorterPath.SortColumn = e.Column;
                    lvwColumnSorterPath.Order = SortOrder.Ascending;
                }

                // Perform the sort with these new sort options.
                megaListBrowserPath.Sort();
            }
        }

        private void megaListBrowserPath_ItemActivate(object sender, EventArgs e)
        {
            BrowserItem b = (BrowserItem)megaListBrowserPath.SelectedItems[0].Tag;
            a.AddTask(new Task(a, b.artist, b.album, b.path, false));
        }

        static Bitmap ResizeBitmap(Bitmap source, int Width, int Height)
        {
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int)((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int)((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(96.0f, 96.0f);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(source,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        private void testBoxFileSave_TextChanged(object sender, EventArgs e)
        {
            this.toolTip.SetToolTip(this.testBoxFileSave, this.testBoxFileSave.Text);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm d = new SettingsForm(this);
            d.ShowDialog(this);

            ThumbNailSize = new Size((int)Properties.Settings.Default.ThumbnailWidth, (int)Properties.Settings.Default.ThumbnailHeight);
            UpdateSize(true);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void browserToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBottomPanel();
        }

        private void queueToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBottomPanel();
        }
    }


}


