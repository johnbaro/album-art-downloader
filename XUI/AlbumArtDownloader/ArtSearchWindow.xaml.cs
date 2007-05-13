using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using AlbumArtDownloader.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Threading;
using AlbumArtDownloader.Scripts;


namespace AlbumArtDownloader
{
	public partial class ArtSearchWindow : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		private Sources mSources = new Sources();

		private Thread mAutoDownloadFullSizeImagesThread;
		private AutoResetEvent mAutoDownloadFullSizeImagesTrigger = new AutoResetEvent(true);
		private Stack<AlbumArt> mResultsToAutoDownloadFullSizeImages = new Stack<AlbumArt>();
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
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, new ExecutedRoutedEventHandler(SaveAsExec)));

			//Stop All is bound only when doing a search (so the Stop All button only appears while searching)
			mStopAllCommandBinding = new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec));
			
			mSourcesViewer.ItemsSource = mSources;
			mResultsViewer.ItemsSource = mSources.CombinedResults;

			foreach (IScript script in ((App)Application.Current).Scripts)
			{
				mSources.Add(new ScriptSource(script));
			}
			mSources.Add(new LocalFilesSource());

			LoadSourceSettings();
			LoadDefaultSaveFolderHistory();
			//Initial value of AutoClose is taken from settings. May be overriden by command line parameters
			AutoClose = Properties.Settings.Default.AutoClose;

			this.Loaded += new RoutedEventHandler(OnLoaded);
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
							resultToProcess = mResultsToAutoDownloadFullSizeImages.Pop();
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

		private AutoResetEvent mWaitForImage = new AutoResetEvent(false);
		private void FullSizeImageDownloadCompleted(object sender)
		{
			mWaitForImage.Set();
			//Causes a refresh. Note that .Refresh doesn't.
			mResultsViewer.Items.Filter = mResultsViewer.Items.Filter;
		}

		private void OnAutoDownloadFullSizeImagesChanged(object sender, RoutedEventArgs e)
		{
			mAutoDownloadFullSizeImagesTrigger.Set();
		}

		private void AddResultToAutoDownloadFullSizeImage(AlbumArt result)
		{
			lock(mResultsToAutoDownloadFullSizeImages)
			{
				mResultsToAutoDownloadFullSizeImages.Push(result);
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
		private SearchParameters mSearchParameters;
		private void FindExec(object sender, RoutedEventArgs e)
		{
			//Check to see whether current results can be altered, or a new search is required
			if (mSearchParameters != null)
			{
				//Can only enhance if the artist and album to search for are identical
				if (mArtist.Text == mSearchParameters.Artist && mAlbum.Text == mSearchParameters.Album)
				{
					AlterSearch();
					return;
				}
			}
			if (mSources.CombinedResults.Count > 0 && Properties.Settings.Default.OpenResultsInNewWindow)
			{
				Common.NewSearchWindow(this).Search(mArtist.Text, mAlbum.Text);
			}
			else
			{
				StartSearch();
			}
		}

		/// <summary>
		/// Perform a search with the specified settings
		/// </summary>
		public void Search(string artist, string album)
		{
			mArtist.Text = artist;
			mAlbum.Text = album;
			StartSearch();
		}

		/// <summary>
		/// Starts a search with the current settings
		/// </summary>
		private void StartSearch()
		{
			mSearchParameters = new SearchParameters(mArtist.Text, mAlbum.Text);

			mDefaultSaveFolder.AddPatternToHistory();
			foreach (Source source in mSources)
			{
				if (source.IsEnabled)
				{
					source.SearchCompleted += OnSourceSearchCompleted; //Hook the complete event to know when to hide the Stop All button
					source.Search(mSearchParameters.Artist, mSearchParameters.Album);
					mSearchParameters.AddSource(source);
				}
				else
				{
					source.AbortSearch();
					source.Results.Clear();
				}
			}
			CommandBindings.Add(mStopAllCommandBinding);
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
						source.SearchCompleted += OnSourceSearchCompleted; //Hook the complete event to know when to hide the Stop All button
						source.Search(mArtist.Text, mAlbum.Text);

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
			defaultPathBinding.ConverterParameter = new string[] { mArtist.Text, mAlbum.Text };
			BindingOperations.SetBinding(art, AlbumArt.DefaultFilePathPatternProperty, defaultPathBinding);
		}

		private class AlbumArtDefaultFilePathPatternSubstitution : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				string pattern = ((string)value).Replace("%artist%", "{0}").Replace("%album%", "{1}");
				return String.Format(pattern, (string[])parameter);
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
			mAutoDownloadFullSizeImagesThread.Abort();
			foreach (Source source in mSources)
			{
				source.AbortSearch();
			}
			base.OnClosed(e);
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
		/// Pass null to set as unspecified.
		/// </summary>
		/// <param name="path"></param>
		public void SetLocalImagesPath(string path)
		{
			LocalFilesSource localFilesSource = null;
			foreach (Source source in mSources)
			{
				localFilesSource = source as LocalFilesSource;
				if (localFilesSource != null) //TODO: Could there ever be more than one local files source?
					break;
			}

			if (localFilesSource != null)
			{
				if (String.IsNullOrEmpty(path))
				{
					localFilesSource.UseSearchPathPattern = false;
				}
				else
				{
					localFilesSource.UseSearchPathPattern = true;
					localFilesSource.SearchPathPattern = path;
				}
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
		private void LoadDefaultSaveFolderHistory()
		{
			mDefaultSaveFolder.History.Clear();
			foreach (string historyItem in Properties.Settings.Default.DefaultSavePathHistory)
			{
				mDefaultSaveFolder.History.Add(historyItem);
			}
		}
		private void SaveDefaultSaveFolderHistory()
		{
			Properties.Settings.Default.DefaultSavePathHistory.Clear();
			foreach (string historyItem in mDefaultSaveFolder.History)
			{
				Properties.Settings.Default.DefaultSavePathHistory.Add(historyItem);
			}
		}
		public void SetDefaultSaveFolderPattern(string path)
		{
			mDefaultSaveFolder.AddPatternToHistory(); //Save the previous value
			Properties.Settings.Default.DefaultSavePath = path; //Set the new value
		}
		#endregion

		#region Stop All
		private void OnSourceSearchCompleted(object sender, EventArgs e)
		{
			((Source)sender).SearchCompleted -= OnSourceSearchCompleted; //Unhook the event, we only want one notification.
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
			//Stop all the sources (asynch, to avoid locking up UI)
			foreach (Source source in mSources)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(AbortSource), source);
			}
		}
		/// <summary>
		/// Delegate to asynchronously abort the source
		/// </summary>
		private void AbortSource(object source)
		{
			((Source)source).AbortSearch();
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

		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = GetAlbumArt(e);
			if (albumArt != null)
			{
				if (AutoClose)
				{
					//The save operation is asynchronous, so connect the handler to watch for the save completing
					albumArt.PropertyChanged += AutoCloseOnSave;
				}

				albumArt.Save();
			}
		}

		private void SaveAsExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = GetAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.PropertyChanged -= AutoCloseOnSave; //No auto-close for SaveAs operation.

				albumArt.SaveAs();
			}
		}

		private AlbumArt GetAlbumArt(ExecutedRoutedEventArgs e)
		{
			FrameworkElement source = e.OriginalSource as FrameworkElement;
			if (!(source is Controls.ArtPanel)) //If the source isn't the panel itself, then it must come from some control in the panels template.
				source = source.TemplatedParent as Controls.ArtPanel;

			if (source == null)
				return null; //Couldn't find an art panel that triggered this command.

			return (AlbumArt)mResultsViewer.ItemContainerGenerator.ItemFromContainer(source.TemplatedParent);
		}

		#region IAppWindow
		public void SaveSettings()
		{
			SaveSourceSettings();
			SaveDefaultSaveFolderHistory();
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