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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String[] files = new String[0];

            try
            {
                files = Directory.GetFiles(TextBoxDirectory.Text);
            }
            catch (Exception exception)
            {

            }

            // has map od disc and its tracks
            Dictionary<String, List<TagLib.File>> discs = new Dictionary<String, List<TagLib.File>>();

            foreach (String item in files) 
            {
                String extension = Path.GetExtension(item);

                if (extension.Equals(".mp3"))
                {
                    TagLib.File tagFile = TagLib.File.Create(item);

                    string disc = tagFile.Tag.Disc.ToString();
                    List<TagLib.File> values;

                    // check if there is entry for given key
                    bool contains = discs.ContainsKey(disc);

                    // there is no entry for current key - create one
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

            List<DiscViewModel> discList = new List<DiscViewModel>();

            foreach (KeyValuePair<string, List<TagLib.File>> pair in discs)
            {
                TagLib.File[] tagLibFiles = pair.Value.ToArray();

                uint discNumber = pair.Value[0].Tag.Disc;

                String discName = "CD ";

                discName += discNumber > 0 ? discNumber.ToString() : "1";

                DiscViewModel disc = new DiscViewModel(discName, tagLibFiles);

                discList.Add(disc);
            }

            base.DataContext = discList.ToArray();

        }

    }
}
