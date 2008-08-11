using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using AlbumArtDownloader.Controls;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Interaction logic for EditPresets.xaml
	/// </summary>
	public partial class EditPresets : Window
	{
		private ObservableCollection<Preset> mPresets = new ObservableCollection<Preset>();

		public EditPresets()
		{
			mPresets.Add(new Preset("hello", "there"));
			mPresets.Add(new Preset("another", "line"));
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(EditableCell.Commands.Edit, EditExec, EditCanExec));
		}

		private void EditExec(object sender, ExecutedRoutedEventArgs e)
		{
			//IsEditing = true;
		}

		private void EditCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mPresetsList.SelectedItems.Count == 1;
		}
		
		public ObservableCollection<Preset> Presets
		{
			get { return mPresets; }
		}

	}
}
