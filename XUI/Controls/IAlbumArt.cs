using System;
using System.ComponentModel;
using System.Windows.Media;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// The ItemsSource of ArtPanelList should be an enumerable of objects implementing this interface.
	/// </summary>
	public interface IAlbumArt : INotifyPropertyChanged
	{
		string ResultName { get; }
		string FilePath { get; set; }
		ImageSource Image { get; }
		double ImageHeight { get; }
		double ImageWidth { get; }
		bool IsCustomFilePath { get; }
		bool IsDownloading { get; }
		bool IsSaved { get; }
		bool IsSaving { get; }
		string SourceName { get; }

		/// <summary>
		/// Begins an asynchronous retrieval of the full size image
		/// </summary>
		void RetrieveFullSizeImage();
		/// <summary>
		/// Begins an asynchronous retrieval of the full size image, notifying a callback on completion
		/// <param name="callback">Called when the retrieval completes. State object will be the AlbumArt instance.</param>
		/// </summary>
		void RetrieveFullSizeImage(System.Threading.WaitCallback callback);
	}
}
