using System;
using System.Windows;
using System.Configuration;
using System.IO;
using System.Reflection;
using AlbumArtDownloader.Scripts;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AlbumArtDownloader
{
	public partial class App : System.Windows.Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			#region Command Args
			
			Arguments arguments = new Arguments(e.Args);
			if (arguments.Contains("?"))
			{
				ShowCommandArgs();
				Shutdown();
				return;
			}

			bool? autoClose = null;
			string artist = null, album = null, path = null;
			List<String> useSources = new List<string>(); 
			List<String> excludeSources = new List<string>();
			string errorMessage = null;

			foreach (Parameter parameter in arguments)
			{
				//Check un-named parameters
				if (parameter.Name == null)
				{
					//For un-named parameters, use compatibility mode: 3 args,  "<artist>" "<album>" "<path to save image>"
					switch (arguments.IndexOf(parameter))
					{
						case 0:
							artist = parameter.Value;
							break;
						case 1:
							album = parameter.Value;
							break;
						case 2:
							path = parameter.Value;
							break;
						default:
							errorMessage = "Only the first three parameters may be un-named";
							break;
					}
				}
				else
				{
					//Check named parameters
					switch (parameter.Name.ToLower()) //Case insensitive parameter names
					{
						case "artist":
						case "ar":
							artist = parameter.Value;
							break;
						case "album":
						case "al":
							album = parameter.Value;
							break;
						case "path":
						case "p":
							path = parameter.Value;
							//Compatibility mode: if an "f" parameter, for filename, is provided, append it to the path.
							string filename;
							if (arguments.TryGetParameterValue("f", out filename))
							{
								path = Path.Combine(path, filename);
							}
							break;
						case "f":
							break; //See case "p" for handling of this parameter
						case "autoclose":
						case "ac":
							if (parameter.Value.Equals("off", StringComparison.InvariantCultureIgnoreCase))
							{
								autoClose = false;
							}
							else
							{
								autoClose = true;
							}
							break;
						case "sources":
						case "s":
							useSources.AddRange(parameter.Value.Split(','));
							break;
						case "exclude":
						case "es":
							excludeSources.AddRange(parameter.Value.Split(','));
							break;
						case "ae": //Compatibility: Show Existing Album Art
							excludeSources.Add("Local Files");
							break; //Not currently supported
						case "pf": //Compatibility: Show pictures in folder
							break; //Not currently supported
						default:
							errorMessage = "Unexpected command line parameter: " + parameter.Name;
							break;
					}
				}
				if (errorMessage != null)
					break; //Stop parsing args if there was an error
			}
			if (errorMessage != null) //Problem with the command args, so display the error, and the help
			{
				ShowCommandArgs(errorMessage);
				Shutdown();
				return;
			}
			
			#endregion

			UpgradeSettings();

#if EPHEMERAL_SETTINGS
			AlbumArtDownloader.Properties.Settings.Default.Reset();
#endif

			AssignDefaultSettings();

			//Only shut down if the Exit button is pressed
			ShutdownMode = ShutdownMode.OnExplicitShutdown;
			if (Splashscreen.ShowIfRequired())
			{
				LoadScripts(); //TODO: Should this be done showing the splashscren? Faliures to load scripts could be shown in the details area...

				//Now shut down when all the windows are closed
				ShutdownMode = ShutdownMode.OnLastWindowClose;
				ArtSearchWindow searchWindow = new ArtSearchWindow();

				#region Apply Command Args Settings
				if (autoClose.HasValue)
					searchWindow.OverrideAutoClose(autoClose.Value);
				if (path != null)
					searchWindow.SetDefaultSaveFolderPattern(path);
				if(useSources.Count > 0)
					searchWindow.UseSources(useSources);
				if (excludeSources.Count > 0)
					searchWindow.ExcludeSources(excludeSources);
				#endregion

				searchWindow.Show();

				if (artist != null || album != null)
					searchWindow.Search(artist, album);
			}
			else
			{
				//Splashscreen returned false, so exit
				Shutdown();
			}
		}

		//Any other settings loaded will also require upgrading, if the main settings do, so set this flag to indicate that.
		private bool mSettingsUpgradeRequired;
		private void UpgradeSettings()
		{
			//Settings may need upgrading from an earlier version
			string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			if (AlbumArtDownloader.Properties.Settings.Default.ApplicationVersion != currentVersion)
			{
				System.Diagnostics.Debug.WriteLine("Upgrading settings");
				mSettingsUpgradeRequired = true;
				AlbumArtDownloader.Properties.Settings.Default.Upgrade();
				AlbumArtDownloader.Properties.Settings.Default.ApplicationVersion = currentVersion;
			}
		}

		/// <summary>
		/// Show the command args helper screen, without any error message
		/// </summary>
		private void ShowCommandArgs()
		{
			ShowCommandArgs(null);
		}
		/// <summary>
		/// Show the command args helper screen, with the specified error message
		/// </summary>
		private void ShowCommandArgs(string errorMessage)
		{
			new CommandArgsHelp().ShowDialog(errorMessage);
		}

		/// <summary>
		/// Assign sensible defaults to values without hard-coded defaults
		/// </summary>
		private void AssignDefaultSettings()
		{
			if(AlbumArtDownloader.Properties.Settings.Default.DefaultSavePath == "%default%")
				AlbumArtDownloader.Properties.Settings.Default.DefaultSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), @"Album Art\%artist%\%album%\Folder.%extension%");
		}

		/// <summary>
		/// Load all the scripts from dlls in the Scripts folder.
		/// <remarks>Boo scripts should previously have been compiled into
		/// a dll (boo script cache.dll)</remarks> 
		/// </summary>
		private void LoadScripts()
		{
			mScripts = new List<IScript>();
			foreach (string dllFile in Directory.GetFiles(ScriptsPath, "*.dll"))
			{
				try
				{
					Assembly assembly = Assembly.LoadFile(dllFile);
					foreach (Type type in assembly.GetTypes())
					{
						try
						{
							IScript script = null;
							//Check for types implementing IScript
							if (typeof(IScript).IsAssignableFrom(type))
							{
								script = (IScript)Activator.CreateInstance(type);
							}
							//Check for static scripts (for backwards compatibility)
							else if (type.Namespace == "CoverSources")
							{
								script = new StaticScript(type);
							}

							if(script != null)
								mScripts.Add(script);
						}
						catch (Exception e)
						{
							//Skip the type. Does this need to display a user error message?
							System.Diagnostics.Debug.Fail(String.Format("Could not load script: {0}\n\n{1}", type.Name, e.Message));
						}
					}
				}
				catch (Exception e)
				{
					//Skip the assembly
					System.Diagnostics.Debug.Fail(String.Format("Could not load assembly: {0}\n\n{1}", dllFile, e.Message));
				}
			}
		}

		private List<IScript> mScripts;
		public IEnumerable<IScript> Scripts
		{
			get
			{
				if (mScripts == null)
					throw new InvalidOperationException("Must call LoadScripts() before using this property");

				return mScripts;
			}
		}

		private static string mCachedScriptsPath;
		public static string ScriptsPath
		{
			get
			{
				if (mCachedScriptsPath == null)
					mCachedScriptsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "scripts");

				return mCachedScriptsPath;
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			//Save all the settings
			AlbumArtDownloader.Properties.Settings.Default.Save();
			foreach (SourceSettings sourceSettings in mSourceSettings.Values)
			{
				sourceSettings.Save();
			}

			base.OnExit(e);
		}

		private Dictionary<string, SourceSettings> mSourceSettings = new Dictionary<String, SourceSettings>();
		public SourceSettings GetSourceSettings(string sourceName)
		{
			SourceSettings sourceSettings;
			if (!mSourceSettings.TryGetValue(sourceName, out sourceSettings))
			{
				sourceSettings = new SourceSettings(sourceName);
				if(mSettingsUpgradeRequired)
					sourceSettings.Upgrade();

#if EPHEMERAL_SETTINGS
				sourceSettings.Reset();
#endif

				mSourceSettings.Add(sourceName, sourceSettings);
			}
			
			return sourceSettings;
		}
	}
}