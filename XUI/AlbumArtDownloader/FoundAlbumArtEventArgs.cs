using System;

namespace AlbumArtDownloader
{
	internal class FoundAlbumArtEventArgs : EventArgs
	{
		private AlbumArt mAlbumArt;
		public FoundAlbumArtEventArgs(AlbumArt albumArt)
		{
			mAlbumArt = albumArt;
		}
		public AlbumArt AlbumArt
		{
			get { return mAlbumArt; }
		}
	}
}
