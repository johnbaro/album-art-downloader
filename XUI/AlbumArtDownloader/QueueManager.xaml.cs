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
	public partial class QueueManager : System.Windows.Window
	{
		public QueueManager()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(DeleteExec), new CanExecuteRoutedEventHandler(DeleteCanExec)));
		}

		private void DeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter is ArtSearchWindow)
			{
				//Remove the specified item
				RemoveFromQueue((ArtSearchWindow)e.Parameter);
			}
			else
			{
				//Remove all the selected items
				foreach (ArtSearchWindow searchWindow in new System.Collections.ArrayList(mQueueDisplay.SelectedItems))
				{
					RemoveFromQueue(searchWindow);
				}
			}
		}

		private void DeleteCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			if (e.Parameter is ArtSearchWindow || mQueueDisplay.SelectedItems.Count > 0)
				e.CanExecute = true; //Can execute if there is a specific item to delete, or if there is a selection to delete.
			else
				e.CanExecute = false;
		}

		private void RemoveFromQueue(ArtSearchWindow searchWindow)
		{
			((App)Application.Current).SearchQueue.CancelSearchWindow(searchWindow);
		}

		
	}
}