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
using System.Windows.Controls.Primitives;

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
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewExec));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteExec, SelectedItemCanExec));
			CommandBindings.Add(new CommandBinding(EditableCell.Commands.Edit, EditExec, SelectedItemCanExec));
			CommandBindings.Add(new CommandBinding(ComponentCommands.MoveUp, MoveUpExec, MoveUpCanExec));
			CommandBindings.Add(new CommandBinding(ComponentCommands.MoveDown, MoveDownExec, MoveDownCanExec));
		}

		#region Add New Preset
		private Preset mNewlyAddedPreset;
		private void NewExec(object sender, ExecutedRoutedEventArgs e)
		{
			mPresetsList.ItemContainerGenerator.StatusChanged += OnNewItemContainerGenerated;

			mNewlyAddedPreset = new Preset("(New Preset)", "");
			Presets.Add(mNewlyAddedPreset);
			mPresetsList.SelectedItem = mNewlyAddedPreset;
		}

		private void OnNewItemContainerGenerated(object sender, EventArgs e)
		{
			if (mPresetsList.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				mPresetsList.ItemContainerGenerator.StatusChanged -= OnNewItemContainerGenerated; //Only do this once

				//Find the cell, and start editing it
				var editableCell = Common.FindVisualChild<EditableCell>(mPresetsList.ItemContainerGenerator.ContainerFromItem(mNewlyAddedPreset));
				editableCell.IsEditing = true;

				editableCell.IsEditingChanged += OnNewItemNameFirstEdited;
			}
		}

		private void OnNewItemNameFirstEdited(object sender, DependencyPropertyChangedEventArgs e)
		{
			var editableCell = ((EditableCell)sender);
			editableCell.IsEditingChanged -= OnNewItemNameFirstEdited; //Only do this for the first edit.

			mNewlyAddedPreset.Value = (string)editableCell.Value; //Default value is the same as the newly specified name

			mNewlyAddedPreset = null; //No further interaction with this needed
		}
		#endregion

		private void DeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			int selectedIndex = mPresetsList.SelectedIndex;
			
			Presets.RemoveAt(selectedIndex);
			
			if (selectedIndex >= Presets.Count)
			{
				selectedIndex = Presets.Count - 1;
			}
			mPresetsList.SelectedIndex = selectedIndex;
		}
	
		private void EditExec(object sender, ExecutedRoutedEventArgs e)
		{
			EditableCell firstEditableCell = Common.FindVisualChild<EditableCell>(mPresetsList.ItemContainerGenerator.ContainerFromIndex(mPresetsList.SelectedIndex));
			firstEditableCell.IsEditing = true;
		}

		private void SelectedItemCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mPresetsList.SelectedIndex != -1;
		}

		private void MoveUpExec(object sender, ExecutedRoutedEventArgs e)
		{
			Presets.Move(mPresetsList.SelectedIndex, mPresetsList.SelectedIndex - 1);
		}

		private void MoveUpCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mPresetsList.SelectedIndex > 0;
		}

		private void MoveDownExec(object sender, ExecutedRoutedEventArgs e)
		{
			Presets.Move(mPresetsList.SelectedIndex, mPresetsList.SelectedIndex + 1);
		}

		private void MoveDownCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mPresetsList.SelectedIndex != -1 && mPresetsList.SelectedIndex < Presets.Count - 1;
		}

		public ObservableCollection<Preset> Presets
		{
			get { return mPresets; }
		}

		private void OnOKClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
