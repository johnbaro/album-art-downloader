using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An album, as found by one of the Browser tasks
	/// </summary>
	internal struct Album
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
		private FileInfo mArtFile;
		public FileInfo ArtFile
		{
			get { return mArtFile; }
			set { mArtFile = value; }
		}
	}
}
