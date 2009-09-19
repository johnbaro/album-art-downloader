using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// A group description for grouping by whether the source is Local or Online.
	/// </summary>
	internal class SizeGroupDescription : GroupDescription
	{
		private static readonly int[] sSizeGroupings = new[] { 1000, 700, 540, 400 };
		private static readonly string[] sSizeLabels = new[] { "Extra-Large", "Large", "Medium", "Small", "Extra-Small" };
		private static readonly string[] sSizeGroups = new string[sSizeLabels.Length];

		static SizeGroupDescription()
		{
			for (int i = 0; i < sSizeGroupings.Length; i++)
			{
				sSizeGroups[i] = String.Format(CultureInfo.CurrentUICulture, "{0} (over {1} x {1})", sSizeLabels[i], sSizeGroupings[i]);
			}
			int last = sSizeGroups.Length - 1;
			sSizeGroups[last] = String.Format(CultureInfo.CurrentUICulture, "{0} (below {1} x {1})", sSizeLabels[last], sSizeGroupings[last - 1]);
		}
		public SizeGroupDescription()
		{
			foreach (string sizeGroup in sSizeGroups)
			{
				GroupNames.Add(sizeGroup);
			}
		}

		public override object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert(level == 0, "Multiple levels are not supported");
			AlbumArt albumArt = item as AlbumArt;
			if (albumArt == null)
			{
				System.Diagnostics.Debug.Fail("Expecting to be grouping album art");
				return null;
			}

			double smallerDimension = Math.Min(albumArt.ImageWidth, albumArt.ImageHeight);
			for (int i = 0; i < sSizeGroupings.Length; i++)
			{
				if(smallerDimension > sSizeGroupings[i])
				{
					return sSizeGroups[i];
				}
			}
			return sSizeGroups[sSizeGroupings.Length];
		}
	}
}
