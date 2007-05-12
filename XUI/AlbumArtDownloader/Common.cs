using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AlbumArtDownloader
{
	internal static class Common
	{
		public static ArtSearchWindow NewSearchWindow()
		{
			return NewSearchWindow(null);
		}
		public static ArtSearchWindow NewSearchWindow(IAppWindow existingWindow)
		{
			return (ArtSearchWindow)ShowNewWindow(new ArtSearchWindow(), existingWindow);
		}

		public static FileBrowser NewFileBrowser()
		{
			return NewFileBrowser(null);
		}
		public static FileBrowser NewFileBrowser(IAppWindow existingWindow)
		{
			return (FileBrowser)ShowNewWindow(new FileBrowser(), existingWindow);
		}

		public static FoobarBrowser NewFoobarBrowser()
		{
			return NewFoobarBrowser(null);
		}
		public static FoobarBrowser NewFoobarBrowser(IAppWindow existingWindow)
		{
			return (FoobarBrowser)ShowNewWindow(new FoobarBrowser(), existingWindow);
		}

		private static IAppWindow ShowNewWindow(IAppWindow newWindow, IAppWindow oldWindow)
		{
			if (oldWindow != null)
			{
				//Save values to settings so that the new window picks up on them
				oldWindow.SaveSettings();

				//Move the window a little, so that it is obvious it is a new window
				newWindow.Left = oldWindow.Left + 40;
				newWindow.Top = oldWindow.Top + 40;
			}

			newWindow.Show();
			return newWindow;
		}
	}
}
