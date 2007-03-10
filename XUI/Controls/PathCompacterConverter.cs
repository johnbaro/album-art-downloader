using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Controls;

namespace AlbumArtDownloader.Controls
{

	/// <summary>
	/// Compacts a file path to fit the size available
	/// </summary>
	internal class PathCompacterConverter : IMultiValueConverter
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
		static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TextBlock textBlock = (TextBlock)values[0];
			double actualWidth = (double)values[1]; //Included seperately so that updates are triggered when the width changes
			string path = (string)values[2];

			Typeface textBlockTypeFace = Utilities.GetTypeface(textBlock);
			
			StringBuilder result = new StringBuilder(path);
			int maxChars = path.Length; //Start at full length, and shrink to fit
			do
			{
				//Check the size
				FormattedText formattedText = new FormattedText(result.ToString(), culture, textBlock.FlowDirection, textBlockTypeFace, textBlock.FontSize, null);
				if (formattedText.Width <= actualWidth)
				{
					//Small enough.
					return result.ToString();
				}
				//Not small enough, so shrink by one
				PathCompactPathEx(result, path, --maxChars, 0);
			} while (maxChars > 0);
			
			return result.ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
