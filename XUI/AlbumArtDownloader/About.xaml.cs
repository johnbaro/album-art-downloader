using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlbumArtDownloader
{
	public partial class About : System.Windows.Window
	{
		public About()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseExec)));
		}

		private void CloseExec(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}
	}
}