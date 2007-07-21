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

			//ApplicationCommands.Delete cancels a single (selected) search.
			//ApplicationCommands.Stop cancels all searches (clears the queue).
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(DeleteExec), new CanExecuteRoutedEventHandler(DeleteCanExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec), new CanExecuteRoutedEventHandler(StopCanExec)));
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

		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			IList<ArtSearchWindow> searchQueue = ((App)Application.Current).SearchQueue.Queue;
			while (searchQueue.Count > 0)
			{
				RemoveFromQueue(searchQueue[0]);
			}
		}

		private void StopCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ((App)Application.Current).SearchQueue.Queue.Count > 0;
		}

		private void RemoveFromQueue(ArtSearchWindow searchWindow)
		{
			((App)Application.Current).SearchQueue.CancelSearchWindow(searchWindow);
		}
	}
}