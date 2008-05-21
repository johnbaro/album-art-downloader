using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Reflection;

namespace AlbumArtDownloader.Controls
{
	internal static class Utilities
	{
		/// <summary>
		/// Exposes the private ComboBox HighlightedItem functionality
		/// to return the highlighted value of a combo box when open.
		/// </summary>
		public static object GetComboBoxHighlightedItem(ComboBox comboBox)
		{
			//TODO: If WPF ever exposes this information properly, avoid using reflection on private members
			try
			{
				PropertyInfo highlightedItemProperty = typeof(ComboBox).GetProperty("HighlightedItem", BindingFlags.Instance | BindingFlags.NonPublic);
				if (highlightedItemProperty != null)
				{
					return highlightedItemProperty.GetValue(comboBox, null);
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Could not access ComboBox.HighlightedItem property using reflection: " + ex.Message);
			}
			return null;
		}

		#region Text Measuring -  Formatted Text Caching
		private static Dictionary<string, FormattedTextInfo> sFormattedTextCache = new Dictionary<string, FormattedTextInfo>();
		
		/// <summary>
		/// Measures the width of the text text, as would be displayed by the given textblock
		/// </summary>
		/// <remarks>Uses a caching system for better performance at the cost of using more memory.</remarks>
		public static double GetTextWidth(string text, TextBlock textBlock, CultureInfo culture)
		{
			if (String.IsNullOrEmpty(text))
				return 0;

			//Most times, text will only appear in the same format, so check if we have a cached copy
			FormattedTextInfo formattedTextInfo;

			//The only thing that does tend to vary on format is Bold/Not Bold, so worth including that in the key.
			string key = text + textBlock.FontWeight.ToString();

			if(sFormattedTextCache.TryGetValue(key, out formattedTextInfo))
			{
				if (!formattedTextInfo.IsEquivalent(culture, textBlock))
				{
					//Same text, different formatting. Must create a new one
					formattedTextInfo = null;
				}
			}
			if(formattedTextInfo == null)
			{
				//Must create a new formatted text
				formattedTextInfo = new FormattedTextInfo(text, culture, textBlock);
				sFormattedTextCache[key] = formattedTextInfo;
			}
			return formattedTextInfo.MeasureWidth();
		}

		/// <summary>
		/// Stores a FormattedText, and the parameters used to create it
		/// </summary>
		private class FormattedTextInfo
		{
			private FormattedText mFormattedText;

			private CultureInfo mCulture;
			private FlowDirection mFlowDirection;
			private Typeface mTypeface;
			private double mFontSize;

			public FormattedTextInfo(string text, CultureInfo culture, TextBlock textBlock)
			{
				mCulture = culture;
				mFlowDirection = textBlock.FlowDirection;
				mTypeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
				mFontSize = textBlock.FontSize;
				
				mFormattedText = new FormattedText(	text, 
													mCulture, 
													mFlowDirection,
													mTypeface,
													mFontSize,
													null);
			}

			
			/// <summary>
			/// Checks whether this formatted text can be used to return information based on the specified TextBlock
			/// </summary>
			public bool IsEquivalent(CultureInfo culture, TextBlock textBlock)
			{
				return
					culture == mCulture &&
					textBlock.FlowDirection == mFlowDirection &&
					textBlock.FontFamily.Equals(mTypeface.FontFamily) &&
					textBlock.FontStyle == mTypeface.Style &&
					textBlock.FontWeight == mTypeface.Weight &&
					textBlock.FontStretch == mTypeface.Stretch &&
					textBlock.FontSize == mFontSize
				;
			}

			public double MeasureWidth()
			{
				return mFormattedText.Width;
			}
		}
		#endregion
	}
}
