using System;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Interaction logic for Menu.xaml
	/// </summary>
	public partial class Menu : System.Windows.Controls.Border
	{
		public static class Commands
		{
			public static RoutedUICommand NewFileBrowser = new RoutedUICommand("New File Browser...", "NewFileBrowser", typeof(Commands), FileInputGestures());
			private static InputGestureCollection FileInputGestures()
			{
				InputGestureCollection gestures = new InputGestureCollection();
				gestures.Add(new KeyGesture(Key.B, ModifierKeys.Control));
				return gestures;
			}

			public static RoutedUICommand NewFoobarBrowser = new RoutedUICommand("New Foobar Browser...", "NewFoobarBrowser", typeof(Commands));
			public static RoutedUICommand About = new RoutedUICommand("About...", "About", typeof(Commands));
			public static RoutedUICommand Exit = new RoutedUICommand("E_xit", "Exit", typeof(Commands));
		}

		static Menu()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(NewSearchWindowExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.NewFileBrowser, new ExecutedRoutedEventHandler(NewFileBrowserExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.NewFoobarBrowser, new ExecutedRoutedEventHandler(NewFoobarBrowserExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.Exit, new ExecutedRoutedEventHandler(ExitExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.About, new ExecutedRoutedEventHandler(AboutExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(NavigationCommands.GoToPage, new ExecutedRoutedEventHandler(GoToPageExec)));
		}

		public Menu()
		{
			InitializeComponent();

#if EPHEMERAL_SETTINGS
			this.Background = System.Windows.Media.Brushes.Red;
#endif
		}

		private void OnWindowListOpened(object sender, RoutedEventArgs e)
		{
			//Populate the list with all the windows
			MenuItem windowList = sender as MenuItem;
			if (windowList != null)
			{
				windowList.Items.Clear();
				int iWindow = 0;
				IAppWindow thisWindow = Window.GetWindow(windowList) as IAppWindow;
				
				if(thisWindow != null)
				{
					//Add this window as the first in the list.
					MenuItem thisWindowMenuItem = CreateWindowMenuItem(thisWindow, ++iWindow);
					thisWindowMenuItem.IsChecked = true; //This is the active window
					windowList.Items.Add(thisWindowMenuItem);
				}

				foreach (Window window in Application.Current.Windows)
				{
					if (window is IAppWindow && window != thisWindow) //Don't add this window again, it's already added.
					{
						windowList.Items.Add(CreateWindowMenuItem((IAppWindow)window, ++iWindow));
					}
				}
			}
		}

		private MenuItem CreateWindowMenuItem(IAppWindow window, int windowIndex)
		{
			MenuItem windowMenuItem = new MenuItem();
			windowMenuItem.Header = String.Format("_{0} {1}", windowIndex, window.Description);
			windowMenuItem.IsCheckable = true;
			windowMenuItem.Command = NavigationCommands.GoToPage;
			windowMenuItem.CommandParameter = window;
			return windowMenuItem;
		}

		#region Standard Handlers
		private static void NewSearchWindowExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewSearchWindow(sender as IAppWindow);
		}
		private static void NewFileBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFileBrowser(sender as IAppWindow);
		}
		private static void NewFoobarBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFoobarBrowser(sender as IAppWindow);
		}
		private static void CloseExec(object sender, ExecutedRoutedEventArgs e)
		{
			Window window = sender as Window;
			if(window != null)
				window.Close();
		}
		private static void ExitExec(object sender, ExecutedRoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
		private static void AboutExec(object sender, ExecutedRoutedEventArgs e)
		{
			About about = new About();
			about.Owner = sender as Window;
			about.ShowDialog();
		}
		private static void GoToPageExec(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter is string)
			{
				//TODO: Validation that this is a web address?
				System.Diagnostics.Process.Start((string)e.Parameter);
			}
			else if (e.Parameter is Window)
			{
				((Window)e.Parameter).Activate();
			}
		}
		#endregion
	}
}
