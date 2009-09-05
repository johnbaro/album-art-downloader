using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using AlbumArtDownloader.Controls;
using System.Runtime.InteropServices;

namespace AlbumArtDownloader
{
	public partial class BrowserResults : SortableListView, IDisposable
	{
		public static class Commands
		{
			public static RoutedUICommand SelectMissing = new RoutedUICommand("Select Missing", "SelectMissing", typeof(Commands));
			public static RoutedUICommand GetArtwork = new RoutedUICommand("Get Artwork", "GetArtwork", typeof(Commands));
			/// <summary>Displays the file passed in as the parameter to the command in Windows Explorer</summary>
			public static RoutedUICommand ShowInExplorer = new RoutedUICommand("Show in Explorer", "ShowInExplorer", typeof(Commands));
			/// <summary>Displays the file in the preview window</summary>
			public static RoutedUICommand Preview = new RoutedUICommand("Preview", "Preview", typeof(Commands));
			/// <summary>Shows a dialog to rename the file</summary>
			public static RoutedUICommand Rename = new RoutedUICommand("Rename", "Rename", typeof(Commands));
			/// <summary>Deletes the file (to the recycle bin)</summary>
			public static RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof(Commands));
		}

		private ObservableAlbumCollection mAlbums;
		private Thread mArtFileSearchThread;
		private Queue<Album> mArtFileSearchQueue = new Queue<Album>();
		private AutoResetEvent mArtFileSearchTrigger = new AutoResetEvent(false);
		
		public BrowserResults()
		{
			InitializeComponent();

			Albums = new ObservableAlbumCollection(); //Have one by default

			CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(SelectAllExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(DeleteExec)));
			CommandBindings.Add(new CommandBinding(Commands.SelectMissing, new ExecutedRoutedEventHandler(SelectMissingExec)));
			CommandBindings.Add(new CommandBinding(Commands.GetArtwork, new ExecutedRoutedEventHandler(GetArtworkExec), new CanExecuteRoutedEventHandler(GetArtworkCanExec)));
			CommandBindings.Add(new CommandBinding(Commands.ShowInExplorer, new ExecutedRoutedEventHandler(ShowInExplorerExec)));
			CommandBindings.Add(new CommandBinding(Commands.Preview, new ExecutedRoutedEventHandler(PreviewExec)));
			CommandBindings.Add(new CommandBinding(Commands.Delete, new ExecutedRoutedEventHandler(DeleteArtFileExec)));
			CommandBindings.Add(new CommandBinding(Commands.Rename, new ExecutedRoutedEventHandler(RenameArtFileExec)));

