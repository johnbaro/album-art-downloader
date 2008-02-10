using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
		private static MediaInfoLib.MediaInfo sMediaInfo;
		private static MediaInfoState sMediaInfoState;

		private Thread mMediaFileSearchThread;
		private Queue<SearchThreadParameters> mMediaFileSearchQueue = new Queue<SearchThreadParameters>();
		private AutoResetEvent mMediaFileSearchTrigger = new AutoResetEvent(false);
		private ObservableAlbumCollection mAlbums = new ObservableAlbumCollection();

		public FileBrowser()
		{
			InitializeComponent();
			LoadSettings();

			mBrowse.Click += new RoutedEventHandler(OnBrowseForFilePath);

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));

			IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);

			mResults.Albums = mAlbums;
			mResults.DragEnter += new DragEventHandler(OnResultsDragEnter);
			mResults.Drop += new DragEventHandler(OnResultsDragDrop);
			mResults.ProgressTextChanged += new EventHandler(OnResultsProgressTextChanged);
			mResults.StateChanged += new EventHandler(OnResultsStateChanged);

			CreateMediaFileSearchThread();
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
					if (String.IsNullOrEmpty(sMediaInfo.Option("Info_Version", String.Format("0.7.5.9;{0};{1}", assemblyName.Name, assemblyName.Version))))
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
			if (mMediaFileSearchThread != null)
			{
				mMediaFileSearchThread.Abort();
				mMediaFileSearchThread = null;
			}
			mResults.Dispose(); //Closes down the search thread
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
		public void LoadSettings()
		{
			LoadPathPatternHistory();
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

		#region Drag and Drop
		private void OnResultsDragEnter(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effects = DragDropEffects.Copy;
		}

		private void OnResultsDragDrop(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				foreach (string filename in (string[])e.Data.GetData(DataFormats.FileDrop))
				{
					EnqueueMediaFileSearch(new SearchThreadParameters(filename, mIncludeSubfolders.IsChecked.GetValueOrDefault()));
				}
			}
		}
		#endregion

		#region Command Handlers
		private void FindExec(object sender, ExecutedRoutedEventArgs e)
		{
			mImagePathPatternBox.AddPatternToHistory();
			Search(mFilePathBox.Text, mIncludeSubfolders.IsChecked.GetValueOrDefault(), mImagePathPatternBox.PathPattern);
		}

		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			AbortSearch();
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
			mAlbums.Clear(); //Clear existing results

			EnqueueMediaFileSearch(new SearchThreadParameters(rootPath, includeSubfolders));
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			//Abort the media file searching
			lock (mMediaFileSearchQueue)
			{
				mMediaFileSearchQueue.Clear();
			}
			mMediaFileSearchThread.Abort();
			mMediaFileSearchThread = null;
			CreateMediaFileSearchThread();
			mResults.AbortSearch();
		}

		private void EnqueueMediaFileSearch(SearchThreadParameters parameters)
		{
			mResults.ImagePathPattern = mImagePathPatternBox.PathPattern; //Set this once, rather than binding, so it is kept constant for a search.
			
			lock (mMediaFileSearchQueue)
			{
				mMediaFileSearchQueue.Enqueue(parameters);
			}
			mMediaFileSearchTrigger.Set();
		}

		private void CreateMediaFileSearchThread()
		{
			System.Diagnostics.Debug.Assert(mMediaFileSearchThread == null, "A media file search thread already exists!");
			mMediaFileSearchThread = new Thread(new ThreadStart(SearchWorker));
			mMediaFileSearchThread.Name = "File Browser Search";
			mMediaFileSearchThread.Start();
		}

		private void SearchWorker()
		{
			try
			{
				do
				{
					mMediaFileSearchTrigger.WaitOne(); //Wait until there is work to do

					do //Loop through all the queued searches.
					{
						SearchThreadParameters parameters;
						lock (mMediaFileSearchQueue)
						{
							if (mMediaFileSearchQueue.Count == 0)
							{
								break; //Nothing to search for, so go back and wait until there is.
							}
							else
							{
								parameters = mMediaFileSearchQueue.Dequeue();
							}
						}
						//Now searching for something, so set the state to indicate that.
						Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
						{
							State = BrowserState.FindingFiles;
							ProgressText = "Searching...";
						}));

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
						if ((root.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
						{
							//This isn't a directory, so try reading it as a single media file
							ReadMediaFile(new FileInfo(root.FullName));
						}
						else
						{
							//It is a director, so read its contents
							try
							{
								foreach (FileInfo file in NetMatters.FileSearcher.GetFiles(root, "*", parameters.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
								{
									ReadMediaFile(file);
								}
							}
							catch (Exception e)
							{
								SetErrorState("Error occurred while searching: " + e.Message);
								continue;
							}
						}
					} while (true);
					//Will break out here when there are no more searches queued

					//Finished with the FindingFiles state, so now set the state to whatever the results state is (either FindingArt, or Done).
					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						if (State != BrowserState.Error)
						{
							ProgressText = mResults.ProgressText;
							State = mResults.State;
						}
					}));
					//Return and wait until new searches are queued
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

		private void ReadMediaFile(FileInfo file)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				ProgressText = "Searching... " + file.Name;
			}));

			string artistName, albumName;
			try
			{
				sMediaInfo.Open(file.FullName);
				try
				{
					artistName = sMediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Artist");
					albumName = sMediaInfo.Get(MediaInfoLib.StreamKind.General, 0, "Album");
				}
				finally
				{
					sMediaInfo.Close();
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("Could not get artist and album information for file: " + file.FullName);
				System.Diagnostics.Trace.Indent();
				System.Diagnostics.Trace.WriteLine(e.Message);
				System.Diagnostics.Trace.Unindent();
				return; //If this media file couldn't be read, just go on to the next one.
			}

			if (!(String.IsNullOrEmpty(artistName) && String.IsNullOrEmpty(albumName))) //No point adding it if no artist or album could be found.
			{
				Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					mAlbums.Add(new Album(file.DirectoryName, artistName, albumName));
				}));
			}
		}

		private void OnResultsStateChanged(object sender, EventArgs e)
		{
			switch (mResults.State)
			{
				case BrowserState.Ready: //Not interested.
				case BrowserState.FindingArt:
					//Not interested, this will be set by SearchWorker when it finishes.
					break;
				case BrowserState.Done:
					//If we're currently FindingArt, then this does mean we're now done
					if (State == BrowserState.FindingArt)
						State = BrowserState.Done;
					break;
				case BrowserState.Error:
				case BrowserState.Stopped:
					//Inidicate this state
					State = mResults.State;
					break;
				case BrowserState.FindingFiles:
					System.Diagnostics.Debug.Fail("Unexpected state: Results should not be finding files");
					break;
				default:
					System.Diagnostics.Debug.Fail("Unexpected state");
					break;
			}
		}

		private void OnResultsProgressTextChanged(object sender, EventArgs e)
		{
			//Not binding, as we want to be able to update the progress text here directly too, but if the results indicates the progress text it would like displayed, we oblige here.
			if (!String.IsNullOrEmpty(mResults.ProgressText))
				ProgressText = mResults.ProgressText;
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

		#region Search Thread Parameters
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
		#endregion
	}
}