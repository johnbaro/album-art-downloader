using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AlbumArtDownloader
{
	public partial class FileBrowser : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		private enum MediaInfoState
		{
			Uninitialised,
			Initialised,
			Error
		}

		public enum BrowserState
		{
			/// <summary>No search been performed yet.</summary>
			Ready,
			/// <summary>Finding media files with artist and album tags, and simultaneously finding art for them.</summary>
			FindingFiles,
			/// <summary>No longer finding media files, only finding art for albums already located.</summary>
			FindingArt,
			/// <summary>Search completed successfully.</summary>
			Done,
			/// <summary>Search stopped by user.</summary>
			Stopped,
			/// <summary>Search abandoned due to error.</summary>
			Error
		}

		private static MediaInfoLib.MediaInfo sMediaInfo;
		private static MediaInfoState sMediaInfoState;

		private Thread mSearchThread;
		private ObservableAlbumCollection mAlbums = new ObservableAlbumCollection();

		private Thread mArtFileSearchThread;
		private Queue<Album> mArtFileSearchQueue = new Queue<Album>();
		private AutoResetEvent mArtFileSearchTrigger = new AutoResetEvent(false);
		private string mImagePathPattern; //This is held separately to the value of mImagePathPatternBox.PathPattern, so it will be constant during a search.

		public FileBrowser()
		{
			InitializeComponent();
			LoadPathPatternHistory();

			mResults.ItemsSource = mAlbums;
			mBrowse.Click += new RoutedEventHandler(OnBrowseForFilePath);

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(SelectAllExec)));

			IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);

			mAlbums.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAlbumsCollectionChanged);

			mResults.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);

			CreateArtFileSearchThread();
		}

		private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				//Window is being shown.

				if (sMediaInfoState == MediaInfoState.Uninitialised)
				{
					//Initialise the MediaInfo tag reader
					sMediaInfo = new MediaInfoLib.MediaInfo();
					AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();
					if (String.IsNullOrEmpty(sMediaInfo.Option("Info_Version", String.Format("0.7.4.7;{0};{1}", assemblyName.Name, assemblyName.Version))))
					{
						sMediaInfoState = MediaInfoState.Error;
					}
					else
					{
						sMediaInfoState = MediaInfoState.Initialised;
					}
				}
				if (sMediaInfoState == MediaInfoState.Error)
				{
					MessageBox.Show("The version of the MediaInfo.dll found is not compatible with the expected version. Please re-install the latest version of Album Art Downloader to use the File Browser functionality", "DLL Version Mismatch", MessageBoxButton.OK, MessageBoxImage.Error);
					this.Close();
				}
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			SaveSettings();
		}
		protected override void OnClosed(EventArgs e)
		{
			AbortSearch();
			if (mArtFileSearchThread != null)
			{
				mArtFileSearchThread.Abort();
				mArtFileSearchThread = null;
			}
			base.OnClosed(e);
		}

		#region Settings
		private void LoadPathPatternHistory()
		{
			ICollection<String> history = mImagePathPatternBox.History;
			history.Clear();
			foreach (string historyItem in Properties.Settings.Default.FileBrowseImagePathHistory)
			{
				history.Add(historyItem);
			}
		}

		private void SavePathPatternHistory()
		{
			ICollection<String> history = mImagePathPatternBox.History;

			Properties.Settings.Default.FileBrowseImagePathHistory.Clear();
			foreach (string historyItem in history)
			{
				Properties.Settings.Default.FileBrowseImagePathHistory.Add(historyItem);
			}
		}

		public void SaveSettings()
		{
			SavePathPatternHistory();
		}
		#endregion

		#region Properties
		string IAppWindow.Description
		{
			get
			{
				if (String.IsNullOrEmpty(mFilePathBox.Text))
					return "File Browser";

				return "File Browser: " + Path.GetFileName(mFilePathBox.Text);
			}
		}

		public static readonly DependencyPropertyKey StatePropertyKey = DependencyProperty.RegisterReadOnly("State", typeof(BrowserState), typeof(FileBrowser), new FrameworkPropertyMetadata(BrowserState.Ready));
		public BrowserState State
		{
			get { return (BrowserState)GetValue(StatePropertyKey.DependencyProperty); }
			private set { SetValue(StatePropertyKey, value); }
		}

		public static readonly DependencyPropertyKey ProgressTextPropertyKey = DependencyProperty.RegisterReadOnly("ProgressText", typeof(string), typeof(FileBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ProgressText
		{
			get { return (string)GetValue(ProgressTextPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressTextPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ErrorTextPropertyKey = DependencyProperty.RegisterReadOnly("ErrorText", typeof(string), typeof(FileBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ErrorText
		{
			get { return (string)GetValue(ErrorTextPropertyKey.DependencyProperty); }
			private set { SetValue(ErrorTextPropertyKey, value); }
		}
		#endregion

		private void OnBrowseForFilePath(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.FileDialog filePathBrowser = (Microsoft.Win32.FileDialog)Resources["mFilePathBrowser"];
			filePathBrowser.InitialDirectory = mFilePathBox.Text;
			filePathBrowser.FileName = "[Filename will be ignored]";
			if (filePathBrowser.ShowDialog(this).GetValueOrDefault())
			{
				mFilePathBox.Text = System.IO.Path.GetDirectoryName(filePathBrowser.FileName);
			}
		}

		#region Command Handlers
		private void FindExec(object sender, RoutedEventArgs e)
		{
			mImagePathPatternBox.AddPatternToHistory();
			Search(mFilePathBox.Text, mIncludeSubfolders.IsChecked.GetValueOrDefault(), mImagePathPatternBox.PathPattern);
		}

		private void StopExec(object sender, RoutedEventArgs e)
		{
			AbortSearch();
		}

		private void SelectAllExec(object sender, RoutedEventArgs e)
		{
			AllSelected = !AllSelected.GetValueOrDefault(true); //Mimic behaviour of clicking on the checkbox.
		}
		#endregion

		#region Select All
		private bool mSettingAllSelected = false; //Flag to prevent listening to IsSelected changes when setting them all
		private bool? mAllSelected = false;
		/// <summary>
		/// This can be set to true, to enable all sources, false, to disable them all,
		/// or null to leave them as they are. It will return true if all sources are
		/// Selected, false if they are all disabled, or null if they are mixed.
		/// </summary>
		public bool? AllSelected
		{
			get
			{
				return mAllSelected;
			}
			set
			{
				if (value != mAllSelected)
				{
					if (value.HasValue)
					{
						mSettingAllSelected = true;
						if (value.Value)
						{
							mResults.SelectAll();
						}
						else
						{
							mResults.SelectedItems.Clear();
						}
						mSettingAllSelected = false;
					}
					mAllSelected = value;
					NotifyPropertyChanged("AllSelected");
				}
			}
		}
		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!mSettingAllSelected) //Ignore selection change while setting selection from AllSelected setter.
			{
				if (e.RemovedItems.Count > 0)
				{
					//Check to see if there is now nothing selected
					if (mResults.SelectedItems.Count == 0)
					{
						mAllSelected = false; //Don't change through the accessor, so it doesn't bother trying to reapply the selection
						NotifyPropertyChanged("AllSelected");
						return;
					}
				}
				if (e.AddedItems.Count > 0)
				{
					//Check to see if there is now all selected
					if (mResults.SelectedItems.Count == mResults.Items.Count)
					{
						mAllSelected = true; //Don't change through the accessor, so it doesn't bother trying to reapply the selection
						NotifyPropertyChanged("AllSelected");
						return;
					}
				}
				//Not all items are selected, so set property as mixed.
				mAllSelected = null;
				NotifyPropertyChanged("AllSelected");
			}
		}
		#endregion

		#region Media File Searching
		/// <summary>
		/// Begins an asynchronous search for files.
		/// </summary>
		public void Search(string rootPath, bool includeSubfolders, string imagePathPattern)
		{
			//Ensure UI is synched
			mFilePathBox.Text = rootPath;
			mIncludeSubfolders.IsChecked = includeSubfolders;
			mImagePathPatternBox.PathPattern = imagePathPattern;

			AbortSearch(); //Abort any existing search

			mImagePathPattern = imagePathPattern; //Keep this in a variable for the ArtFileSearchWorker to refer to.
			mSearchThread = new Thread(new ParameterizedThreadStart(SearchWorker));
			mSearchThread.Name = "File Browser Search";
			mSearchThread.Start(new SearchThreadParameters(rootPath, includeSubfolders));
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			ClearArtFileSearchQueue();
			
			//Restart the art file search thread
			mArtFileSearchThread.Abort();
			mArtFileSearchThread = null;
			CreateArtFileSearchThread();

			if (mSearchThread != null)
			{
				mSearchThread.Abort();
				mSearchThread = null;
			}
		}

		private struct SearchThreadParameters
		{
			private string mRootPath;
			private bool mIncludeSubfolders;
			public SearchThreadParameters(string rootPath, bool includeSubfolders)
			{
				mRootPath = rootPath;
				mIncludeSubfolders = includeSubfolders;
			}
			public string RootPath { get { return mRootPath; } }
			public bool IncludeSubfolders { get { return mIncludeSubfolders; } }
		}
		private void SearchWorker(object state)
		{
			Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				State = BrowserState.FindingFiles;
				mAlbums.Clear();
			}));
			
			SearchThreadParameters parameters = (SearchThreadParameters)state;
			SetProgressText("Searching...");

			DirectoryInfo root = null;
			try
			{
				root = new DirectoryInfo(parameters.RootPath);
			}
			catch (Exception e)
			{
				//Path not valid, so no images to find
				System.Diagnostics.Trace.WriteLine("Path not valid for file search: " + parameters.RootPath);
				System.Diagnostics.Trace.Indent();
				System.Diagnostics.Trace.WriteLine(e.Message);
				System.Diagnostics.Trace.Unindent();
				SetErrorState(String.Format("Could not search \"{0}\": {1}", parameters.RootPath, e.Message));
				return;
			}

			try
			{
				foreach (FileInfo file in NetMatters.FileSearcher.GetFiles(root, "*", parameters.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
				{
					SetProgressText("Searching... " + file.Name);

					string artistName, albumName;
					sMediaInfo.Open(file.FullName);
					try
					{
						artistName = sMediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Artist");
						albumName = sMediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Album");
					}
					catch (Exception e)
					{
						System.Diagnostics.Trace.WriteLine("Could not get artist and album information for file: " + file.FullName);
						System.Diagnostics.Trace.Indent();
						System.Diagnostics.Trace.WriteLine(e.Message);
						System.Diagnostics.Trace.Unindent();
						continue;
					}
					finally
					{
						sMediaInfo.Close();
					}
					if (!(String.IsNullOrEmpty(artistName) && String.IsNullOrEmpty(albumName))) //No point adding it if no artist or album could be found.
					{
						Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
						{
							mAlbums.Add(new Album(file.DirectoryName, artistName, albumName));
						}));
					}
				}
			}
			catch (ThreadAbortException)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.Stopped;
				}));
				return;
			}
			catch (Exception e)
			{
				SetErrorState(String.Format("Error occurred while searching: {0}", e.Message));
				return;
			}

			bool artSearchComplete;
			lock (mArtFileSearchQueue)
			{
				artSearchComplete = mArtFileSearchQueue.Count == 0;
			}

			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				if (artSearchComplete)
				{
					State = BrowserState.Done;
				}
				else
				{
					State = BrowserState.FindingArt;
					ProgressText = "Finding art...";
				}
			}));
		}

		/// <summary>
		/// Update the progress text, safe to call from the search worker thread.
		/// </summary>
		private void SetProgressText(string text)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				ProgressText = text;
			}));
		}

		/// <summary>
		/// Sets the error state, with error message. Safe to call from the search worker thread.
		/// </summary>
		private void SetErrorState(string message)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				ProgressText = String.Empty;
				ErrorText = message;
				State = BrowserState.Error;
			}));
		}

		#endregion

		#region ArtFile Searching
		private void OnAlbumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (Album album in e.NewItems)
					{
						QueueAlbumForArtFileSearch(album);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					ClearArtFileSearchQueue();
					break;
			}
		}
		
		private void QueueAlbumForArtFileSearch(Album album)
		{
			album.ArtFileStatus = ArtFileStatus.Queued;
			lock (mArtFileSearchQueue)
			{
				mArtFileSearchQueue.Enqueue(album);
			}
			mArtFileSearchTrigger.Set();
		}

		private void ClearArtFileSearchQueue()
		{
			lock (mArtFileSearchQueue)
			{
				foreach (Album album in mArtFileSearchQueue)
				{
					album.ArtFileStatus = ArtFileStatus.Unknown;
				}
				mArtFileSearchQueue.Clear();
			}
		}

		private void CreateArtFileSearchThread()
		{
			System.Diagnostics.Debug.Assert(mArtFileSearchThread == null, "An art file search thread already exists!");
			mArtFileSearchThread = new Thread(new ThreadStart(ArtFileSearchWorker));
			mArtFileSearchThread.Name = "Art File Searcher";
			mArtFileSearchThread.Start();
		}

		private void ArtFileSearchWorker()
		{
			try
			{
				do
				{
					mArtFileSearchTrigger.WaitOne(); //Wait until there is work to do

					do //Loop through all the queued art.
					{
						Album album;
						lock (mArtFileSearchQueue)
						{
							if (mArtFileSearchQueue.Count == 0)
							{
								break; //Nothing to search for, so go back and wait until there is.
							}
							else
							{
								album = mArtFileSearchQueue.Dequeue();
							}
						}
						System.Diagnostics.Debug.Assert(album.ArtFileStatus == ArtFileStatus.Queued, "Expecting the album to be queued for searching");
						album.ArtFileStatus = ArtFileStatus.Searching;
						try
						{
							SetProgressText(String.Format("Finding art... {0} / {1}", album.Artist, album.Name));

							string artFileSearchPattern = Common.SubstitutePlaceholders(mImagePathPattern, album.Artist, album.Name);

							if (!Path.IsPathRooted(artFileSearchPattern))
							{
								artFileSearchPattern = Path.Combine(album.BasePath, artFileSearchPattern);
							}
							foreach (string artFile in Common.ResolvePathPattern(artFileSearchPattern))
							{
								album.ArtFile = artFile;
								album.ArtFileStatus = ArtFileStatus.Present;
								break; //Only use the first art file that matches, if there are multiple matches.
							}
						}
						catch(Exception)
						{
							album.ArtFileStatus = ArtFileStatus.Unknown; //It might not be missing, we just haven't found it before hitting an exception
						}
						if (album.ArtFileStatus != ArtFileStatus.Present) //If it wasn't found, then it's missing.
							album.ArtFileStatus = ArtFileStatus.Missing;
					} while (true);

					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						if (State == BrowserState.FindingArt) //If only finding art, then that has now been done, and the state is now Done.
						{
							State = BrowserState.Done;
						}
					}));
				} while (true);
			}
			catch (ThreadAbortException)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.Stopped;
				}));
				return;
			}
		}
		#endregion

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