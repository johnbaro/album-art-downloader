using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using AlbumArtDownloader.Controls;
using AlbumArtDownloader.Scripts;


namespace AlbumArtDownloader
{
	public partial class ArtSearchWindow : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		private Sources mSources = new Sources();

		private Thread mAutoDownloadFullSizeImagesThread;
		private AutoResetEvent mAutoDownloadFullSizeImagesTrigger = new AutoResetEvent(true);
		private Queue<AlbumArt> mResultsToAutoDownloadFullSizeImages = new Queue<AlbumArt>();
		private CommandBinding mStopAllCommandBinding;

		public ArtSearchWindow()
		{
			InitializeComponent();
			
			mAutoDownloadFullSizeImagesThread = new Thread(new ThreadStart(AutoDownloadFullSizeImagesWorker));
			mAutoDownloadFullSizeImagesThread.Name = "Auto Download Full Size Images";
			mAutoDownloadFullSizeImagesThread.Priority = ThreadPriority.BelowNormal;
			mAutoDownloadFullSizeImagesThread.Start();

			//Bind the SelectAll checkbox
			Binding selectAllBinding = new Binding("AllEnabled");
			selectAllBinding.Source = mSources;
			selectAllBinding.Mode = BindingMode.TwoWay;
			BindingOperations.SetBinding(mSelectAll, CheckBox.IsCheckedProperty, selectAllBinding);

			//Bind the Search button being enabled
			Binding sourceEnabledBinding = new Binding("AllEnabled");
			sourceEnabledBinding.Source = mSources;
			sourceEnabledBinding.Mode = BindingMode.OneWay;
			sourceEnabledBinding.Converter = new NotFalseConverter();
			BindingOperations.SetBinding(mSearch, Button.IsEnabledProperty, sourceEnabledBinding);

			mSources.CombinedResults.CollectionChanged += new NotifyCollectionChangedEventHandler(OnResultsChanged);

			//Commands:
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(CopyExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, new ExecutedRoutedEventHandler(SaveAsExec)));
			CommandBindings.Add(new CommandBinding(AlbumArtDownloader.Controls.ArtPanelList.Commands.Preview, new ExecutedRoutedEventHandler(PreviewExec)));

