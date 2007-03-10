using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;

namespace AlbumArtDownloader
{
	internal class AlbumArt : DependencyObject, Controls.IAlbumArt
	{
		private Source mSource;
		private object mFullSizeCallbackParameter;
		/// <summary>If true, then the current value of Image is the full sized image, and no further retrieval is necessary.</summary>
		private bool mIsFullSize = false;

		public event PropertyChangedEventHandler PropertyChanged;

		public AlbumArt(Source source, Bitmap thumbnail, string name, double width, double height, object fullSizeCallbackParameter)
		{
			mSource = source;
			BitmapImage = thumbnail;
			ResultName = name;
			ImageWidth = width;
			ImageHeight = height;
			mFullSizeCallbackParameter = fullSizeCallbackParameter;
		}

		#region Dependency Properties
		public static readonly DependencyProperty DefaultFilePathPatternProperty = DependencyProperty.Register("DefaultFilePathPattern", typeof(string), typeof(AlbumArt), new FrameworkPropertyMetadata(String.Empty, new PropertyChangedCallback(OnDefaultFilePathPatternChanged)));
		public string DefaultFilePathPattern
		{
			get { return (string)GetValue(DefaultFilePathPatternProperty); }
			set { SetValue(DefaultFilePathPatternProperty, value); }
		}
		private static void OnDefaultFilePathPatternChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			sender.CoerceValue(FilePathProperty);
		}

