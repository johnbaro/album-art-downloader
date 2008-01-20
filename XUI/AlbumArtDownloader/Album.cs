using System;
using System.ComponentModel;

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
		/// <remarks>Note that this will not automatically set <see cref="ArtFileSize"/>, that must
		/// be set separately if required.</remarks>
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

		/// <summary>
		/// The art file filesize in bytes, or 0 if none has been found
		/// </summary>
		private long mArtFileSize;
		public long ArtFileSize
		{
			get { return mArtFileSize; }
			set
			{
				if (value != mArtFileSize)
				{
					mArtFileSize = value;
					NotifyPropertyChanged("ArtFileSize");
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
