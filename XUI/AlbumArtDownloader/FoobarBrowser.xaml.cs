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
	/// <summary>
	/// Interaction logic for FoobarBrowser.xaml
	/// </summary>

	public partial class FoobarBrowser : System.Windows.Window, IAppWindow
	{

		public FoobarBrowser()
		{
			InitializeComponent();
		}


		public void SaveSettings()
		{
			
		}

		public void LoadSettings()
		{

		}

		string IAppWindow.Description
		{
			get
			{
				return "Foobar Browser";
			}
		}
	}
}