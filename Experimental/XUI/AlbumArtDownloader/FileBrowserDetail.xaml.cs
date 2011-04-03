using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AlbumArtDownloader.Controls;
using System.Windows.Controls;
using System.Xml;
using System.Text;
using AlbumArtDownloader.TreeViewViewModel;

namespace AlbumArtDownloader
{
    /// <summary>
    /// Interaction logic for FileBrowserDetail.xaml
    /// </summary>
    public partial class FileBrowserDetail : System.Windows.Window, INotifyPropertyChanged, IAppWindow
    {
        public FileBrowserDetail()
        {
            InitializeComponent();
            
            mBrowse.Click += new RoutedEventHandler(OnBrowseForFilePath);

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
        }

        #region Properties
        string IAppWindow.Description
        {
            get
            {
                return "File Browser Detail";
            }
        }
        #endregion

        /// <summary>
        /// Show standard file browser dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBrowseForFilePath(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.FileDialog filePathBrowser = (Microsoft.Win32.FileDialog)Resources["mFilePathBrowser"];
            filePathBrowser.InitialDirectory = mFilePathBox.Text;
            filePathBrowser.FileName = "";
            if (filePathBrowser.ShowDialog(this).GetValueOrDefault())
            {
                mFilePathBox.Text = System.IO.Path.GetDirectoryName(filePathBrowser.FileName);
            }
        }

        public void SaveSettings()
        {
            
        }
        public void LoadSettings()
        {
            
        }

        #region Property Notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        /// <summary>
        /// Find media files in current directory and show them 
        /// in treeview component in album - track - track info structure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindExec(object sender, ExecutedRoutedEventArgs e)
        {
            String[] files = new String[0];

            try
            {
                files = Directory.GetFiles(mFilePathBox.Text);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
            }

            // has map od disc and its tracks
            Dictionary<String, List<TagLib.File>> discs = new Dictionary<String, List<TagLib.File>>();

            // go through all files in given directory
            foreach (String item in files) 
            {
                String extension = Path.GetExtension(item);

                // work only with mp3 files
                // TODO: refactor using constants
                if (extension.Equals(".mp3"))
                {
                    TagLib.File tagFile = TagLib.File.Create(item);

                    string disc = tagFile.Tag.Album;
                    List<TagLib.File> values;

                    // check if there is entry for given key
                    bool contains = discs.ContainsKey(disc);

                    // there is no entry for current key - create one
                    // and insert empty list of files
                    if (!contains)
                    {
                        values = new List<TagLib.File>();
                        discs.Add(disc, values);
                    }
                    else
                    {
                        // TODO: some check for null pointer etc
                        bool success = discs.TryGetValue(disc, out values);
                    }

                    values.Add(tagFile);
                }
            }

            // prepare dataprovider for the treeview component
            List<DiscViewModel> discList = new List<DiscViewModel>();

            // go through hashmap of albums and add them all into 
            // treeview item model with all its children
            foreach (KeyValuePair<string, List<TagLib.File>> pair in discs)
            {
                TagLib.File[] tagLibFiles = pair.Value.ToArray();
                String discName = pair.Value[0].Tag.Album;
                DiscViewModel disc = new DiscViewModel(discName, tagLibFiles);
                discList.Add(disc);
            }

            // connect discList view model to the treeview component
            base.DataContext = discList.ToArray();

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
