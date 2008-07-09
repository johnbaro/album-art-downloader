using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AlbumArtDownloader
{
	/// <summary>
	/// This class holds a queue of search windows that have yet to be displayed or searched
	/// </summary>
	public class SearchQueue: INotifyPropertyChanged
	{
		private QueueManager mManagerWindow;
		private ObservableCollection<ArtSearchWindow> mQueue = new ObservableCollection<ArtSearchWindow>();
		
		public SearchQueue()
		{
			SimulataneousWindowsAllowed = Properties.Settings.Default.NumberOfWindowsForQueue;
		}

		/// <summary>
		/// Enqueues the specified search window. This may mean that the search
		/// window is shown immediately, if there are less than <see cref="SimultaneousWindowsAllowed"/>
		/// windows open already.
		/// </summary>
		public void EnqueueSearchWindow(ArtSearchWindow searchWindow)
		{
			if (NumberOfOpenSearchWindows < SimulataneousWindowsAllowed)
			{
				//Show the window immediately
				ShowSearchWindow(searchWindow);
			}
			else
			{
				//Enqueue it
				mQueue.Add(searchWindow);
			}
		}

		/// <summary>
		/// If the specified search window is in the queue, remove it from the queue without showing it.
		/// Disposes of the search window, if it is removed.
		/// </summary>
		public void CancelSearchWindow(ArtSearchWindow searchWindow)
		{
			if (mQueue.Remove(searchWindow))
			{
				searchWindow.Close();
			}
		}

		/// <summary>
		/// If the specified search window is in the queue, force it to be shown immediately.
		/// </summary>
		public void ForceSearchWindow(ArtSearchWindow searchWindow)
		{
			int index = mQueue.IndexOf(searchWindow);
			if(index >= 0)
			{
				mQueue.RemoveAt(index);
				ShowSearchWindow(searchWindow);
			}
		}

		/// <summary>
		/// Checks to see if a search window is ready for dequeuing, and if so, dequeues it
		/// </summary>
		private void DequeueNextSearchWindow()
		{
			if (Queue.Count > 0)
			{
				while (NumberOfOpenSearchWindows < SimulataneousWindowsAllowed) //Keep dequeueing as long as more simultaneous windows are allowed.
				{
					//Dequeue and show the next window
					ArtSearchWindow searchWindow = Queue[0];
					mQueue.RemoveAt(0);
					ShowSearchWindow(searchWindow);
				}
				if (Queue.Count == 0)
				{
					//Close the manager, if it is open
					if (mManagerWindow != null)
						mManagerWindow.Close();
				}
			}
		}

		/// <summary>
		/// Shows the specified search window, and starts it searching.
		/// The window should have been dequeued before calling this method.
		/// </summary>
		private void ShowSearchWindow(ArtSearchWindow searchWindow)
		{
			searchWindow.LoadSettings(); //Ensure the settings are brought up to date
			searchWindow.Closed += new EventHandler(OnSearchWindowClosed);
			searchWindow.Show();
			NumberOfOpenSearchWindows++;
		}

		private void OnSearchWindowClosed(object sender, EventArgs e)
		{
			NumberOfOpenSearchWindows--;
		}

		private int mNumberOfOpenSearchWindows;
		/// <summary>
		/// The number of search windows that are currently open.
		/// Note that this only includes search windows opened through
		/// <see cref="ShowSearchWindow"/>, not opened by other means.
		/// </summary>
		public int NumberOfOpenSearchWindows
		{
			get { return mNumberOfOpenSearchWindows; }
			set 
			{
				if (value != mNumberOfOpenSearchWindows)
				{
					mNumberOfOpenSearchWindows = value;

					//Check to see if a new window can be opened
					DequeueNextSearchWindow();
				}
			}
		}

		public ObservableCollection<ArtSearchWindow> Queue
		{
			get
			{
				return mQueue;
			}
		}

		/// <summary>
		/// Displays the Queue Manager window to view and manipulate the queue
		/// </summary>
		public void ShowManagerWindow()
		{
			if (mManagerWindow == null)
			{
				mManagerWindow = new QueueManager();
				mManagerWindow.Closed += new EventHandler(OnManagerWindowClosed);
				mManagerWindow.Show();
			}
			else
			{
				//There is already a queue manager window, so just bring it to the front.
				mManagerWindow.Activate();
			}
		}

		private void OnManagerWindowClosed(object sender, EventArgs e)
		{
			mManagerWindow = null; //Reset the manager window variable so that a new window is created next time.
		}

		#region SimulataneousWindowsAllowed
		private int mSimulataneousWindowsAllowed = 1;
		/// <summary>
		/// The number of windows that can be open before further windows are added to the queue rather than shown.
		/// </summary>
		public int SimulataneousWindowsAllowed
		{
			get { return mSimulataneousWindowsAllowed; }
			set
			{
				if (value != mSimulataneousWindowsAllowed)
				{
					if (value < 1)
						throw new ArgumentOutOfRangeException(); //Must allow at least 1 simultaneous window

					mSimulataneousWindowsAllowed = value;

					DequeueNextSearchWindow();
					Properties.Settings.Default.NumberOfWindowsForQueue = mSimulataneousWindowsAllowed;

					NotifyPropertyChanged("SimulataneousWindowsAllowed");
				}
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
	}
}
