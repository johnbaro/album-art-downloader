using System.Collections.Generic;

namespace AlbumArtDownloader
{
	internal class SearchParameters
	{
		private string mArtist;
		private string mAlbum;
		private Dictionary<Source, int?> mSources = new Dictionary<Source,int?>();

		public SearchParameters(string artist, string album)
		{
			mArtist = artist;
			mAlbum = album;
		}

		public string Artist
		{
			get { return mArtist; }
		}
		public string Album
		{
			get { return mAlbum; }
		}

		public void AddSource(Source source)
		{
			if (source.UseMaximumResults)
			{
				mSources[source] = source.MaximumResults;
			}
			else
			{
				mSources[source] = null;
			}
		}
		public void RemoveSource(Source source)
		{
			mSources.Remove(source);
		}

		public bool ContainsSource(Source source)
		{
			return mSources.ContainsKey(source);
		}

		public int? this[Source source]
		{
			get { return mSources[source]; }
		}
	}
}