			//Stop All is bound only when doing a search (so the Stop All button only appears while searching)
			mStopAllCommandBinding = new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec));
			
			mSourcesViewer.ItemsSource = mSources;
			mResultsViewer.ItemsSource = mSources.CombinedResults;

			foreach (IScript script in ((App)Application.Current).Scripts)
			{
				ScriptSource scriptSource = new ScriptSource(script);
				mSources.Add(scriptSource);
				//Hook the complete event to know when to hide the Stop All button
				scriptSource.SearchCompleted += OnSourceSearchCompleted;
			}

			LocalFilesSource localFilesSource = new LocalFilesSource();
			localFilesSource.DefaultFilePath = mDefaultSaveFolder.PathPattern;
			//Update the default file path if the pattern changes
			mDefaultSaveFolder.PathPatternChanged += delegate(object sender, DependencyPropertyChangedEventArgs e) { localFilesSource.DefaultFilePath = (string)e.NewValue; };
			mSources.Add(localFilesSource);
			localFilesSource.SearchCompleted += OnSourceSearchCompleted;

			LoadSettings();
			//Initial value of AutoClose is taken from settings. May be overriden by command line parameters
			AutoClose = Properties.Settings.Default.AutoClose;

			this.Loaded += new RoutedEventHandler(OnLoaded);

			//Tab auto-select all for Artist and Album boxes
			mArtist.GotKeyboardFocus += OnAutoSelectTextBoxFocusChange;
			mAlbum.GotKeyboardFocus += OnAutoSelectTextBoxFocusChange;
		}

		private void OnAutoSelectTextBoxFocusChange(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			//If the textbox gains focus, but not from app gaining focus, and not from mouse press, then select its contents
			if (textBox != null && e.OldFocus != null && Mouse.LeftButton != MouseButtonState.Pressed)
			{
				textBox.SelectAll();
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			//Assign sensible defaults to things that depend on layout
			if (Properties.Settings.Default.PanelWidth < 0)
			{
				Properties.Settings.Default.PanelWidth = mResultsViewer.ActualWidth / 2;
				mResultsViewer.AutoSizePanels();
			}
		}
		
		#region Auto Download Full Size Images
		private AutoResetEvent mWaitForImage = new AutoResetEvent(false);

		/// <summary>
		/// Thread worker for downloading full size images
		/// </summary>
		private void AutoDownloadFullSizeImagesWorker()
		{
			do
			{
				mAutoDownloadFullSizeImagesTrigger.WaitOne(); //Wait until something has changed to look at

				while (Properties.Settings.Default.AutoDownloadFullSizeImages != AutoDownloadFullSizeImages.Never)
				{
					AlbumArt resultToProcess;
					lock (mResultsToAutoDownloadFullSizeImages)
					{
						if (mResultsToAutoDownloadFullSizeImages.Count == 0)
						{
							break; //Finished downloading all the pending results to download, so skip back round to the waiting.
						}
						else
						{
							resultToProcess = mResultsToAutoDownloadFullSizeImages.Dequeue();
						}
					}
					if (!resultToProcess.IsDownloading && !resultToProcess.IsFullSize && 
							(
								Properties.Settings.Default.AutoDownloadFullSizeImages == AutoDownloadFullSizeImages.Always ||
								(Properties.Settings.Default.AutoDownloadFullSizeImages == AutoDownloadFullSizeImages.WhenSizeUnknown && resultToProcess.ImageWidth == -1 && resultToProcess.ImageHeight == -1)
							)
						)
					{
						resultToProcess.RetrieveFullSizeImage(new WaitCallback(FullSizeImageDownloadCompleted));
						//Wait until it is finished to move on to the next one, which triggers the trigger.
						mWaitForImage.WaitOne();
					}
				}
			} while (true);
		}

		private void FullSizeImageDownloadCompleted(object sender)
		{
			mWaitForImage.Set();
		}

		private void OnAutoDownloadFullSizeImagesChanged(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.AutoDownloadFullSizeImages == AutoDownloadFullSizeImages.Always)
			{
				//Re-queue all images, to ensure they all get a chance at being auto-downloaded, even if they were skipped from having had a non-unknown size before.
				lock (mResultsToAutoDownloadFullSizeImages)
				{
					mResultsToAutoDownloadFullSizeImages.Clear();
					foreach (AlbumArt albumArt in mSources.CombinedResults)
					{
						mResultsToAutoDownloadFullSizeImages.Enqueue(albumArt);
					}
				}
			}
			mAutoDownloadFullSizeImagesTrigger.Set();
		}

		private void AddResultToAutoDownloadFullSizeImage(AlbumArt result)
		{
			lock(mResultsToAutoDownloadFullSizeImages)
			{
				mResultsToAutoDownloadFullSizeImages.Enqueue(result);
			}
			mAutoDownloadFullSizeImagesTrigger.Set();
		}

		private void ClearAutoDownloadFullSizeImageResults()
		{
			lock (mResultsToAutoDownloadFullSizeImages)
			{
				mResultsToAutoDownloadFullSizeImages.Clear();
			}
		}
		#endregion

		#region Searching


		public static readonly DependencyProperty ArtistProperty = DependencyProperty.Register("Artist", typeof(string), typeof(ArtSearchWindow));
		public string Artist
		{
			get { return (string)GetValue(ArtistProperty); }
			set { SetValue(ArtistProperty, value); }
		}

		public static readonly DependencyProperty AlbumProperty = DependencyProperty.Register("Album", typeof(string), typeof(ArtSearchWindow));
		public string Album
		{
			get { return (string)GetValue(AlbumProperty); }
			set { SetValue(AlbumProperty, value); }
		}

		private SearchParameters mSearchParameters;
		private void FindExec(object sender, RoutedEventArgs e)
		{
			//Check to see whether current results can be altered, or a new search is required
			if (mSearchParameters != null)
			{
				//Can only enhance if the artist and album to search for are identical
				if (Artist == mSearchParameters.Artist && Album == mSearchParameters.Album)
				{
					AlterSearch();
					return;
				}
			}
			if (mSources.CombinedResults.Count > 0 && Properties.Settings.Default.OpenResultsInNewWindow)
			{
				Common.NewSearchWindow(this).Search(Artist, Album);
			}
			else
			{
				StartSearch();
			}
		}

		/// <summary>
		/// Perform a search with the specified settings.
		/// If the window is held in a queue, defer the search until the window is shown.
		/// </summary>
		public void Search(string artist, string album)
		{
			Artist = artist;
			Album = album;
			if (IsVisible)
			{
				StartSearch();
			}
			else
			{
				this.IsVisibleChanged += SearchOnShown;
			}
		}

		private void SearchOnShown(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue) //If this window has just been shown
			{
				//Stop listening to the event
				this.IsVisibleChanged -= SearchOnShown;
				//and kick off the search
				StartSearch();
			}
		}

		/// <summary>
		/// Starts a search with the current settings
		/// </summary>
		private void StartSearch()
		{
			mSearchParameters = new SearchParameters(Artist, Album);

			mDefaultSaveFolder.AddPatternToHistory();
			foreach (Source source in mSources)
			{
				if (source.IsEnabled)
				{
					source.Search(mSearchParameters.Artist, mSearchParameters.Album);
					mSearchParameters.AddSource(source);
				}
				else
				{
					source.AbortSearch();
					source.Results.Clear();
				}
			}
			if (!CommandBindings.Contains(mStopAllCommandBinding))
			{
				CommandBindings.Add(mStopAllCommandBinding);
			}
		}
		/// <summary>
		/// Alters the existing search to include or exclude additional sources
		/// </summary>
		private void AlterSearch()
		{
			foreach (Source source in mSources)
			{
				if (source.IsEnabled)
				{
					bool performSearch = false;
					//Perform the search if the source was not previously searched
					if (!mSearchParameters.ContainsSource(source))
					{
						performSearch = true;
					}
					else
					{
						//Repeat the search if the maximum results settings for the source have changed.
						performSearch = source.SettingsChanged;
					}
					if(performSearch)
					{
						source.Search(Artist, Album);

						mSearchParameters.AddSource(source);
					}
				}
				else
				{
					//Remove the results if the source was unselected
					source.AbortSearch();
					source.Results.Clear();

					mSearchParameters.RemoveSource(source);
				}
			}
		}
		#endregion

		#region Results Updated
		private void OnResultsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Replace:
					foreach (AlbumArt art in e.NewItems)
					{
						BindAlbumArtDefaultFilePath(art);
						AddResultToAutoDownloadFullSizeImage(art);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					ClearAutoDownloadFullSizeImageResults();
					break;
			}
		}
		
		private void BindAlbumArtDefaultFilePath(AlbumArt art)
		{
			Binding defaultPathBinding = new Binding();
			defaultPathBinding.Source = mDefaultSaveFolder;
			defaultPathBinding.Path = new PropertyPath(ArtPathPatternBox.PathPatternProperty);
			defaultPathBinding.Mode = BindingMode.OneWay;
			defaultPathBinding.Converter = new AlbumArtDefaultFilePathPatternSubstitution();
			defaultPathBinding.ConverterParameter = new string[] { Artist, Album };
			BindingOperations.SetBinding(art, AlbumArt.DefaultFilePathPatternProperty, defaultPathBinding);
		}

		private class AlbumArtDefaultFilePathPatternSubstitution : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				string[] parameters = (string[])parameter;
				return ((string)value).Replace("%artist%", Common.MakeSafeForPath(parameters[0]))
									  .Replace("%album%", Common.MakeSafeForPath(parameters[1]));
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return null; //Not used
			}
		}
		#endregion

		#region Closing
		private bool mAutoClose;
		/// <summary>
		/// If set true, the window will automatically be closed after a save (but not save as) operation
		/// To set AutoClose behaviour without saving the setting, use <see cref="OverrideAutoClose"/>.
		/// </summary>
		public bool AutoClose
		{
			get 
			{
				return mAutoClose; 
			}
			set 
			{
				//Assigns the value to the settings too
				Properties.Settings.Default.AutoClose = value;
				OverrideAutoClose(value);
			}
		}

		/// <summary>
		/// Sets the <see cref="AutoClose"/> behaviour without persisting the value in Settings.
		/// </summary>
		/// <param name="value"></param>
		public void OverrideAutoClose(bool value)
		{
			if (mAutoClose != value)
			{
				mAutoClose = value;
				NotifyPropertyChanged("AutoClose");
			}
		}
		private void AutoCloseOnSave(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSaved" && ((AlbumArt)sender).IsSaved)
			{
				Close();
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			SaveSettings();
		}

		protected override void OnClosed(EventArgs e)
		{
			string winName = ((IAppWindow)this).Description;

			//Tear down all the threads in the background
			if (!ThreadPool.QueueUserWorkItem(TerminateThreads, winName))
			{
				System.Diagnostics.Trace.TraceError("Failed to queue thread termination work item - terminating synchronously");
				//Attempt to tear down synchronously
				TerminateThreads(winName);
			}

			//HACK: Absolutely outrageous slimy hack that stops other open windows becoming unresponsive for no good reason.
			new System.Windows.Controls.Primitives.Popup() { Width = 0, Height = 0, IsOpen = true }.IsOpen = false;
			
			base.OnClosed(e);
		}

		private void TerminateThreads(object state)
		{
			string winName = state as string ?? String.Empty;
			System.Diagnostics.Trace.TraceInformation("Starting thread tear-down for " + winName);

			mAutoDownloadFullSizeImagesThread.Abort();
			mAutoDownloadFullSizeImagesThread.Join();
			foreach (Source source in mSources)
			{
				source.TerminateSearch();
				//Dispose of the art in results (images need disposal)
				foreach (IDisposable result in source.Results)
				{
					result.Dispose();
				}
			}

			System.Diagnostics.Trace.TraceInformation("Finished thread tear-down for " + winName);
		}

		#endregion

		#region Source Settings
		private void LoadSourceSettings()
		{
			foreach (Source source in mSources)
			{
				source.LoadSettings();
			}
		}
		private void SaveSourceSettings()
		{
			foreach (Source source in mSources)
			{
				source.SaveSettings();
			}
		}
		/// <summary>
		/// Sets the LocalFilesSource image search path.
		/// </summary>
		/// <param name="path"></param>
		public void SetLocalImagesPath(string path)
		{
			//TODO: Could there ever be more than one local files source?
			LocalFilesSource localFilesSource = mSources.OfType<LocalFilesSource>().FirstOrDefault();
			
			if (localFilesSource != null)
			{
				localFilesSource.SearchPathPattern = path;
			}
		}
		#endregion

		#region Disabling Sources
		/// <summary>
		/// Disable all sources except those specified
		/// </summary>
		public void UseSources(IEnumerable<string> useSources)
		{
			foreach (Source source in mSources) //Go through all the sources
			{
				source.IsEnabled = false; //Disabled unless it's name matches
				foreach (string useSource in useSources) //Check against the list of sources to use
				{
					//Use a case insensitive check
					if (source.Name.Equals(useSource, StringComparison.InvariantCultureIgnoreCase))
					{
						//The source name matches, so use it. Enable it, and stop checking names.
						source.IsEnabled = true;
						break;
					}
				}
			}
		}
		/// <summary>
		/// Disable the specified sources
		/// </summary>
		/// <param name="useSources"></param>
		public void ExcludeSources(IEnumerable<string> excludeSources)
		{
			foreach (Source source in mSources) //Go through all the sources
			{
				foreach (string excludeSource in excludeSources) //Check against the list of sources to use
				{
					//Use a case insensitive check
					if (source.Name.Equals(excludeSource, StringComparison.InvariantCultureIgnoreCase))
					{
						//The source name matches, so disable the source, and stop checking names
						source.IsEnabled = false;
						break;
					}
				}
			}
		}
		#endregion

		#region Default Save Folder
		/// <summary>
		/// This is the temporary default save path, if there is one. If the default save path
		/// is equal to this value, it should not be saved to settings.
		/// </summary>
		private string mDefaultSavePathIsTemporary;
		
		private void LoadDefaultSaveFolderHistory()
		{
			mDefaultSaveFolder.History.Clear();
			foreach (string historyItem in Properties.Settings.Default.DefaultSavePathHistory)
			{
				mDefaultSaveFolder.History.Add(historyItem);
			}
			mDefaultSaveFolder.PathPattern = Properties.Settings.Default.DefaultSavePath;
		}
		private void SaveDefaultSaveFolderHistory()
		{
			if (!String.Equals(mDefaultSaveFolder.PathPattern, mDefaultSavePathIsTemporary, StringComparison.CurrentCultureIgnoreCase))
			{
				//Only save the default path if it isn't a temporary one
				Properties.Settings.Default.DefaultSavePath = mDefaultSaveFolder.PathPattern;
			}
			Properties.Settings.Default.DefaultSavePathHistory.Clear();
			foreach (string historyItem in mDefaultSaveFolder.History)
			{
				Properties.Settings.Default.DefaultSavePathHistory.Add(historyItem);
			}
		}
		/// <summary>
		/// Sets the default save folder pattern, optionally on a temporary basis so it won't be saved
		/// as the default in the settings.
		/// </summary>
		public void SetDefaultSaveFolderPattern(string path)
		{
			SetDefaultSaveFolderPattern(path, false);
		}
		public void SetDefaultSaveFolderPattern(string path, bool temporary)
		{
			if (temporary)
				mDefaultSavePathIsTemporary = path;

			mDefaultSaveFolder.AddPatternToHistory(); //Save the previous value
			mDefaultSaveFolder.PathPattern = path; //Set the new value
		}
		#endregion

		#region Stop All
		private void OnSourceSearchCompleted(object sender, EventArgs e)
		{
			//Check to see if any sources are still searching
			foreach (Source source in mSources)
			{
				if (source != sender && source.IsSearching)
					return; //At least one source is still searching, so don't remove the Stop All command
			}
			//All sources have finished, so remove the Stop All handler
			CommandBindings.Remove(mStopAllCommandBinding);
			CommandManager.InvalidateRequerySuggested();
		}
		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			//Stop all the sources
			foreach (Source source in mSources)
			{
				source.AbortSearch();
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

		private void CopyExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.CopyToClipboard();
			}
		}

		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				if (AutoClose)
				{
					//The save operation is asynchronous, so connect the handler to watch for the save completing
					albumArt.PropertyChanged += AutoCloseOnSave;
				}

				string preset = e.Parameter as String;
				if (preset != null)
				{
					albumArt.Preset = preset;
				}

				albumArt.Save();
			}
		}

		private void SaveAsExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.PropertyChanged -= AutoCloseOnSave; //No auto-close for SaveAs operation.

				albumArt.SaveAs();
			}
		}

		private void PreviewExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.RetrieveFullSizeImage();
				//Show persistant preview window
				Common.NewPreviewWindow(this).AlbumArt = albumArt;
			}
		}

		#region IAppWindow
		public void LoadSettings()
		{
			System.Diagnostics.Trace.Write("Loading settings for: " + ((IAppWindow)this).Description + "... ");

			AlbumArtDownloader.Properties.WindowSettings.GetWindowSettings(this).LoadWindowState();
			LoadSourceSettings();

			if (String.IsNullOrEmpty(mDefaultSavePathIsTemporary)) //If a temporary save path has been set, don't override it.
			{
				LoadDefaultSaveFolderHistory();
			}

			System.Diagnostics.Trace.WriteLine("done.");
		}
		public void SaveSettings()
		{
			System.Diagnostics.Trace.Write("Saving settings from: " + ((IAppWindow)this).Description + "... ");

			AlbumArtDownloader.Properties.WindowSettings.GetWindowSettings(this).SaveWindowState();
			SaveSourceSettings();
			SaveDefaultSaveFolderHistory();

			System.Diagnostics.Trace.WriteLine("done.");
		}
		string IAppWindow.Description
		{
			get
			{
				if (String.IsNullOrEmpty(Artist))
				{
					if (String.IsNullOrEmpty(Album))
					{
						return "Search";
					}
					return "Search: " + Album;
				}
				else if(String.IsNullOrEmpty(Album))
				{
					return "Search: " + Artist;
				}
				return String.Format("Search: {0} / {1}", Artist, Album);
			}
		}
		#endregion

		/// <summary>
		/// Used so that if any of the sources are selected, the search button is enabled
		/// </summary>
		private class NotFalseConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((bool?)value).GetValueOrDefault(true);
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return null; //Not used
			}
		}
	}
}