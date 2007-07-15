using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Controls;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Windows;

namespace AlbumArtDownloader
{
	internal class LocalFilesSource : Source, IWeakEventListener
	{
		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int GdipCreateBitmapFromFile(string filename, out IntPtr bitmap);
		[DllImport("gdiplus.dll", ExactSpelling = true)]
		private static extern int GdipDisposeImage(HandleRef image);

		public LocalFilesSource()
		{
			//Ensure GDI+ is initialised
			Pen pen = Pens.Black;

			//Weak event pattern for listening to property changed.
			PropertyChangedEventManager.AddListener(Properties.Settings.Default, this);
		}

		#region Weak Event Listener
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(PropertyChangedEventManager))
			{
				OnPropertyChanged(sender, (System.ComponentModel.PropertyChangedEventArgs)e);
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		public override string Name
		{
			get { return "Local Files"; }
		}

		public override string Author
		{
			get { return "Alex Vallat"; }
		}

		public override string Version
		{
			get { return typeof(LocalFilesSource).Assembly.GetName().Version.ToString(); }
		}

		protected override void SearchInternal(string artist, string album, AlbumArtDownloader.Scripts.IScriptResults results)
		{
			//Add the pattern used to the history list.
			CustomSettingsUI.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.DataBind, new System.Threading.ThreadStart(delegate
			{
				((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.AddPatternToHistory();
			}));

			string pathPattern = Common.SubstitutePlaceholders(SearchPathPattern, artist, album);
			
			//Avoid duplicates
			StringDictionary addedFiles = new StringDictionary();

			//Match path with wildcards
			foreach (string filename in Common.ResolvePathPattern(pathPattern))
			{
				if (!addedFiles.ContainsKey(filename)) //Don't re-add a file that's already been added
				{
					addedFiles.Add(filename, null);

					//Each filename is potentially an image, so try to load it
					try
					{
						IntPtr hBitmap;
						int status = GdipCreateBitmapFromFile(filename, out hBitmap);
						GdipDisposeImage(new HandleRef(this, hBitmap));
						if (status == 0)
						{
							//Successfully opened as image

							//Create an in-memory copy so that the bitmap file isn't in use, and can be replaced
							byte[] fileBytes = File.ReadAllBytes(filename); //Read the file, closing it after use
							Bitmap bitmap = new Bitmap(new MemoryStream(fileBytes)); //NOTE: Do not dispose of MemoryStream, or it will cause later saving of the bitmap to throw a generic GDI+ error (annoyingly)
							results.Add(bitmap, Path.GetFileName(filename), bitmap.Width, bitmap.Height, bitmap);
						}
						else
						{
							System.Diagnostics.Trace.WriteLine("Skipping non-bitmap file in local file search: " + filename);
						}
					}
					catch (Exception e)
					{
						System.Diagnostics.Trace.WriteLine("Skipping unreadable file in local file search: " + filename);
						System.Diagnostics.Trace.Indent();
						System.Diagnostics.Trace.WriteLine(e.Message);
						System.Diagnostics.Trace.Unindent();
					}
				}
			}
		}

		internal override Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return (Bitmap)fullSizeCallbackParameter;
		}

		private string mSearchPathPattern;
		/// <summary>
		/// The path pattern to search for images in.
		/// </summary>
		public string SearchPathPattern
		{
			get 
			{
				if (UseSearchPathPattern)
				{
					return mSearchPathPattern;
				}
				else
				{
					//If not using a custom search path pattern, use a pattern based on the location to save images to
					return Properties.Settings.Default.DefaultSavePath
												.Replace("%name%", "*")
												.Replace("%extension%", "*")
												.Replace("%source%", "*")
												.Replace("%size%", "*");
				}
			}
			set 
			{
				if (mSearchPathPattern != value)
				{
					mSearchPathPattern = value;

					SettingsChanged = true;
					NotifyPropertyChanged("SearchPathPattern");
				}
			}
		}

		private bool mUseSearchPathPattern;
		/// <summary>
		/// Whether or not to use a custom specified search path pattern.
		/// If false, a pattern based on the location to save images to is used.
		/// </summary>
		public bool UseSearchPathPattern
		{
			get { return mUseSearchPathPattern; }
			set 
			{
				if (mUseSearchPathPattern != value)
				{
					if (value)
					{
						//Ensure that the default path pattern is in the history list, so it can be selected
						((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.AddPatternToHistory();
					}

					mUseSearchPathPattern = value;
					NotifyPropertyChanged("UseSearchPathPattern");
					NotifyPropertyChanged("SearchPathPattern"); //This will change too, as it is coerced by this value.
				}
			}
		}

		private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(!UseSearchPathPattern && e.PropertyName == "DefaultSavePath")
				NotifyPropertyChanged("SearchPathPattern"); //This will change too, as it is coerced by this value.
		}

		#region Settings
		protected override SourceSettings GetSettings()
		{
			if (String.IsNullOrEmpty(Name))
				throw new InvalidOperationException("Cannot load settings for a source with no name");

			return ((App)Application.Current).GetSourceSettings(Name, Settings.Creator);
		}

		protected override void LoadSettingsInternal(SourceSettings settings)
		{
			base.LoadSettingsInternal(settings);

			Settings localFilesSourceSettings = (Settings)settings;
			SearchPathPattern = localFilesSourceSettings.SearchPathPattern;
			UseSearchPathPattern = localFilesSourceSettings.UseSearchPathPattern;

			LoadPathPatternHistory(localFilesSourceSettings);
		}
		
		protected override void SaveSettingsInternal(SourceSettings settings)
		{
			Settings localFilesSourceSettings = (Settings)settings;
			localFilesSourceSettings.SearchPathPattern = mSearchPathPattern;
			localFilesSourceSettings.UseSearchPathPattern = mUseSearchPathPattern;

			SavePathPatternHistory(localFilesSourceSettings);

			base.SaveSettingsInternal(localFilesSourceSettings);
		}

		private void LoadPathPatternHistory(Settings localFilesSourceSettings)
		{
			ICollection<String> history = ((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.History;
			history.Clear();
			foreach (string historyItem in localFilesSourceSettings.SearchPathPatternHistory)
			{
				history.Add(historyItem);
			}
		}

		private void SavePathPatternHistory(Settings localFilesSourceSettings)
		{
			ICollection<String> history = ((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.History;

			localFilesSourceSettings.SearchPathPatternHistory.Clear();
			foreach (string historyItem in history)
			{
				localFilesSourceSettings.SearchPathPatternHistory.Add(historyItem);
			}
		}

		protected override System.Windows.Controls.Control CreateCustomSettingsUI()
		{
			return new LocalFilesSourceSettings();
		}

		internal class Settings : SourceSettings
		{
			#region Creation
			//SourceSettings overrides should provide custom versions of these too
			public new static SourceSettingsCreator Creator
			{
				get
				{
					return new SourceSettingsCreator(Create);
				}
			}
			private static SourceSettings Create(string name)
			{
				return new Settings(name);
			}
			#endregion

			public Settings(string sourceName): base(sourceName)
			{
			}

			[DefaultSettingValueAttribute("")]
			[UserScopedSetting]
			public string SearchPathPattern
			{
				get
				{
					return ((string)(this["SearchPathPattern"]));
				}
				set
				{
					this["SearchPathPattern"] = value;
				}
			}

			[DefaultSettingValueAttribute("False")]
			[UserScopedSetting]
			public bool UseSearchPathPattern
			{
				get
				{
					return ((bool)(this["UseSearchPathPattern"]));
				}
				set
				{
					this["UseSearchPathPattern"] = value;
				}
			}

			[DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
			[UserScopedSetting]
			public StringCollection SearchPathPatternHistory
			{
				get
				{
					return ((StringCollection)(this["SearchPathPatternHistory"]));
				}
				set
				{
					this["SearchPathPatternHistory"] = value;
				}
			}
		}
		#endregion
	}
}
