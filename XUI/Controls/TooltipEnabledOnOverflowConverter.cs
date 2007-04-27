using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace AlbumArtDownloader.Controls
{
	internal class TooltipEnabledOnOverflowConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Length == 3 && values[0] is TextBlock && values[1] is double && values[2] is string)
			{
				TextBlock textBlock = (TextBlock)values[0];
				double actualWidth = (double)values[1];
				string text = (string)values[2];

				//Check the size
				FormattedText formattedText = new FormattedText(text, culture, textBlock.FlowDirection, Utilities.GetTypeface(textBlock), textBlock.FontSize, null);
				return formattedText.Width > actualWidth;
			}
			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
