using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace AlbumArtDownloader
{
	internal static class Common
	{
		#region New Window
		public static ArtSearchWindow NewSearchWindow()
		{
			return NewSearchWindow(null);
		}
		public static ArtSearchWindow NewSearchWindow(IAppWindow existingWindow)
		{
			return NewSearchWindow(existingWindow, false);
		}
		/// <param name="forceShown">If true, the new search window will be immediately shown, rather than queued.</param>
		public static ArtSearchWindow NewSearchWindow(IAppWindow existingWindow, bool forceShown)
		{
			//Enqueue rather than opening the new window directly
			ArtSearchWindow newWindow = new ArtSearchWindow();
			SetupNewWindow(newWindow, existingWindow);
			SearchQueue searchQueue = ((App)Application.Current).SearchQueue;
			searchQueue.EnqueueSearchWindow(newWindow);

			if (forceShown)
			{
				//Ensure the new window is shown immediately, rather than queued.
				searchQueue.ForceSearchWindow(newWindow);
			}
			else if (searchQueue.Queue.Count == 1)
			{
				//This is the first item enqueued, so show the queue manager window
				searchQueue.ShowManagerWindow();
			}

			return newWindow;
		}

		public static FileBrowser NewFileBrowser()
		{
			return NewFileBrowser(null);
		}
		public static FileBrowser NewFileBrowser(IAppWindow existingWindow)
		{
			return (FileBrowser)ShowNewWindow(new FileBrowser(), existingWindow);
		}

		public static FoobarBrowser NewFoobarBrowser()
		{
			return NewFoobarBrowser(null);
		}
		public static FoobarBrowser NewFoobarBrowser(IAppWindow existingWindow)
		{
			return (FoobarBrowser)ShowNewWindow(new FoobarBrowser(), existingWindow);
		}

		public static ArtPreviewWindow NewPreviewWindow()
		{
			return NewPreviewWindow(null);
		}
		public static ArtPreviewWindow NewPreviewWindow(IAppWindow existingWindow)
		{
			return (ArtPreviewWindow)ShowNewWindow(new ArtPreviewWindow(), existingWindow);
		}

		private static IAppWindow ShowNewWindow(IAppWindow newWindow, IAppWindow oldWindow)
		{
			SetupNewWindow(newWindow, oldWindow);

			newWindow.Show();
			return newWindow;
		}

		private static void SetupNewWindow(IAppWindow newWindow, IAppWindow oldWindow)
		{
			if (oldWindow != null)
			{
				//Save values to settings so that the new window picks up on them
				oldWindow.SaveSettings();
				//Load the newly saved settings
				newWindow.LoadSettings();

				//Move the window a little, so that it is obvious it is a new window
				newWindow.Left = oldWindow.Left + 40;
				newWindow.Top = oldWindow.Top + 40;

				//TODO: Neater laying out of windows which would go off the screen. Note how Firefox handles this, for example, when opening lots of new non-maximised windows.
				//TODO: Multimonitor support.
				if (newWindow.Left + newWindow.Width > SystemParameters.PrimaryScreenWidth)
				{
					//For the present, just make sure that the window doesn't leave the screen.
					newWindow.Left = SystemParameters.PrimaryScreenWidth - newWindow.Width;
				}
				if (newWindow.Top + newWindow.Height > SystemParameters.PrimaryScreenHeight)
				{
					newWindow.Top = SystemParameters.PrimaryScreenHeight - newWindow.Height;
				}
			}
		}
		#endregion

		#region EnumerableHelpers
		/// <summary>
		/// Takes a specified generic IEnumerable, and returns an unspecified one.
		/// </summary>
		public static System.Collections.IEnumerable UnspecifyEnumerable<T>(IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				yield return item;
			}
		}
		#endregion

		#region Resolve Path with Wildcards
		/// <summary>
		/// Substitutes Artist and Album placeholders for an image search path pattern.
		/// </summary>
		public static string SubstitutePlaceholders(string pathPattern, string artist, string album)
		{
			return pathPattern.Replace("%artist%", Common.MakeSafeForPath(artist))
								.Replace("%album%", Common.MakeSafeForPath(album))
							//Replace these too, just in case path pattern was copied and pasted with them in, for example
								.Replace("%name%", "*")
								.Replace("%extension%", "*")
								.Replace("%source%", "*")
								.Replace("%size%", "*");
		}

		private static Regex sPathPatternSplitter = new Regex(@"(?<fixed>(?:[^/\\*]*(?:[/\\]|$))*)(?<match>[^/\\]+)?[/\\]?(?<remainder>.*)", RegexOptions.Compiled);
		public static IEnumerable<string> ResolvePathPattern(string pathPattern)
		{
			Match match = sPathPatternSplitter.Match(pathPattern);

			if (match.Groups["match"].Success)
			{
				//Theres a wildcard part of the path that needs matching against.
				DirectoryInfo fixedPart = null;
				try
				{
					fixedPart = new DirectoryInfo(match.Groups["fixed"].Value);
				}
				catch (Exception e)
				{
					//Path not valid, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not valid for file search: " + match.Groups["fixed"].Value);
					System.Diagnostics.Trace.Indent();
					System.Diagnostics.Trace.WriteLine(e.Message);
					System.Diagnostics.Trace.Unindent();
					yield break;
				}
				if (fixedPart == null || !fixedPart.Exists)
				{
					//Path not found, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not found for file search: " + match.Groups["fixed"].Value);
					yield break;
				}

				//Find all the matching paths for the part of pattern specified
				string searchPattern = match.Groups["match"].Value;
				if (searchPattern == "**")
				{
					//Recursive folder matching wildcard
					//Go into subfolders
					Stack<DirectoryInfo> subfolders = new Stack<DirectoryInfo>();
					subfolders.Push(fixedPart); //Start with the current folder
					while (subfolders.Count > 0)
					{
						DirectoryInfo searchInFolder = subfolders.Pop();

						foreach (string result in ResolvePathPattern(Path.Combine(searchInFolder.FullName, match.Groups["remainder"].Value)))
						{
							yield return result;
						}

						try
						{
							foreach (DirectoryInfo subfolder in searchInFolder.GetDirectories())
							{
								if ((subfolder.Attributes & FileAttributes.ReparsePoint) == 0) //Don't recurse into reparse points
								{
									subfolders.Push(subfolder);
								}
							}
						}
						catch (Exception e)
						{
							//Can't get subfolders
							System.Diagnostics.Trace.WriteLine("Can't search inside: " + searchInFolder.FullName);
							System.Diagnostics.Trace.Indent();
							System.Diagnostics.Trace.WriteLine(e.Message);
							System.Diagnostics.Trace.Unindent();
						}
					}
				}
				else
				{
					//Normal wildcard
					foreach (FileSystemInfo matchedPath in fixedPart.GetFileSystemInfos(searchPattern))
					{
						foreach (string result in ResolvePathPattern(Path.Combine(matchedPath.FullName, match.Groups["remainder"].Value)))
						{
							yield return result;
						}
					}
				}
			}
			else
			{
				//There is no wildcard part of the path remaining, so check if it exists
				if (Directory.Exists(pathPattern))
				{
					//It's a folder, so return all the files within it
					foreach (string result in Directory.GetFiles(pathPattern))
					{
						yield return result;
					}
				}
				else if (File.Exists(pathPattern))
				{
					//It's a file, so return it
					yield return pathPattern;
				}
				else
				{
					//Path not found, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not found for file search: " + match.Groups["fixed"].Value);
					yield break;
				}
			}
		}
		#endregion

		/// <summary>
		/// Gets the command args this app was started with, as executed, not parsed into args[]
		/// </summary>
		public static string GetCommandArgs() //Surely this should not be that tricky?
		{
			string commandLine = Environment.CommandLine;

			//Find either the space, which is a the delimeter, or a " mark, which bounds spaces
			int pos = 0;
			do
			{
				if (pos >= commandLine.Length)
					return String.Empty; //No command line args

				pos = commandLine.IndexOfAny(new char[] { ' ', '"' }, pos);
				if (pos == -1)
					return String.Empty; //No command line args

				if (commandLine[pos] == '"')
				{
					//Find the closing " mark. " marks can't be escaped in path names
					pos = commandLine.IndexOf('"', pos + 1) + 1;
					if (pos == 0)
					{
						//No command line args. Probably malformed command line too.
						System.Diagnostics.Trace.TraceWarning("Could not find closing \" mark in command line: " + commandLine);
						return String.Empty;
					}
					//Otherwise, go round again to find another quote, or alternatively a space
				}
				else
				{
					System.Diagnostics.Debug.Assert(commandLine[pos] == ' ', "Expecting a space here, if not a \" mark");
					//Everything past this point should now be the command args, as this is an unquoted space
					return commandLine.Substring(pos + 1);
				}
			} while (true);
		}

		/// <summary>
		/// Ensures that a string is safe to be part of a file path by replacing all illegal
		/// characters with underscores.
		/// </summary>
		public static string MakeSafeForPath(string value)
		{
			char[] invalid = Path.GetInvalidFileNameChars();
			char[] valueChars = value.ToCharArray();

			bool valueChanged = false;
			int invalidIndex = -1;
			while ((invalidIndex = value.IndexOfAny(invalid, invalidIndex + 1)) >= 0)
			{
				valueChars[invalidIndex] = '_';
				valueChanged = true;
			}
			if (valueChanged)
			{
				return new string(valueChars);
			}
			else //Don't perform the construction of the new string if not required
			{
				return value;
			}
		}
	}
}
