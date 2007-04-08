using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace AlbumArtDownloader
{
	public class SourceSettings : ApplicationSettingsBase
	{
		public SourceSettings(string sourceName) : base(sourceName)
		{
		}

		[DefaultSettingValueAttribute("True")]
		[UserScopedSetting]
		public bool Enabled
		{
			get
			{
				return ((bool)(this["Enabled"]));
			}
			set
			{
				this["Enabled"] = value;
			}
		}

		[DefaultSettingValueAttribute("10")]
		[UserScopedSetting]
		public int MaximumResults
		{
			get
			{
				return ((int)(this["MaximumResults"]));
			}
			set
			{
				this["MaximumResults"] = value;
			}
		}

		[DefaultSettingValueAttribute("False")]
		[UserScopedSetting]
		public bool UseMaximumResults
		{
			get
			{
				return ((bool)(this["UseMaximumResults"]));
			}
			set
			{
				this["UseMaximumResults"] = value;
			}
		}
	}
}