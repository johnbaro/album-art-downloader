using System;
using System.Windows.Data;
using System.IO;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Returns just the filename of a file path
	/// </summary>
	public class FileNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is string)
				return Path.GetFileName((string)value);

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
