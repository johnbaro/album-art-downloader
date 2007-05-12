using System.Windows.Input;
using System.Windows;

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
		}

		#region Standard Handlers
		private static void NewSearchWindowExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewSearchWindow(sender as IAppWindow).Show();
		}
		private static void NewFileBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFileBrowser(sender as IAppWindow).Show();
		}
		private static void NewFoobarBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFoobarBrowser(sender as IAppWindow).Show();
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
			//TODO: Validation that this is a web address?
			if (e.Parameter is string)
			{
				System.Diagnostics.Process.Start((string)e.Parameter);
			}
		}
		#endregion
	}
}
