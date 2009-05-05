using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using AlbumArtDownloader.Scripts;
using System.IO;

namespace AlbumArtDownloader
{
	public class Updates
	{
		//Only have one viewer window open at any one time. This is that window.
		private static UpdatesViewer sViewer;

		private static bool sRestartPending;

		/// <summary>
		/// Performs a check for available updates, if <see cref="Properties.Settings.Default.AutoUpdateCheckInterval"/> has elapsed since the last check was made.
		/// <param name="forceDisplayViewer">If true, an update will always be performed, and the updates viewer will be displayed even if no updates are available</param>
		/// </summary>
		public static void CheckForUpdates(bool forceCheck)
		{
			TimeSpan timeSinceLastCheck = DateTime.Now - Properties.Settings.Default.LastUpdateCheck;
			if (forceCheck || timeSinceLastCheck > Properties.Settings.Default.AutoUpdateCheckInterval)
			{
				//A check is due, so start the thread to asynchronously download and process the update data
				ThreadPool.QueueUserWorkItem(new WaitCallback(PerformUpdateCheck), new PerformUpdateCheckParameters(forceCheck));
			}
		}

		/// <summary>
		/// Downloads the latest Updates XML file and produces an <see cref="Updates"/> list of available
		/// updates from it
		/// </summary>
		private static void PerformUpdateCheck(object state)
		{
			PerformUpdateCheckParameters parameters = (PerformUpdateCheckParameters)state;

			Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
			
			Updates updates = new Updates();

			try
			{
				XmlDocument updatesXml = new XmlDocument();
				updatesXml.Load(Properties.Settings.Default.UpdatesURI.AbsoluteUri);

				Uri baseUri = new Uri(updatesXml.DocumentElement.GetAttribute("BaseURI"));

				//Check to see if there is an application update available
				XmlElement appUpdateXml = updatesXml.SelectSingleNode("/Updates/Application") as XmlElement;
				if (appUpdateXml != null)
				{
					Version newAppVersion = new Version(appUpdateXml.GetAttribute("Version"));

					if (newAppVersion > Assembly.GetEntryAssembly().GetName().Version)
					{
						//Theres a new application version, so return an update with just that
						Uri uri = new Uri(baseUri, appUpdateXml.GetAttribute("URI"));

						updates.SetAppUpdate(appUpdateXml.GetAttribute("Name"), uri);
					}
				}

				//Create a lookup of all current scripts and their versions
				Dictionary<String, String> scripts = new Dictionary<String, String>();
				foreach (IScript script in ((App)Application.Current).Scripts)
				{
					scripts.Add(script.Name, script.Version);
				}

				foreach (XmlElement scriptUpdateXml in updatesXml.SelectNodes("/Updates/Script"))
				{
					string name = scriptUpdateXml.GetAttribute("Name");
					string newVersion = scriptUpdateXml.GetAttribute("Version");

					//Check to see if there is an older version of this script to update
					string currentVersion;
					if (scripts.TryGetValue(name, out currentVersion))
					{
						if (currentVersion != newVersion)
						{
							Uri uri = new Uri(baseUri, scriptUpdateXml.GetAttribute("URI"));

							updates.AddScriptUpdate(new ScriptUpdate(name, currentVersion, newVersion, uri));
						}
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Could not parse update xml from \"{0}\": {1}", Properties.Settings.Default.UpdatesURI.AbsoluteUri, ex.Message);
			}

			if (parameters.ForceCheck || //If forcing a check, show the updates viewer regardless
				updates.mScriptUpdates.Count > 0 || //If not forcing, only show if there are updates to be shown
				updates.HasApplicationUpdate)
			{
				parameters.Dispatcher.Invoke(new ThreadStart(delegate
				{
					if (sViewer != null)
					{
						sViewer.Close();
					}
					sViewer = new UpdatesViewer();
					sViewer.Show(updates);
					sViewer.Closed += new EventHandler(delegate { sViewer = null; });
				}));
			}
		}

		private struct PerformUpdateCheckParameters
		{
			private readonly Dispatcher mDispatcher;
			private readonly bool mForceCheck;

			public PerformUpdateCheckParameters(bool forceCheck)
			{
				mDispatcher = Dispatcher.CurrentDispatcher;
				mForceCheck = forceCheck;
			}
			public Dispatcher Dispatcher { get { return mDispatcher; } }
			public bool ForceCheck { get { return mForceCheck; } }
		}


		#region Instance members
		private readonly List<ScriptUpdate> mScriptUpdates = new List<ScriptUpdate>();
		private string mAppUpdateName;
		private Uri mAppUpdateUri;
		
		private Updates()
		{
		}

		private void AddScriptUpdate(ScriptUpdate scriptUpdate)
		{
			mScriptUpdates.Add(scriptUpdate);
		}

		private void SetAppUpdate(string name, Uri uri)
		{
			mAppUpdateName = name;
			mAppUpdateUri = uri;
		}

		public bool HasApplicationUpdate { get { return mAppUpdateName != null; } }
		public string ApplicationUpdateName { get { return mAppUpdateName; } }
		public Uri ApplicationUpdateUri { get { return mAppUpdateUri; } }

		/// <summary>
		/// If a restart is pending, no further updates should be displayed until the application has been restarted.
		/// </summary>
		public bool RestartPending { get { return sRestartPending; } }

		public IEnumerable<ScriptUpdate> ScriptUpdates { get { return mScriptUpdates.AsReadOnly(); } }

		public void DownloadSelectedScriptUpdates()
		{
			foreach (ScriptUpdate scriptUpdate in mScriptUpdates.Where(s => s.Selected))
			{
				scriptUpdate.Download();
				
				//A new script has been downloaded, so flag for restart required (which is static - remains set until restart!)
				sRestartPending = true;
			}
		}
		#endregion
	}

	public class ScriptUpdate
	{
		private readonly string mName;
		private readonly string mOldVersion;
		private readonly string mNewVersion;
		private readonly Uri mUri;
		private bool mSelected;

		public ScriptUpdate(string name, string oldVersion, string newVersion, Uri uri)
		{
			mName = name;
			mOldVersion = oldVersion;
			mNewVersion = newVersion;
			mUri = uri;

			mSelected = true;
		}

		public string Name { get { return mName; } }
		public string OldVersion { get { return mOldVersion; } }
		public string NewVersion { get { return mNewVersion; } }
		
		public bool Selected
		{
			get { return mSelected; }
			set { mSelected = value; }
		}

		public void Download()
		{
			string filename = Path.GetFileName(mUri.LocalPath);

			string targetPath = null;

			foreach (string scriptsPath in App.ScriptsPaths)
			{
				//See whether the file can be written there
				try
				{
					targetPath = Path.Combine(scriptsPath, filename);
					File.OpenWrite(targetPath).Close();
					break; //If it reaches here, the file is writable
				}
				catch(Exception fileWriteException)
				{
					System.Diagnostics.Trace.TraceWarning("Could not download script update for {0} to \"{1}\": {2}", Name, targetPath, fileWriteException.Message);
					targetPath = null;
				}
			}

			if (targetPath == null)
			{
				System.Diagnostics.Trace.TraceError("Could not download script update for {0} from \"{1}\": No writable paths found.", Name, mUri);
				return;
			}
				
			try
			{
				new System.Net.WebClient().DownloadFile(mUri, targetPath + ".part");
				//If it reaches here, then it was successfull - replace the existing one
				File.Delete(targetPath);
				File.Move(targetPath + ".part", targetPath);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Could not download script update for {0} from \"{1}\": {2}", Name, mUri, ex.Message);
			}
		}
	}
}
