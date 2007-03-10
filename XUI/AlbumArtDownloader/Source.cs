using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using AlbumArtDownloader.Scripts;
using System.Drawing;

namespace AlbumArtDownloader
{
	internal abstract class Source : ISource
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler SearchCompleted;

		private ObservableCollection<IAlbumArt> mResults = new ObservableCollection<IAlbumArt>();

		private Thread mSearchThread;

		public Source()
		{
		}

		#region Abstract members
		public abstract string Name {get;}
		public abstract string Author { get;}
		public abstract string Version { get;}

		/// <summary>
		/// Perform the actual internal searching operation
		/// This should not update any WPF controls, or
		/// perform any direct modification of property values.
		/// </summary>
		protected abstract void SearchInternal(string artist, string album, IScriptResults results);

		internal abstract Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter);
		#endregion

		#region Basic properties
		private bool mIsEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return mIsEnabled;
			}
			set
			{
				if (mIsEnabled != value)
				{
					mIsEnabled = value;
					NotifyPropertyChanged("IsEnabled");

					if (!mIsEnabled && IsSearching)
					{
						AbortSearch();
					}
				}
			}
		}

		public ObservableCollection<IAlbumArt> Results
		{
			get { return mResults; }
		}

		private bool mIsSearching;
		public bool IsSearching
		{
			get
			{
				return mIsSearching;
			}
			private set
			{
				mIsSearching = value;
				NotifyPropertyChanged("IsSearching");
			}
		}

		private int mEstimatedResultsCount;
		public int EstimatedResultsCount
		{
			get
			{
				return mEstimatedResultsCount;
			}
			private set
			{
				mEstimatedResultsCount = value;
				NotifyPropertyChanged("EstimatedResultsCount");
			}
		}

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
		/// Begins an asynchronous search. Results are raised by <see cref="FoundAlbumArt"/> events.
		/// </summary>
		public void Search(string artist, string album)
		{
			AbortSearch(); //Abort any existing search
			mSearchThread = new Thread(new ParameterizedThreadStart(SearchWorker));
			mSearchThread.Name = String.Format("{0} search", Name);
			mSearchThread.Start(new SearchThreadParameters(Dispatcher.CurrentDispatcher, artist, album));
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			if (mSearchThread != null)
			{
				mSearchThread.Abort();
			}
		}

		private struct SearchThreadParameters
		{
			private Dispatcher mDispatcher;
			private string mArtist;
			private string mAlbum;
			public SearchThreadParameters(Dispatcher dispatcher, string artist, string album)
			{
				mDispatcher = dispatcher;
				mArtist = artist;
				mAlbum = album;
			}
			public Dispatcher Dispatcher { get { return mDispatcher; } }
			public string Artist { get { return mArtist; } }
			public string Album { get { return mAlbum; } }
		}
		private void SearchWorker(object state)
		{
			SearchThreadParameters parameters = (SearchThreadParameters)state;

			try
			{
				parameters.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
				{
					Results.Clear();
					IsSearching = true;
				}));

				SearchInternal(parameters.Artist, parameters.Album, new ScriptResults(this, parameters.Dispatcher));
			}
			finally
			{
				parameters.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
				{
					RaiseSearchCompleted();
					IsSearching = false;
				}));
			}
		}

		private void RaiseSearchCompleted()
		{
			EventHandler temp = SearchCompleted;
			if (temp != null)
			{
				temp(this, EventArgs.Empty);
			}
		}

		
		private class ScriptResults : IScriptResults
		{
			private Source mSource;
			private Dispatcher mDispatcher;

			public ScriptResults(Source source, Dispatcher dispatcher)
			{
				mSource = source;
				mDispatcher = dispatcher;
			}

			#region Redirects for obsolete members
			//This region can be copied and pasted for reuse
			public void SetCountEstimate(int count)
			{
				EstimatedCount = count;
			}
			public void AddThumb(string thumbnailUri, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailUri, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void AddThumb(System.IO.Stream thumbnailStream, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailStream, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void AddThumb(System.Drawing.Image thumbnailImage, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailImage, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			#endregion

			public int EstimatedCount
			{
				get
				{
					return mSource.EstimatedResultsCount;
				}
				set
				{
					mDispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
					{
						mSource.EstimatedResultsCount = value;
					}));
				}
			}

			public void Add(object thumbnail, string name, object fullSizeImageCallback)
			{
				Add(thumbnail, name, -1, -1, fullSizeImageCallback);
			}
			public void Add(object thumbnail, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				//TODO: does downloading the thumbnail need to be asynch?
				Bitmap thumbnailBitmap = BitmapHelpers.GetBitmap(thumbnail);

				if (thumbnailBitmap != null)
				{
					mDispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
					{
						mSource.Results.Add(new AlbumArt(mSource,
							thumbnailBitmap,
							name,
							fullSizeImageWidth,
							fullSizeImageHeight,
							fullSizeImageCallback));
					}));
				}
			}
		}
	}
}
