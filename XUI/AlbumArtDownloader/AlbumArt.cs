using AlbumArtDownloader.Scripts;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace AlbumArtDownloader
{
	public class AlbumArt : DependencyObject, INotifyPropertyChanged, IDisposable
	{
		private Source mSource;
		private object mFullSizeCallbackParameter;
		/// <summary>If true, then the current value of Image is the full sized image, and no further retrieval is necessary.</summary>
		private bool mIsFullSize = false;

		public event PropertyChangedEventHandler PropertyChanged;

		public AlbumArt(Source source, Bitmap thumbnail, string name, string infoUri, double width, double height, object fullSizeCallbackParameter, CoverType coverType)
		{
			mSource = source;
			BitmapImage = thumbnail;
			ResultName = name;
			InfoUri = infoUri;
			ImageWidth = width;
			ImageHeight = height;
			mFullSizeCallbackParameter = fullSizeCallbackParameter;
			CoverType = coverType;
		}
		
		[System.Obsolete("set coverType")]
		public AlbumArt(Source source, Bitmap thumbnail, string name, string infoUri, double width, double height, object fullSizeCallbackParameter)
		{
			mSource = source;
			BitmapImage = thumbnail;
			ResultName = name;
			InfoUri = infoUri;
			ImageWidth = width;
			ImageHeight = height;
			mFullSizeCallbackParameter = fullSizeCallbackParameter;
			CoverType = CoverType.Unknown;
		}
		
		public void Dispose()
		{
			BitmapImage = null; //This will dispose of the bitmap
			//No need for finaliser patten, as the Bitmap should finalise itself.
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
				return albumArt.DefaultFilePathPattern
								.Replace("%name%", Common.MakeSafeForPath(albumArt.ResultName))
								.Replace("%source%", Common.MakeSafeForPath(albumArt.SourceName))
								.Replace("%size%", String.Format("{0} x {1}", albumArt.ImageWidth, albumArt.ImageHeight))
								.Replace("%extension%", albumArt.ImageCodecInfo.FilenameExtension.Split(';')[0].Substring(2).ToLower()) //Use the first filename extension of the codec, with *. removed from it, in lower case
								.Replace("%preset%", Common.MakeSafeForPath(albumArt.Preset));
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
					if(mBitmapImage != null)
						mBitmapImage.Dispose(); //Dispose of the old bitmap first

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
					if (BitmapImage != null)
					{
						//Find the codec					
						Guid bitmapFormatGuid = BitmapImage.RawFormat.Guid;
						mCachedImageCodecInfo = ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == bitmapFormatGuid);
					}
				}
				if (mCachedImageCodecInfo == null)
				{
					//Could not find the codec. Leave the cached codec info as null, to attempt to recalculate
					Guid bmpFormatGuid = System.Drawing.Imaging.ImageFormat.Bmp.Guid;
					return ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == bmpFormatGuid);
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

		private string mInfoUri;
		public string InfoUri
		{
			get { return mInfoUri; }
			internal set
			{
				if (mInfoUri != value)
				{
					mInfoUri = value;
					NotifyPropertyChanged("InfoUri");
				}
			}
		}
		

		private string mPreset;
		public string Preset
		{
			get { return mPreset; }
			internal set
			{
				if (mPreset != value)
				{
					mPreset = value;
					NotifyPropertyChanged("Preset");
					CoerceValue(FilePathProperty);
				}
			}
		}

		public bool IsSourceLocal
		{
			//TODO: Add possibility of other local sources? Perhaps a flag on a sources?
			get { return mSource is LocalFilesSource; }
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
		
		private CoverType mCoverType = CoverType.Unknown;
		public CoverType CoverType
		{
			get { return mCoverType; }
			private set
			{
				if (mCoverType != value)
				{
					mCoverType = value;
					NotifyPropertyChanged("CoverType");
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
				if (Image != null) //Image should *never* be null, but this might be what's causing reported crashes at this location.
				{
					ImageWidth = Math.Round(Image.Width);
					ImageHeight = Math.Round(Image.Height);
				}
				else
				{
					System.Diagnostics.Debug.Fail("Image was unexpectedly null");
				}
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
				MessageBox.Show(String.Format("Could not save image '{0}':\n\n{1}", FilePath, e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
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
			saveFileDialog.AddExtension = true;
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

		#region Copy to Clipboard

		/// <summary>
		/// Begins an asynchronous Copy to Clipboard operation, including downloading the full size image.
		/// </summary>
		internal void CopyToClipboard()
		{
			RetrieveFullSizeImage(new WaitCallback(CopyToClipboardInternal));
		}

		private void CopyToClipboardInternal(object sender)
		{
			System.Diagnostics.Debug.Assert(mIsFullSize, "Full size image was not retrieved");
			System.Diagnostics.Debug.Assert(Image is BitmapSource, "Image is not a BitmapSource");

			try
			{
				Clipboard.SetImage((BitmapSource)Image);
			}
			catch (Exception e)
			{
				MessageBox.Show(String.Format("Unexpected faliure copying image to clipboard\n\n" + e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		#endregion
	}
}