		public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(AlbumArt), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnFilePathChanged), new CoerceValueCallback(CoerceFilePath)));
		public string FilePath
		{
			get { return (string)GetValue(FilePathProperty); }
			set { SetValue(FilePathProperty, value); }
		}
		private static void OnFilePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)sender;
			albumArt.IsSaved = false; //Not saved if the file path has changed
		}
		private static object CoerceFilePath(DependencyObject sender, object value)
		{
			AlbumArt albumArt = (AlbumArt)sender;

			if (String.IsNullOrEmpty((string)value))
			{
				albumArt.IsCustomFilePath = false;

				//Construct the default file path
				string defaultFilePathFormat = albumArt.DefaultFilePathPattern.Replace("%name%", "{0}").Replace("%source%", "{1}").Replace("%size%", "{2} x {3}").Replace("%extension%", "{4}");
				return String.Format(defaultFilePathFormat, 
					albumArt.ResultName, 
					albumArt.SourceName, 
					albumArt.ImageWidth, albumArt.ImageHeight,
					albumArt.ImageCodecInfo.FilenameExtension.Split(';')[0].Substring(2).ToLower()); //Use the first filename extension of the codec, with *. removed from it, in lower case
			}
			else
			{
				albumArt.IsCustomFilePath = true;
				return value;
			}
		}

		#endregion
		#region Properties
		/// <summary>The Album Art, as a bitmap. Backs the <see cref="Image"/> accessor</summary>
		private Bitmap mBitmapImage;
		private Bitmap BitmapImage
		{
			get
			{
				return mBitmapImage;
			}
			set
			{
				if (value != mBitmapImage)
				{
					mBitmapImage = value;

					//Reset the cached image source and codec info (they will be recreated from the new bitmap), and notify of the change.
					mCachedImageSource = null;
					mCachedImageCodecInfo = null;

					NotifyPropertyChanged("Image");
					NotifyPropertyChanged("ImageCodecInfo");
				}
			}
		}

		private ImageSource mCachedImageSource;
		public ImageSource Image
		{
			get 
			{
				if (mCachedImageSource == null)
				{
					//Create the image source from the bitmap image
					mCachedImageSource = BitmapHelpers.ConvertBitmapToBitmapSource(BitmapImage);
				}
				return mCachedImageSource; 
			}
		}

		private ImageCodecInfo mCachedImageCodecInfo;
		public ImageCodecInfo ImageCodecInfo
		{
			get
			{
				if (mCachedImageCodecInfo == null)
				{
					//Find the codec
					Guid bitmapFormatGuid = BitmapImage.RawFormat.Guid;
					foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
					{
						if (info.FormatID == bitmapFormatGuid)
						{
							mCachedImageCodecInfo = info;
							break;
						}
					}
				}
				return mCachedImageCodecInfo;
			}
		}

		private double mImageWidth;
		public double ImageWidth
		{
			get { return mImageWidth; }
			private set
			{
				if (mImageWidth != value)
				{
					mImageWidth = value;
					NotifyPropertyChanged("ImageWidth");
					CoerceValue(FilePathProperty);
				}
			}
		}
		private double mImageHeight;
		public double ImageHeight
		{
			get { return mImageHeight; }
			private set
			{
				if (mImageHeight != value)
				{
					mImageHeight = value;
					NotifyPropertyChanged("ImageHeight");
					CoerceValue(FilePathProperty);
				}
			}
		}

		private string mResultName;
		public string ResultName
		{
			get { return mResultName; }
			internal set
			{
				if (mResultName != value)
				{
					mResultName = value;
					NotifyPropertyChanged("ResultName");
					CoerceValue(FilePathProperty);
				}
			}
		}
		public string SourceName
		{
			get { return mSource.Name; }
		}

		private bool mIsCustomFilePath;
		public bool IsCustomFilePath
		{
			get { return mIsCustomFilePath; }
			private set
			{
				if (mIsCustomFilePath != value)
				{
					mIsCustomFilePath = value;
					NotifyPropertyChanged("IsCustomFilePath");
				}
			}
		}
		private bool mIsSaved;
		public bool IsSaved
		{
			get { return mIsSaved; }
			internal set
			{
				if (mIsSaved != value)
				{
					mIsSaved = value;
					NotifyPropertyChanged("IsSaved");
				}
			}
		}
		private bool mIsSaving;
		public bool IsSaving
		{
			get { return mIsSaving; }
			private set
			{
				if (mIsSaving != value)
				{
					mIsSaving = value;
					NotifyPropertyChanged("IsSaving");
				}
			}
		}
		private bool mIsDownloading;
		public bool IsDownloading
		{
			get { return mIsDownloading; }
			private set
			{
				if (mIsDownloading != value)
				{
					mIsDownloading = value;
					NotifyPropertyChanged("IsDownloading");
				}
			}
		}
		#endregion

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region Retrieve Full Size Image
		/// <summary>
		/// Begins an asynchronous retrieval of the full size image
		/// </summary>
		public void RetrieveFullSizeImage()
		{
			RetrieveFullSizeImage(null);
		}
		/// <summary>
		/// Begins an asynchronous retrieval of the full size image, notifying a callback on completion
		/// <param name="callback">Called when the retrieval completes. State object will be the AlbumArt instance.</param>
		/// </summary>
		public void RetrieveFullSizeImage(WaitCallback callback)
		{
			if (!mIsFullSize)
			{
				IsDownloading = true;
				ThreadPool.QueueUserWorkItem(new WaitCallback(RetrieveFullSizeImageWorker), callback);
			}
			else
			{
				//Raise the callback anyway, in case anything is waiting on it
				if (callback != null)
					callback(this);
			}
		}

		private void RetrieveFullSizeImageWorker(object state)
		{
			WaitCallback callback = state as WaitCallback;

			Bitmap fullSizeImage = mSource.RetrieveFullSizeImage(mFullSizeCallbackParameter);
			if (fullSizeImage != null) //If it is null, just use the thumbnail image
			{
				BitmapImage = fullSizeImage;
			}

			//Update the values
			Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				ImageWidth = Math.Round(Image.Width);
				ImageHeight = Math.Round(Image.Height);
				IsDownloading = false;

				mIsFullSize = true;

				if (callback != null)
					callback(this);
			}));
		}

		/// <summary>If true, then the current value of Image is the full sized image, and no further retrieval is necessary.</summary>
		internal bool IsFullSize
		{
			get
			{
				return mIsFullSize;
			}
		}
		#endregion

		#region Saving
		/// <summary>
		/// Begins an asynchronous Save operation, including downloading the full size image.
		/// The operation is synchronous until download begins, then asynch.
		/// </summary>
		internal void Save()
		{
			if (String.IsNullOrEmpty(FilePath))
			{
				SaveAs();
				return;
			}
			//Check that it is possible to save to FilePath
			try
			{
				//Check if FilePath already exists
				if (File.Exists(FilePath))
				{
					//Confirm overwrite
					if (MessageBox.Show(String.Format("'{0}' already exists.\nDo you want to replace it?", Path.GetFullPath(FilePath)), "Album Art Downloader", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
					{
						SaveAs();
						return;
					}
				}

				DirectoryInfo folder = new DirectoryInfo(Path.GetDirectoryName(FilePath));
				if (!folder.Exists)
					folder.Create();
				
				File.Create(FilePath, 1, FileOptions.DeleteOnClose).Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(String.Format("Could not save image '{0}':\n\n{1}", Path.GetFileName(FilePath), e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			IsSaving = true;
			
			RetrieveFullSizeImage(new WaitCallback(SaveInternal));
		}

		/// <summary>
		/// Begins an asynchronous Save As operation, including downloading the full size image.
		/// The operation is synchronous until download begins (including the dialog), then asynch.
		/// </summary>
		internal void SaveAs()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = FilePath;
			saveFileDialog.DefaultExt = ImageCodecInfo.FilenameExtension.Split(';')[0].ToLower(); //Default to the first extension
			saveFileDialog.OverwritePrompt = false; //That will be handled by Save();
			saveFileDialog.Filter = String.Format("Image Files ({0})|{0}|All Files|*.*", ImageCodecInfo.FilenameExtension.ToLower());
			saveFileDialog.ValidateNames = false;

			if (saveFileDialog.ShowDialog().GetValueOrDefault(false))
			{
				FilePath = saveFileDialog.FileName;

				Save();
			}
		}

		//Performs the actual save operation, as a result of the full size image retreival completing
		private void SaveInternal(object sender)
		{
			System.Diagnostics.Debug.Assert(mIsFullSize, "Full size image was not retrieved");

			try
			{
				this.BitmapImage.Save(FilePath);
			}
			catch (Exception e)
			{
				MessageBox.Show(String.Format("Unexpected faliure saving image to: \"{0}\"\n\n{1}", FilePath, e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			IsSaving = false;
			IsSaved = true;
		}
		#endregion
	}
}
