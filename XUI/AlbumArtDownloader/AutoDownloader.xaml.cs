using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace AlbumArtDownloader
{
	internal class AlbumArtworkUpdatedEventArgs : EventArgs
	{
		private readonly Album mAlbum;

		public AlbumArtworkUpdatedEventArgs(Album album)
		{
			mAlbum = album;
		}

		public Album Album { get { return mAlbum; } }
	}

	internal partial class AutoDownloader : Window, IAppWindow
	{
		public event EventHandler<AlbumArtworkUpdatedEventArgs> AlbumArtworkUpdated;

		public AutoDownloader()
		{
			InitializeComponent();
		}

		public void Add(Album album, string pathPattern)
		{
			test.Items.Add(String.Format("{0} / {1} => {2}", album.Artist, album.Name, pathPattern));
		}

		#region IAppWindow Members

		public void SaveSettings()
		{
			//No settings that need saving
		}

		public void LoadSettings()
		{
			//No settings that need updating
		}

		public string Description
		{
			get { return String.Format("{0}: {1} albums", Title, test.Items.Count); }
		}

		#endregion
	}
}
