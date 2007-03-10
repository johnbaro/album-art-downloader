using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace AlbumArtDownloader.Controls
{
	public interface ISource : INotifyPropertyChanged
	{
		string Name { get; }
		string Author { get; }
		string Version { get; }

		bool IsEnabled { get;set;}

		ObservableCollection<IAlbumArt> Results { get;}
		bool IsSearching { get; }
		int EstimatedResultsCount { get; }

		void AbortSearch();
	}
}
