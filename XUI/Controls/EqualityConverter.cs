using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Checks if the values are equal to the first value, and returns true if they are.
	/// </summary>
	internal class EqualityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Length < 2)
				return false;

			for (int i = 1; i < values.Length; i++)
			{
				if (!values[0].Equals(values[i]))
					return false;
			}
			return true;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
