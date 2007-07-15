using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An observable collection that owns all its contents, and when one of its
	/// items is removed, it is disposed of.
	/// </summary>
	internal class ObservableCollectionOfDisposables<T> : ObservableCollection<T>
		where T : IDisposable
	{
		protected override void ClearItems()
		{
			foreach (T item in this)
			{
				item.Dispose();
			}
			base.ClearItems();
		}
		protected override void RemoveItem(int index)
		{
			T item = this[index];
			
			base.RemoveItem(index);
			
			if (item != null)
				item.Dispose();
		}
	}
}
