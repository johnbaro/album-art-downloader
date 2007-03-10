using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;

namespace CoverDownloader.XUI
{
	public partial class Scene1
	{
		
		public Scene1()
		{
			this.InitializeComponent();

			mDescription.LayoutUpdated += new EventHandler(mDescription_LayoutUpdated);

			mFilePathDisplay.MouseLeftButtonDown += new MouseButtonEventHandler(mFilePathDisplay_MouseLeftButtonDown);
			mFilePath.LostFocus += new RoutedEventHandler(mFilePath_LostFocus);
			mFilePath.KeyDown += new KeyEventHandler(mFilePath_KeyDown);
		}

		#region Description sizing
		private double mMinimumGridSizeForLabelColumn;
		private void mDescription_LayoutUpdated(object sender, EventArgs e)
		{
			mMinimumGridSizeForLabelColumn = mDescription.ColumnDefinitions[0].ActualWidth * 3;
			if(mMinimumGridSizeForLabelColumn > 0)
				mDescription.LayoutUpdated -= mDescription_LayoutUpdated; //Unhook after successfully finding the width.
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			//Check to see whether there is enough space to show the label column
			if (mDescription.ActualWidth < mMinimumGridSizeForLabelColumn)
			{
				if (mDescription.ColumnDefinitions[0].Width.IsAuto)
					mDescription.ColumnDefinitions[0].Width = new GridLength(0);
			}
			else
			{
				if (!mDescription.ColumnDefinitions[0].Width.IsAuto)
					mDescription.ColumnDefinitions[0].Width = GridLength.Auto;
			}
		}
		#endregion

		#region Thumbnail resizing
		private void OnResizeMouseDown(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).CaptureMouse();
		}
		private void OnResizeMouseUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();
		}
		private void OnResizeMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				FrameworkElement parent = (FrameworkElement)((FrameworkElement)sender).Parent;
				Point mousePos = e.GetPosition(parent);
				
				//Keep square aspect ratio, don't have negative size.
				double size = Math.Max(0, Math.Max(mousePos.X, mousePos.Y));
				parent.Width = parent.Height = size;
			}
		}
		#endregion

		#region File path editing
		private void mFilePathDisplay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			ShowFilePathEdit();
		}
		private void mFilePath_LostFocus(object sender, RoutedEventArgs e)
		{
			HideFilePathEdit();	
		}
		private void mFilePath_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				//Cancel the edit
				HideFilePathEdit();
				e.Handled = true;
			}
			else if (e.Key == Key.Enter)
			{
				//Confirm the edit
				HideFilePathEdit();
				e.Handled = true;
			}
		}

		private void HideFilePathEdit()
		{
			mFilePathDisplay.Visibility = Visibility.Visible;
			mFilePathEditor.Visibility = Visibility.Hidden;
		}
		private void ShowFilePathEdit()
		{
			mFilePathDisplay.Visibility = Visibility.Hidden;
			mFilePathEditor.Visibility = Visibility.Visible;
			mFilePath.Focus();
		}

		#endregion

		#region Focus
		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).Focus();
		}
		#endregion
	}
}
