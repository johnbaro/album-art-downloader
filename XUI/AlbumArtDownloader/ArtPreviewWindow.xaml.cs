using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AlbumArtDownloader.Controls;
using System.Windows.Controls;

namespace AlbumArtDownloader
{
	public partial class ArtPreviewWindow : System.Windows.Window
	{
		/// <summary>This value is how close to 1:1 the zoom has to be before being snapped to 1:1</summary>
		private static readonly double sZoomSnapping = 1.2;
		/// <summary>The factor by which the zoom is changed by the zoom in or out buttons</summary>
		private static readonly double sZoomButtonFactor = 1.5; //NOTE: Must be greater than sZoomSnapping
		/// <summary>The factor by which the zoom is changed by the mouse wheel with Ctrl held down</summary>
		private static readonly double sZoomWheelFactor = 1.3; //NOTE: Must be greater than sZoomSnapping
		
		public ArtPreviewWindow()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.IncreaseZoom, new ExecutedRoutedEventHandler(IncreaseZoomExec)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.DecreaseZoom, new ExecutedRoutedEventHandler(DecreaseZoomExec)));

			mImageScroller.PreviewMouseWheel += new MouseWheelEventHandler(OnMouseWheel);
			mImageScroller.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnMouseDown);
			mImageScroller.MouseMove += new MouseEventHandler(OnMouseMove);
			mImageScroller.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnMouseUp);

			//DEBUG: Create a test album art
			AlbumArt albumArt = new AlbumArt(new LocalFilesSource(), new System.Drawing.Bitmap(@"C:\Documents and Settings\Family Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testFullsize.png"), "Test", 100, 100, null);
			albumArt.DefaultFilePathPattern = "Hello";
			AlbumArt = albumArt;
		}

		#region Drag panning
		private Point? mPreviousMousePosition;
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource is Image)
			{
				mPreviousMousePosition = e.GetPosition(mImageScroller);
				mImageScroller.CaptureMouse();
				e.Handled = true;
			}
		}
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && mPreviousMousePosition.HasValue)
			{
				Point mousePosition = e.GetPosition(mImageScroller);
				Vector offset = mousePosition - mPreviousMousePosition.Value;
				mPreviousMousePosition = mousePosition;

				mImageScroller.ScrollToHorizontalOffset(mImageScroller.HorizontalOffset - offset.X);
				mImageScroller.ScrollToVerticalOffset(mImageScroller.VerticalOffset - offset.Y);
			}
		}
		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			mPreviousMousePosition = null;
			mImageScroller.ReleaseMouseCapture();
		}
		#endregion

		#region Command Handlers
		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Save album art, add a handler to close once saved.
		}
		private void IncreaseZoomExec(object sender, ExecutedRoutedEventArgs e)
		{
			Zoom *= sZoomButtonFactor;
		}
		private void DecreaseZoomExec(object sender, ExecutedRoutedEventArgs e)
		{
			Zoom /= sZoomButtonFactor;
		}
		#endregion

		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				//Zoom
				if(e.Delta > 0)
					Zoom *= sZoomWheelFactor;
				else if(e.Delta < 0)
					Zoom /= sZoomWheelFactor;

				e.Handled = true;
			}
			else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				//The handling for vertical scrolling is to scroll 3 lines up or down.
				//Do the equivalent for left and right here.
				if (e.Delta > 0)
				{
					mImageScroller.LineLeft();
					mImageScroller.LineLeft();
					mImageScroller.LineLeft();
				}
				else if (e.Delta < 0)
				{
					mImageScroller.LineRight();
					mImageScroller.LineRight();
					mImageScroller.LineRight();
				}

				e.Handled = true;
			}
		}

		public static readonly DependencyProperty AlbumArtProperty = DependencyProperty.Register("AlbumArt", typeof(IAlbumArt), typeof(ArtPreviewWindow));
		/// <summary>The AlbumArt to preview</summary>
		public IAlbumArt AlbumArt
		{
			get { return (IAlbumArt)GetValue(AlbumArtProperty); }
			set { SetValue(AlbumArtProperty, value); }
		}

		public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(ArtPreviewWindow), new FrameworkPropertyMetadata(1D, new PropertyChangedCallback(OnZoomChanged), new CoerceValueCallback(CoerceZoom)));
		/// <summary>The Zoom to preview</summary>
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		private static object CoerceZoom(DependencyObject sender, object baseValue)
		{
			//Snap to zoom 1:1, if within sZoomSnapping
			double zoomRatio = (double)baseValue;
			//Do the equivalent of Math.Abs, for a ratio
			if (zoomRatio < 1)
				zoomRatio = 1 / zoomRatio;

			if (zoomRatio < sZoomSnapping)
				return 1D;

			return baseValue;
		}
		private static void OnZoomChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ScrollViewer scrollViewer = ((ArtPreviewWindow)sender).mImageScroller;

			double deltaZoom = (double)e.NewValue / (double)e.OldValue;
			if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
			{
				double halfViewportWidth = scrollViewer.ViewportWidth / 2;
				scrollViewer.ScrollToHorizontalOffset((scrollViewer.HorizontalOffset + halfViewportWidth) * deltaZoom - halfViewportWidth);
			}
			if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
			{
				double halfViewportHeight = scrollViewer.ViewportHeight / 2;
				scrollViewer.ScrollToVerticalOffset((scrollViewer.VerticalOffset + halfViewportHeight) * deltaZoom - halfViewportHeight);
			}
		}
	}
}