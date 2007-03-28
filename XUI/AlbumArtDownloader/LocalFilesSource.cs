using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Controls;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace AlbumArtDownloader
{
	internal class LocalFilesSource : Source
	{
		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int GdipCreateBitmapFromFile(string filename, out IntPtr bitmap);
		[DllImport("gdiplus.dll", ExactSpelling = true)]
		private static extern int GdipDisposeImage(HandleRef image);

		public LocalFilesSource()
		{
			Results.CollectionChanged += new NotifyCollectionChangedEventHandler(OnResultsChanged);
			//Ensure GDI+ is initialised
			Pen pen = Pens.Black;
		}

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
			string pathPattern = Properties.Settings.Default.DefaultSavePath
												.Replace("%artist%", artist)
												.Replace("%album%", album)
												.Replace("%name%", "*")
												.Replace("%extension%", "*")
												.Replace("%source%", "*")
												.Replace("%size%", "*");

			//Match path with wildcards
			foreach (string filename in ResolvePathPattern(pathPattern))
			{
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
						results.Add(bitmap, filename, bitmap.Width, bitmap.Height, bitmap);
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

		private void OnResultsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (AlbumArt albumArt in e.NewItems)
				{
					//Results from this source should have their filepath be their actual path
					albumArt.FilePath = albumArt.ResultName;
					//Their name can be the filename only
					albumArt.ResultName = Path.GetFileName(albumArt.FilePath);
					//And they are already saved.
					albumArt.IsSaved = true;
				}
			}
		}

		private static Regex sPathPatternSplitter = new Regex(@"(?<fixed>(?:[^/\\*]*(?:[/\\]|$))*)(?<match>[^/\\]+)?[/\\]?(?<remainder>.*)", RegexOptions.Compiled);
		private IEnumerable<string> ResolvePathPattern(string pathPattern)
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
					System.Diagnostics.Trace.WriteLine("Path not valid for local file search: " + match.Groups["fixed"].Value);
					System.Diagnostics.Trace.Indent();
					System.Diagnostics.Trace.WriteLine(e.Message);
					System.Diagnostics.Trace.Unindent();
					yield break;
				}
				if (fixedPart == null || !fixedPart.Exists)
				{
					//Path not found, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not found for local file search: " + match.Groups["fixed"].Value);
					yield break;
				}

				//Find all the matching paths for the part of pattern specified
				foreach (FileSystemInfo matchedPath in fixedPart.GetFileSystemInfos(match.Groups["match"].Value))
				{
					foreach (string result in ResolvePathPattern(Path.Combine(matchedPath.FullName, match.Groups["remainder"].Value)))
					{
						yield return result;
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
					System.Diagnostics.Trace.WriteLine("Path not found for local file search: " + match.Groups["fixed"].Value);
					yield break;
				}
			}
		}

		internal override Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return (Bitmap)fullSizeCallbackParameter;
		}
	}
}
