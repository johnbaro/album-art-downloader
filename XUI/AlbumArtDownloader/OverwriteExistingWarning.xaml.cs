using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace AlbumArtDownloader
{
	public partial class OverwriteExistingWarning : Window
	{
		
		public static string Show(string filename, SaveFileDialog saveAsDialog, Window owner)
		{
			var dialog = new OverwriteExistingWarning
			{
				Filename = filename,
				SuggestedFilename = SuggestFilename(filename),
				SaveAsDialog = saveAsDialog,
				Owner = owner
			};
			dialog.InitializeComponent();
			dialog.mDefaultButton.Focus();
			
			if (dialog.ShowDialog().GetValueOrDefault(false))
			{
				return dialog.Filename;
			}

			return null;
		}
		
		private static string SuggestFilename(string originalFilename)
		{
			int i = 2; // Start with "File (2).ext"
			string extension = Path.GetExtension(originalFilename);
			string originalFilenameNoExtension = originalFilename.Substring(0, originalFilename.Length - extension.Length);
			
			string suggestedFilename;
			do
			{
				suggestedFilename = String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0} ({1}){2}", originalFilenameNoExtension, i++, extension);
			} while (File.Exists(suggestedFilename));
			
			return suggestedFilename;
		}

		public string Filename { get; private set; }
		public string SuggestedFilename { get; private set; }
		public SaveFileDialog SaveAsDialog { get; private set; }

		private void Overwrite_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void UseSuggestion_Click(object sender, RoutedEventArgs e)
		{
			Filename = SuggestedFilename;
			DialogResult = true;
		}

		private void SaveAs_Click(object sender, RoutedEventArgs e)
		{
			SaveAsDialog.FileName = Filename;
			
			if (SaveAsDialog.ShowDialog(Owner).GetValueOrDefault(false))
			{
				Filename = SaveAsDialog.FileName;
				if (!SaveAsDialog.CheckFileExists && File.Exists(Filename))
				{
					// Reshow the warning dialog for the new filename, which also needs overwrite confirmation
					Filename = OverwriteExistingWarning.Show(Filename, SaveAsDialog, Owner);
				}
				
				DialogResult = true;
			}
		}
	}
}
