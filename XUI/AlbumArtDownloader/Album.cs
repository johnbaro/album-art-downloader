using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An album, as found by one of the Browser tasks
	/// </summary>
	internal class Album: INotifyPropertyChanged
	{
		public Album(string basePath, string artistName, string albumName)
		{
			mBasePath = basePath;
			mName = albumName;
			mArtist = artistName;
			mArtFile = null;
		}

		private string mName;
		public string Name
		{
			get 
			{
				if (mName == null) //Disallow Null names
					return String.Empty;

				return mName; 
			}
		}

		private string mArtist;
		public string Artist
		{
			get 
			{
				if (mArtist == null) //Disallow Null artists
					return String.Empty;

				return mArtist; 
			}
		}

		private string mBasePath;
		/// <summary>
		/// The path relative to which images are found.
		/// </summary>
		public string BasePath
		{
			get 
			{
				//Null base paths are allowed, but then relative image search paths can't be used.
				return mBasePath; 
			}
		}


		/// <summary>
		/// The art file, or null if none has been found
		/// </summary>
		private string mArtFile;
		public string ArtFile
		{
			get { return mArtFile; }
			set
			{
				if (value != mArtFile)
				{
					mArtFile = value;
					NotifyPropertyChanged("ArtFile");
				}
			}
		}

		private ArtFileStatus mArtFileStatus;
		public ArtFileStatus ArtFileStatus
		{
			get { return mArtFileStatus; }
			set
			{
				if (value != mArtFileStatus)
				{
					mArtFileStatus = value;
					NotifyPropertyChanged("ArtFileStatus");
				}
			}
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

	}
}
