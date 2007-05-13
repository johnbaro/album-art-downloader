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
using System.Threading;
using System.Windows.Threading;
using System.IO;

namespace AlbumArtDownloader
{
	public partial class FileBrowser : System.Windows.Window, IAppWindow
	{
		private Thread mSearchThread;

		public FileBrowser()
		{
			InitializeComponent();
			LoadPathPatternHistory();

			mBrowse.Click += new RoutedEventHandler(OnBrowseForFilePath);

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			SaveSettings();
		}
		protected override void OnClosed(EventArgs e)
		{
			AbortSearch();
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
			Search(mFilePathBox.Text, mIncludeSubfolders.IsChecked.GetValueOrDefault());
		}

		private void StopExec(object sender, RoutedEventArgs e)
		{
			AbortSearch();
		}
		#endregion

		#region Searching
		/// <summary>
		/// Begins an asynchronous search for files.
		/// </summary>
		public void Search(string rootPath, bool includeSubfolders)
		{
			//Ensure UI is synched
			mFilePathBox.Text = rootPath;
			mIncludeSubfolders.IsChecked = includeSubfolders;

			AbortSearch(); //Abort any existing search
			mSearchThread = new Thread(new ParameterizedThreadStart(SearchWorker));
			mSearchThread.Name = "File Browser Search";
			mSearchThread.Start(new SearchThreadParameters(rootPath, includeSubfolders));
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
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				mStop.Visibility = Visibility.Visible;
			}));
			try
			{
				SearchThreadParameters parameters = (SearchThreadParameters)state;
				SetStatusText("Searching...");

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
					SetStatusText(String.Format("Could not search \"{0}\": {1}", parameters.RootPath, e.Message));
					return;
				}

				try
				{
					foreach (FileInfo file in NetMatters.FileSearcher.GetFiles(root, "*", parameters.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
					{
						SetStatusText("Searching... " + file.Name);
						//TODO: Get Album and Artist info from the file, add to results
					}
				}
				catch (ThreadAbortException)
				{
					SetStatusText("Stopped");
					return;
				}
				catch (Exception e)
				{
					SetStatusText(String.Format("Error occurred while searching: {0}", e.Message));
					return;
				}

				SetStatusText("Done");
			}
			finally
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					mStop.Visibility = Visibility.Collapsed;
				}));
			}
		}
		
		/// <summary>
		/// Update the status bar text, safe to call from the search worker thread.
		/// </summary>
		private void SetStatusText(string text)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				mStatus.Content = text;
			}));
		}
		#endregion
	}
}