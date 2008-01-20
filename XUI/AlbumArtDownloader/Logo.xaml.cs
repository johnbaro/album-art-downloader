using System;
using System.Reflection;
using System.Windows.Input;

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
				try
				{
					System.Diagnostics.Process.Start((string)e.Parameter);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Could open web address: {0}\n\t{1}", e.Parameter, ex.Message);
				}
			}
		}
	}
}