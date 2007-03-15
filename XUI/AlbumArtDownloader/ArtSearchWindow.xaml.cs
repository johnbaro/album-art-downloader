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
	public partial class ArtSearchWindow : System.Windows.Window
	{
		public static class Commands
		{
			public static RoutedUICommand NewFileBrowser = new RoutedUICommand("New File Browser...", "NewFileBrowser", typeof(Commands));
			public static RoutedUICommand NewFoobarBrowser = new RoutedUICommand("New Foobar Browser...", "NewFoobarBrowser", typeof(Commands));
			public static RoutedUICommand About = new RoutedUICommand("About...", "About", typeof(Commands));
		}

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

			//Menu commands:
			CommandBindings.Add(new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(NewSearchWindowExec)));
			CommandBindings.Add(new CommandBinding(Commands.NewFileBrowser, new ExecutedRoutedEventHandler(NewFileBrowserExec)));
			CommandBindings.Add(new CommandBinding(Commands.NewFoobarBrowser, new ExecutedRoutedEventHandler(NewFoobarBrowserExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseExec)));
			CommandBindings.Add(new CommandBinding(Commands.About, new ExecutedRoutedEventHandler(AboutExec)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, new ExecutedRoutedEventHandler(GoToPageExec)));

			//Other commands:
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

			LoadDisabledSources();
			LoadDefaultSaveFolderHistory();

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

		#region New Window
		private void NewSearchWindowExec(object sender, ExecutedRoutedEventArgs e)
		{
			NewSearchWindow();
		}
		private ArtSearchWindow NewSearchWindow()
		{
			//Save these values to settings so that the new window picks up on them
			SaveDisabledSources();
			SaveDefaultSaveFolderHistory();

			ArtSearchWindow newWindow = new ArtSearchWindow();
			//Move the window a little, so that it is obvious it is a new window
			newWindow.Left = Left + 40;
			newWindow.Top = Top + 40;

			newWindow.Show();
			return newWindow;
		}

		private void NewFileBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			new FileBrowser().Show();
		}
		private void NewFoobarBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			new FoobarBrowser().Show();
		}
		#endregion

		#region Help Menu Commands
		private void AboutExec(object sender, ExecutedRoutedEventArgs e)
		{
			About about = new About();
			about.Owner = this;
			about.ShowDialog();
		}
		private void GoToPageExec(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Validation that this is a web address?
			if (e.Parameter is string)
			{
				System.Diagnostics.Process.Start((string)e.Parameter);
			}
		}
		#endregion

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
		private void FindExec(object sender, RoutedEventArgs e)
		{
			if (mSources.CombinedResults.Count > 0 && Properties.Settings.Default.OpenResultsInNewWindow)
			{
				NewSearchWindow().Search(mArtist.Text, mAlbum.Text);
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
			mDefaultSaveFolder.AddPatternToHistory();
			foreach (Source source in mSources)
			{
				if (source.IsEnabled)
				{
					source.SearchCompleted += OnSourceSearchCompleted; //Hook the complete event to know when to hide the Stop All button
					source.Search(mArtist.Text, mAlbum.Text);
				}
				else
				{
					source.AbortSearch();
					source.Results.Clear();
				}
			}
			CommandBindings.Add(mStopAllCommandBinding);
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
		/// </summary>
		public bool AutoClose
		{
			get { return mAutoClose; }
			set { mAutoClose = value; }
		}
		private void AutoCloseOnSave(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSaved" && ((AlbumArt)sender).IsSaved)
			{
				Close();
			}
		}

		private void CloseExec(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			SaveDefaultSaveFolderHistory();
			SaveDisabledSources();
			Properties.Settings.Default.Save();
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

		#region Disabling Sources
		private void LoadDisabledSources()
		{
			foreach (Source source in mSources)
			{
				if (Properties.Settings.Default.DisabledSources.Contains(source.Name))
					source.IsEnabled = false;
			}
		}
		private void SaveDisabledSources()
		{
			Properties.Settings.Default.DisabledSources.Clear();
			foreach (Source source in mSources)
			{
				if (!source.IsEnabled)
					Properties.Settings.Default.DisabledSources.Add(source.Name);
			}
		}

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
			//Stop all the sources
			foreach (Source source in mSources)
			{
				source.AbortSearch();
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