using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An observable collection of albums, unique by artist and album name (case insensitive).
	/// </summary>
	internal class ObservableAlbumCollection : ICollection<Album>, INotifyCollectionChanged
	{
		private Dictionary<string, Dictionary<string, Album>> mAlbumsByArtist = new Dictionary<string,Dictionary<string,Album>>();
		private int mVersion; //Tick to check for enumeration invalidation

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			NotifyCollectionChangedEventHandler temp = CollectionChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}

		public void Add(Album album)
		{
			//Check for uniquness
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();
			
			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					if (!artistAlbums.ContainsKey(albumNameKey))
					{
						//Unique
						artistAlbums.Add(albumNameKey, album);
						mVersion++;
						RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, album));
					}
				}
				else
				{
					//unique
					artistAlbums = new Dictionary<string, Album>();
					artistAlbums.Add(albumNameKey, album);
					mAlbumsByArtist.Add(artistNameKey, artistAlbums);
					mVersion++;
					RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, album));
				}
			}
		}

		public bool Remove(Album album)
		{
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();
			
			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					if (artistAlbums.Remove(albumNameKey))
					{
						mVersion++;
						RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, album));
						return true;
					}
				}
			}
			return false;
		}

		public void Clear()
		{
			lock (this)
			{
				mAlbumsByArtist.Clear();
				mVersion++;
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		public bool Contains(Album album)
		{
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();
			
			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					if (!artistAlbums.ContainsKey(albumNameKey))
					{
						return true;
					}
				}
			}
			return false;
		}

		public int Count
		{
			get 
			{
				int count = 0;
				lock (this)
				{
					foreach (Dictionary<string, Album> artistAlbums in mAlbumsByArtist.Values)
					{
						count += artistAlbums.Count;
					}
				}
				return count;
			}
		}
		public IEnumerator<Album> GetEnumerator()
		{
			return new Enumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void CopyTo(Album[] array, int arrayIndex)
		{
			List<Album> flatList = new List<Album>(this);
			flatList.CopyTo(array, arrayIndex);
		}

		private class Enumerator : IEnumerator<Album>, System.Collections.IEnumerator
		{
			private IEnumerator<Album> mInnerEnumerator;
			private ObservableAlbumCollection mObservableAlbumCollection;
			private int mVersion;

			public Enumerator(ObservableAlbumCollection observableAlbumCollection)
			{
				mObservableAlbumCollection = observableAlbumCollection;
				mVersion = mObservableAlbumCollection.mVersion;
				mInnerEnumerator = GetAllAlbums().GetEnumerator();
			}

			private IEnumerable<Album> GetAllAlbums()
			{
				foreach (Dictionary<string, Album> artistAlbums in mObservableAlbumCollection.mAlbumsByArtist.Values)
				{
					foreach (Album album in artistAlbums.Values)
					{
						yield return album;
					}
				}
			}

			public void Dispose()
			{
				mInnerEnumerator.Dispose();
				mInnerEnumerator = null;
			}

			public Album Current
			{
				get { return mInnerEnumerator.Current; }
			}

			object System.Collections.IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				//NOTE: Sometimes in WPF Binding this exception is expected, and is handled by the list view control
				if (mVersion != mObservableAlbumCollection.mVersion)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

				return mInnerEnumerator.MoveNext();
			}

			public void Reset()
			{
				//NOTE: Sometimes in WPF Binding this exception is expected, and is handled by the list view control
				if (mVersion != mObservableAlbumCollection.mVersion)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

				mInnerEnumerator.Reset();
			}
		}
	}
}
