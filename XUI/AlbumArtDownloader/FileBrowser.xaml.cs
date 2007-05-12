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

namespace AlbumArtDownloader
{
	public partial class FileBrowser : System.Windows.Window, IAppWindow
	{
		public FileBrowser()
		{
			InitializeComponent();
			LoadPathPatternHistory();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			SavePathPatternHistory();
		}

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

		private void FindExec(object sender, RoutedEventArgs e)
		{
			mImagePathPatternBox.AddPatternToHistory();
			//TODO: Kick off asynch search here.
		}

		public void SaveSettings()
		{
			SavePathPatternHistory();
		}
	}
}