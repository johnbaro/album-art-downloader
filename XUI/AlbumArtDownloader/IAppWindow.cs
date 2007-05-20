using System;
using System.Collections.Generic;
using System.Text;

namespace AlbumArtDownloader
{
	/// <summary>
	/// A main application window, such as a search window or browser.
	/// Applications implementing this interface will appear in the Windows menu
	/// </summary>
	internal interface IAppWindow
	{
		/// <summary>
		/// Save any outstanding values to settings.
		/// </summary>
		void SaveSettings();

		double Left { get; set; }
		double Top { get; set; }

		void Show();
		string Description { get; }
	}
}
