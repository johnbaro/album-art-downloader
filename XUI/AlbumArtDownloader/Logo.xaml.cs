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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace AlbumArtDownloader
{
	public partial class Logo : System.Windows.Controls.UserControl
	{
		public Logo()
		{
			InitializeComponent();
			CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, new ExecutedRoutedEventHandler(GoToPageExec)));
		}

		public string Version
		{
			get
			{
				return String.Format("version {0}", Assembly.GetEntryAssembly().GetName().Version);
			}
		}

		private void GoToPageExec(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Validation that this is a web address?
			if (e.Parameter is string)
			{
				System.Diagnostics.Process.Start((string)e.Parameter);
			}
		}
	}
}