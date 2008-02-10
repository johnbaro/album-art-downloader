using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Interaction logic for FoobarBrowser.xaml
	/// </summary>

	public partial class FoobarBrowser : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		Foobar2000.Application07Class mFoobar;

		private Thread mReadMediaLibraryThread;
		private ObservableAlbumCollection mAlbums = new ObservableAlbumCollection();

		public FoobarBrowser()
		{
			InitializeComponent();
			LoadSettings();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec), new CanExecuteRoutedEventHandler(FindCanExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));

			IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);

			mResults.Albums = mAlbums;
			mResults.ProgressTextChanged += new EventHandler(OnResultsProgressTextChanged);
			mResults.StateChanged += new EventHandler(OnResultsStateChanged);
		}

		private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				//Window is being shown.
				ConnectToFoobar();
			}
		}

		private void ConnectToFoobar()
		{
			try
			{
				mFoobar = new Foobar2000.Application07Class();
			}
			catch (COMException ex)
			{
				mFoobar = null;
				System.Diagnostics.Trace.TraceWarning("Foobar2000 COM server could not be instantiated: " + ex.Message);
			}
			NotifyPropertyChanged("FoobarPresent");
			NotifyPropertyChanged("FoobarVersion");
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			SaveSettings();
		}
		protected override void OnClosed(EventArgs e)
		{
			AbortSearch();
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
				return "Foobar Browser";
			}
		}
		public static readonly DependencyPropertyKey StatePropertyKey = DependencyProperty.RegisterReadOnly("State", typeof(BrowserState), typeof(FoobarBrowser), new FrameworkPropertyMetadata(BrowserState.Ready));
		public BrowserState State
		{
			get { return (BrowserState)GetValue(StatePropertyKey.DependencyProperty); }
			private set { SetValue(StatePropertyKey, value); }
		}

		public static readonly DependencyPropertyKey ProgressTextPropertyKey = DependencyProperty.RegisterReadOnly("ProgressText", typeof(string), typeof(FoobarBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ProgressText
		{
			get { return (string)GetValue(ProgressTextPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressTextPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ProgressPropertyKey = DependencyProperty.RegisterReadOnly("Progress", typeof(double), typeof(FoobarBrowser), new FrameworkPropertyMetadata(0D));
		public double Progress
		{
			get { return (double)GetValue(ProgressPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ProgressMaxPropertyKey = DependencyProperty.RegisterReadOnly("ProgressMax", typeof(double), typeof(FoobarBrowser), new FrameworkPropertyMetadata(0D));
		public double ProgressMax
		{
			get { return (double)GetValue(ProgressMaxPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressMaxPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ErrorTextPropertyKey = DependencyProperty.RegisterReadOnly("ErrorText", typeof(string), typeof(FoobarBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ErrorText
		{
			get { return (string)GetValue(ErrorTextPropertyKey.DependencyProperty); }
			private set { SetValue(ErrorTextPropertyKey, value); }
		}

		/// <summary>
		/// Returns True if the Foobar Automation server is available.
		/// </summary>
		public bool FoobarPresent
		{
			get
			{
				return mFoobar != null;
			}
		}

		public string FoobarVersion
		{
			get
			{
				if (!FoobarPresent)
					return null;

				try
				{
					return mFoobar.Name + ": " + mFoobar.ApplicationPath;
				}
				catch (COMException e)
				{
					return "Could not connect to Foobar2000 automation server: " + e.Message;
				}
			}
		}
		#endregion

		#region Command Handlers
		private void FindExec(object sender, ExecutedRoutedEventArgs e)
		{
			mImagePathPatternBox.AddPatternToHistory();
			Search(mImagePathPatternBox.PathPattern);
		}
		private void FindCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = FoobarPresent;
		}

		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			AbortSearch();
		}
		#endregion

		#region Foobar Media Library Searching
		/// <summary>
		/// Begins an asynchronous search of the foobar media library
		/// </summary>
		public void Search(string imagePathPattern)
		{
			//Ensure UI is synched
			mImagePathPatternBox.PathPattern = imagePathPattern;
			mResults.ImagePathPattern = imagePathPattern; //Set this once, rather than binding, so it is kept constant for a search.

			AbortSearch(); //Abort any existing search
			mAlbums.Clear(); //Clear existing results
			
			//Test that the foobar library can be accessed
			try
			{
				mFoobar.MediaLibrary.GetHashCode();
			}
			catch (COMException)
			{
				//Couldn't be accessed. Try reconnecting
				ConnectToFoobar();

				//Lets try that again now.
				try
				{
					mFoobar.MediaLibrary.GetHashCode();
				}
				catch (COMException e)
				{
					//OK, really give up now.
					SetErrorState("Could not read media library: " + e.Message);
					return;
				}
			}

			//Should be OK, so try reading the library
		
			System.Diagnostics.Debug.Assert(mReadMediaLibraryThread == null, "A media library reader thread already exists!");
			mReadMediaLibraryThread = new Thread(new ThreadStart(ReadMediaLibraryWorker));
			mReadMediaLibraryThread.Name = "Read Foobar Media Library";
			mReadMediaLibraryThread.Start();
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			//Abort the media file searching
			if (mReadMediaLibraryThread != null)
			{
				mReadMediaLibraryThread.Abort();
				mReadMediaLibraryThread = null;
			}

			mResults.AbortSearch();
		}

		private void ReadMediaLibraryWorker()
		{
			try
			{
				var mediaLibrary = mFoobar.MediaLibrary.GetTracks(null);
				//Now searching for something, so set the state to indicate that.
				//Also set the count of albums, for the progress bar
				Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.FindingFiles;
					Progress = 0;
					ProgressMax = mediaLibrary.Count;
					ProgressText = "Reading Media Library...";
				}));
				foreach (Foobar2000.Track07 track in mediaLibrary)
				{
					string artistName = track.FormatTitle("%album artist%");
					string albumName = track.FormatTitle("%album%");
					string path = track.FormatTitle("%path%");
					try
					{
						path = System.IO.Path.GetDirectoryName(path);
					}
					catch (Exception e)
					{
						System.Diagnostics.Trace.WriteLine("Could not get file path for \""+artistName + "\" / \"" + albumName + "\": " + path);
						System.Diagnostics.Trace.Indent();
						System.Diagnostics.Trace.WriteLine(e.Message);
						System.Diagnostics.Trace.Unindent();
						continue; //skip this one, can't find the path.
					}
					
					Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						Progress++;
						if (!(String.IsNullOrEmpty(artistName) && String.IsNullOrEmpty(albumName))) //No point adding it if no artist or album could be found.
						{
							mAlbums.Add(new Album(path, artistName, albumName));
						}
					}));
				}

				//Finished with the FindingFiles state, so now set the state to whatever the results state is (either FindingArt, or Done).
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					ProgressText = mResults.ProgressText;
					State = mResults.State;
				}));
			}
			catch (ThreadAbortException)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.Stopped;
				}));
			}
			catch (Exception e)
			{
				SetErrorState(String.Format("Error occurred while reading media library: {0}", e.Message));
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
				NotifyPropertyChanged("FoobarVersion");
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

	}
}