using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AlbumArtDownloader
{
	public partial class RenameArt : Window
	{
		private readonly string mCurrentFile;

		public RenameArt()
		{
			InitializeComponent();
		}

		public RenameArt(string currentFile)
		{
			mCurrentFile = currentFile;
			InitializeComponent();

			// load the image, specify CacheOption so the file is not locked
			var image = new BitmapImage();
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = new Uri(CurrentFile);
			image.EndInit();
			mPreview.Source = image;

			string fileName = Path.GetFileName(CurrentFile);
			mNewNameBox.Text = fileName;
			mNewNameBox.Select(0, Path.GetFileNameWithoutExtension(fileName).Length);
			mNewNameBox.Focus();
		}

		private void OnOKClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (DialogResult.GetValueOrDefault())
			{
				//Try to perform the actual rename
				try
				{
					File.Move(CurrentFile, Path.Combine(Path.GetDirectoryName(CurrentFile), mNewNameBox.Text));
				}
				catch (Exception ex)
				{
					mFailureMessage.Text = ex.Message;
					mFailureMessage.Visibility = Visibility.Visible;
					mNewNameBox.Focus();
					mNewNameBox.TextChanged += ClearErrorOnChange;
					e.Cancel = true;
					return;
				}
			}

			base.OnClosing(e);
		}

		private void ClearErrorOnChange(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			mNewNameBox.TextChanged -= ClearErrorOnChange;
			mFailureMessage.Visibility = Visibility.Collapsed;
		}

		public string CurrentFile
		{
			get { return mCurrentFile; }
		}
	}
}