			CreateArtFileSearchThread();
		}

		public void Dispose()
		{
			//Close down the search thread.
			if (mArtFileSearchThread != null)
			{
				mArtFileSearchThread.Abort();
				mArtFileSearchThread = null;
			}
		}
		
		#region Command Handlers
		private void DeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			foreach (Album selectedAlbum in new System.Collections.ArrayList(SelectedItems)) //Take copy of selected items list in case it is changed during removal
			{
				Albums.Remove(selectedAlbum);
			}
		}

		private void SelectAllExec(object sender, ExecutedRoutedEventArgs e)
		{
			AllSelected = !AllSelected.GetValueOrDefault(true); //Mimic behaviour of clicking on the checkbox.
		}

		private void SelectMissingExec(object sender, ExecutedRoutedEventArgs e)
		{
			SelectAll(); //Adding items to the selection programatically is irritatingly difficult, so remove them instead.
			for (int i = 0; i < Items.Count; i++)
			{
				Album album = (Album)Items[i];
				if (album.ArtFileStatus != ArtFileStatus.Missing)
				{
					SelectedItems.Remove(album);
				}
			}
		}

		private void GetArtworkCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = SelectedItems.Count > 0; //Can't execute if there aren't any selected items.
		}

		/// <summary>
		/// When creating new multiple search windows, offset each by this amount so that they aren't all on top of each other.
		/// </summary>
		private static readonly int sSearchWindowCascadeOffset = 20;
		private void GetArtworkExec(object sender, ExecutedRoutedEventArgs e)
		{
			//Warn if there are a lot of selected items
			if (SelectedItems.Count > Properties.Settings.Default.EnqueueWarning)
			{
				EnqueueWarning enqueueWarning = new EnqueueWarning();
				enqueueWarning.Owner = Window.GetWindow(this);
				enqueueWarning.NumberToEnqueue = SelectedItems.Count;

				if (!enqueueWarning.ShowDialog().GetValueOrDefault())
				{
					//Cancelled
					return;
				}

				//Trim the selection back to the number to enqueue
				while (SelectedItems.Count > enqueueWarning.NumberToEnqueue)
				{
					SelectedItems.RemoveAt(SelectedItems.Count - 1);
				}
			}

			//The art file search pattern is used as the default path to save the found image to, but first:
			// *In case of alternates, use the first alternate only.
			string artFileSearchPattern = ImagePathPattern.Split(new[] {'|'}, 2)[0];
			// *Don't substitute placeholders, but do substitute recursive path matching with the simplest solution to it, just putting saving to the immediate subfolder
			artFileSearchPattern = artFileSearchPattern.Replace("**\\", "").Replace("**/", "");
			// *Also replace a wildcarded extension
			if (artFileSearchPattern.EndsWith(".*"))
			{
				artFileSearchPattern = artFileSearchPattern.Substring(0, artFileSearchPattern.Length - 2) + ".%extension%";
			}
			// *If the pattern ends in just a wildcard, replace with %name%.%extension%
			else if (artFileSearchPattern.EndsWith("*"))
			{
				artFileSearchPattern = artFileSearchPattern.Substring(0, artFileSearchPattern.Length - 1) + "%name%.%extension%";
			}
			//Replace other wildcards with the %name%, so that for local files search they become wildcards again
			artFileSearchPattern = artFileSearchPattern.Replace("*", "%name%");
			int i = 0;
			foreach (Album album in SelectedItems)
			{
				//If the image path is relative, get an absolute path for it.
				string rootedArtFileSearchPattern;
				if (Path.IsPathRooted(artFileSearchPattern))
				{
					rootedArtFileSearchPattern = artFileSearchPattern;
				}
				else
				{
					rootedArtFileSearchPattern = Path.Combine(album.BasePath, artFileSearchPattern);
				}

				ArtSearchWindow searchWindow = Common.NewSearchWindow(Window.GetWindow(this) as IAppWindow);
				searchWindow.Top += i * sSearchWindowCascadeOffset;
				searchWindow.Left += i * sSearchWindowCascadeOffset;

				//TODO: Neater laying out of windows which would go off the screen. Note how Firefox handles this, for example, when opening lots of new non-maximised windows.
				//TODO: Multimonitor support.
				if (searchWindow.Left + searchWindow.Width > SystemParameters.PrimaryScreenWidth)
				{
					//For the present, just make sure that the window doesn't leave the screen.
					searchWindow.Left = SystemParameters.PrimaryScreenWidth - searchWindow.Width;
				}
				if (searchWindow.Top + searchWindow.Height > SystemParameters.PrimaryScreenHeight)
				{
					searchWindow.Top = SystemParameters.PrimaryScreenHeight - searchWindow.Height;
				}

				searchWindow.SetDefaultSaveFolderPattern(rootedArtFileSearchPattern, true); //Default save to the location where the image was searched for.
				searchWindow.Search(album.Artist, album.Name); //Kick off the search.

				//Watch for the window being closed to update the status of the artwork
				mSearchWindowAlbumLookup.Add(searchWindow, album);
				searchWindow.Closed += OnSearchWindowClosed;

				i++;
			}
		}

		private Dictionary<ArtSearchWindow, Album> mSearchWindowAlbumLookup = new Dictionary<ArtSearchWindow, Album>();
		private void OnSearchWindowClosed(object sender, EventArgs e)
		{
			Album album;
			var searchWindow = sender as ArtSearchWindow;
			if(mSearchWindowAlbumLookup.TryGetValue(searchWindow, out album))
			{
				searchWindow.Closed -= OnSearchWindowClosed;
				mSearchWindowAlbumLookup.Remove(searchWindow);

				QueueAlbumForArtFileSearch(album);
			}
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			
			DependencyObject parent = e.OriginalSource as DependencyObject;
			while (parent != null)
			{
				if (parent is ListViewItem)
				{
					//A list item was double clicked on, so get artwork for it
					e.Handled = true;
					System.Diagnostics.Debug.Assert(SelectedItems.Count == 1, "Expecting only the double clicked item to be selected");
					GetArtworkExec(null, null);
					return;
				}
				else if (parent == this)
				{
					//A list item was not double clicked on, something else was
					break;
				}
				parent = VisualTreeHelper.GetParent(parent);
			}
			//Do nothing for double click happening elsewhere.
		}

		private void ShowInExplorerExec(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter is string)
			{
				//TODO: Validation that this is a file path?
				System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + (string)e.Parameter + "\"");
			}
		}

		private void PreviewExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				LocalFilesSource source = new LocalFilesSource()
				{
					SearchPathPattern = filePath,
					DefaultFilePath = "",
					MaximumResults = 1
				};
				source.SearchCompleted += new EventHandler(delegate {
					if (source.Results.Count > 0)
					{
						var previewWindow = Common.NewPreviewWindow(Window.GetWindow(this) as IAppWindow);
						var albumArt = source.Results[0];
						albumArt.FilePath = filePath;
						previewWindow.AlbumArt = albumArt;
					}
					else
					{
						System.Diagnostics.Trace.TraceError("Could not obtain AlbumArt for local file: " + source.SearchPathPattern);
					}
				});
				source.Search(null, null);
				
				//AlbumArt preview = new AlbumArt(null, 
			}
		}

		private void DeleteArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				DeleteFileToRecycleBin(filePath);

				CheckForChangedArt(e);
			}
		}

		private void RenameArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				var renameWindow = new RenameArt(filePath);
				renameWindow.Owner = Window.GetWindow(this);
				if (renameWindow.ShowDialog().GetValueOrDefault())
				{
					CheckForChangedArt(e);
				}
			}
		}

		//Pass in the ExecutedRoutedEventArgs from the Exec method to check for changed artwork as a result of that command
		private void CheckForChangedArt(ExecutedRoutedEventArgs e)
		{
			var item = e.OriginalSource as ListViewItem;
			if (item != null)
			{
				var album = item.Content as Album;
				if (album != null)
				{
					QueueAlbumForArtFileSearch(album);
				}
			}
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
							SelectAll();
						}
						else
						{
							SelectedItems.Clear();
						}
						mSettingAllSelected = false;
					}
					mAllSelected = value;
					NotifyPropertyChanged("AllSelected");
				}
			}
		}

		protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
		
			if (!mSettingAllSelected) //Ignore selection change while setting selection from AllSelected setter.
			{
				if (e.RemovedItems.Count > 0)
				{
					//Check to see if there is now nothing selected
					if (SelectedItems.Count == 0)
					{
						mAllSelected = false; //Don't change through the accessor, so it doesn't bother trying to reapply the selection
						NotifyPropertyChanged("AllSelected");
						return;
					}
				}
				if (e.AddedItems.Count > 0)
				{
					//Check to see if there is now all selected
					if (SelectedItems.Count == Items.Count)
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

		#region Properties
		private string mImagePathPattern;
		public string ImagePathPattern 
		{
			get
			{
				return mImagePathPattern;
			}
			set
			{
				if (value != mImagePathPattern)
				{
					mImagePathPattern = value;
					NotifyPropertyChanged("ImagePathPattern");
				}
			}
		}

		#region State
		private BrowserState mState = BrowserState.Ready;
		public BrowserState State
		{
			get
			{
				return mState;
			}
			private set
			{
				if (value != mState)
				{
					mState = value;
					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						OnStateChanged(EventArgs.Empty);
						NotifyPropertyChanged("State");
					}));
				}
			}
		}
		public event EventHandler StateChanged;
		protected virtual void OnStateChanged(EventArgs e)
		{
			EventHandler temp = StateChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}
		#endregion

		#region ProgressText
		private string mProgressText;
		public string ProgressText
		{
			get
			{
				return mProgressText;
			}
			private set
			{
				if (value != mProgressText)
				{
					mProgressText = value;
					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						OnProgressTextChanged(EventArgs.Empty);
						NotifyPropertyChanged("ProgressText");
					}));
				}
			}
		}
		public event EventHandler ProgressTextChanged;
		protected virtual void OnProgressTextChanged(EventArgs e)
		{
			EventHandler temp = ProgressTextChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}
		#endregion

		internal ObservableAlbumCollection Albums
		{
			get
			{
				return mAlbums;
			}
			set
			{
				if (value != mAlbums)
				{
					if (mAlbums != null)
					{
						mAlbums.CollectionChanged -= OnAlbumsCollectionChanged;
					}
					mAlbums = value;
					if (mAlbums != null)
					{
						mAlbums.CollectionChanged += OnAlbumsCollectionChanged;
					}

					ItemsSource = mAlbums;

					//Reset art file search queue to the new albums
					ClearArtFileSearchQueue();
					foreach (Album album in mAlbums)
					{
						QueueAlbumForArtFileSearch(album);
					}
					NotifyPropertyChanged("Albums");
				}
			}
		}
		#endregion

		#region ArtFile Searching
		public void AbortSearch()
		{
			ClearArtFileSearchQueue();

			//Restart the art file search thread
			mArtFileSearchThread.Abort();
			mArtFileSearchThread = null;
			CreateArtFileSearchThread();
		}

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
				case NotifyCollectionChangedAction.Remove:
					foreach (Album album in e.OldItems)
					{
						CancelQueuedAlbum(album);
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

		private void CancelQueuedAlbum(Album album)
		{
			album.ArtFileStatus = ArtFileStatus.Unknown;
			lock (mArtFileSearchQueue)
			{
				if (mArtFileSearchQueue.Contains(album))
				{
					Album[] queue = mArtFileSearchQueue.ToArray();
					mArtFileSearchQueue.Clear();
					foreach (Album queuedAlbum in queue)
					{
						if (queuedAlbum != album)
						{
							mArtFileSearchQueue.Enqueue(queuedAlbum);
						}
					}
				}
			}
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

					State = BrowserState.FindingArt;

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
							ProgressText = String.Format("Finding art... {0} / {1}", album.Artist, album.Name);
							
							foreach (string imagePathPatternAlternate in ImagePathPattern.Split('|'))
							{
								string artFileSearchPattern = Common.SubstitutePlaceholders(imagePathPatternAlternate, album.Artist, album.Name);

								if (!Path.IsPathRooted(artFileSearchPattern))
								{
									artFileSearchPattern = Path.Combine(album.BasePath, artFileSearchPattern);
								}
								foreach (string artFile in Common.ResolvePathPattern(artFileSearchPattern))
								{
									album.ArtFile = artFile;
									album.ArtFileStatus = ArtFileStatus.Present;

									try
									{
										album.ArtFileSize = new FileInfo(artFile).Length;
									}
									catch (Exception)
									{
										//Ignore exceptions when reading the filesize it's not important
										album.ArtFileSize = 0;
									}

									//Attempt to get the image dimesions
									try
									{
										using (var fileStream = File.OpenRead(artFile))
										{
											var bitmapDecoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
											album.ArtFileWidth = bitmapDecoder.Frames[0].PixelWidth;
											album.ArtFileHeight = bitmapDecoder.Frames[0].PixelHeight;
										}
									}
									catch (Exception)
									{
										//Ignore exceptions when reading the dimensions, they aren't important
										album.ArtFileWidth = album.ArtFileHeight = 0;
									}

									break; //Only use the first art file that matches, if there are multiple matches.
								}
								//If a matching art file is found, don't search further alternates
								if (album.ArtFileStatus == ArtFileStatus.Present)
								{
									break;
								}
							}
						}
						catch (Exception)
						{
							album.ArtFileStatus = ArtFileStatus.Unknown; //It might not be missing, we just haven't found it before hitting an exception
						}
						if (album.ArtFileStatus != ArtFileStatus.Present) //If it wasn't found, then it's missing.
							album.ArtFileStatus = ArtFileStatus.Missing;
					} while (true);

					State = BrowserState.Done;
				} while (true);
			}
			catch (ThreadAbortException)
			{
				State = BrowserState.Stopped;
				return;
			}
		}
		#endregion
	
		#region Delete to Recycle Bin
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto, Pack=1)] 
		private struct SHFILEOPSTRUCT 
		{ 
			public IntPtr hwnd; 
			[MarshalAs(UnmanagedType.U4)] public int wFunc; 
			public string pFrom; 
			public string pTo; 
			public short fFlags; 
			[MarshalAs(UnmanagedType.Bool)] public bool fAnyOperationsAborted; 
			public IntPtr hNameMappings; 
			public string lpszProgressTitle; 
		} 

		[DllImport("shell32.dll", CharSet=CharSet.Auto)] 
		private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp); 
		private const int FO_DELETE = 3; 
		private const int FOF_ALLOWUNDO = 0x40; 
		//private const int FOF_NOCONFIRMATION = 0x10;    //No prompt dialogs 

		private static void DeleteFileToRecycleBin(string filePath)
		{
			//Useage
			SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT();
			shf.wFunc = FO_DELETE;
			shf.fFlags = FOF_ALLOWUNDO;
			shf.pFrom = filePath + "\0";

			SHFileOperation(ref shf);
		}
		#endregion
	}	
}
