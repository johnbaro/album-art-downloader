using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace AlbumArtDownloader.Controls
{
	public class ArtPanelList : ItemsControl
	{
		static ArtPanelList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPanelList), new FrameworkPropertyMetadata(typeof(ArtPanelList)));
		}

		public ArtPanelList()
		{
			CommandBindings.Add(new CommandBinding(EditingCommands.AlignJustify, new ExecutedRoutedEventHandler(AlignJustifyCommandHandler)));
			AddHandler(ArtPanel.FullSizeImageRequestedEvent, new RoutedEventHandler(OnFullSizeImageRequested));
		}

		#region Mouse shifting
		private IDisposable mDeferRefresh;
		private Point mPanelResizeDragOffset;
		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			base.OnGotMouseCapture(e);

			mDeferRefresh = Items.DeferRefresh();

			ArtPanel panel = GetArtPanel(e.OriginalSource);
			if (panel != null)
			{
				mPanelResizeDragOffset = panel.TranslatePoint(new Point(), this);
				PreviewMouseMove += OnPreviewMouseMoveWhileCaptured;
			}
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			PreviewMouseMove -= OnPreviewMouseMoveWhileCaptured;
			if (mDeferRefresh != null)
			{
				mDeferRefresh.Dispose();
				mDeferRefresh = null;
			}
		}

		private void OnPreviewMouseMoveWhileCaptured(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				if (Mouse.Captured != null)
					Mouse.Captured.ReleaseMouseCapture();

				OnLostMouseCapture(e);
			}
			else
			{
				//Shift the mouse with the panel being moved, so that the resize remains relative to it.
				ArtPanel panel = GetArtPanel(e.OriginalSource);
				if (panel != null)
				{
					panel.BringIntoView(); //Ensure the panel remains in view
					Point newOffset = panel.TranslatePoint(new Point(), this);
					Vector offsetDelta = newOffset - mPanelResizeDragOffset;
					if (offsetDelta.Length > 1)
					{
						MoveMousePosition(offsetDelta);
						mPanelResizeDragOffset = newOffset;
						e.Handled = true;
					}
				}
			}
		}

		private static ArtPanel GetArtPanel(object originalSource)
		{
			//Get the art panel being resized
			FrameworkElement source = originalSource as FrameworkElement;
			if (!(source is ArtPanel)) //If the source isn't the panel itself, then it must come from some control in the panels template.
				source = source.TemplatedParent as Controls.ArtPanel;

			return (ArtPanel)source;
		}

		[DllImport("user32.dll")]
		static extern bool SetCursorPos(int X, int Y);
		[DllImport("user32.dll")]
		static extern bool GetCursorPos(ref POINTAPI lpPoint);
		private struct POINTAPI
		{
			public int X;
			public int Y;
		}
		private void MoveMousePosition(Vector offsetDelta)
		{
			POINTAPI point = new POINTAPI();
			if (GetCursorPos(ref point))
			{
				point.X += (int)offsetDelta.X;
				point.Y += (int)offsetDelta.Y;
				SetCursorPos(point.X, point.Y);
			}
		}
		#endregion

		private void AlignJustifyCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			AutoSizePanels();
		}

		public void AutoSizePanels()
		{
			//Auto-size the panel widths by finding the closest width that fits neatly
			if(ItemsPresenter != null)
			{
				PanelWidth = GetNearestArrangedPanelWidth(PanelWidth);
			}
		}

		private double GetNearestArrangedPanelWidth(double panelWidth)
		{
			int numberOfPanels = (int)Math.Round(ItemsPresenter.ActualWidth / panelWidth, MidpointRounding.AwayFromZero);
			double newPanelWidth = ItemsPresenter.ActualWidth / numberOfPanels;
			ContentPresenter firstItemContentPresenter = (ContentPresenter)ItemContainerGenerator.ContainerFromIndex(0);
			if (firstItemContentPresenter != null) //Will be null if there are no items shown.
			{
				ArtPanel firstArtPanel = firstItemContentPresenter.ContentTemplate.FindName("PART_ArtPanel", firstItemContentPresenter) as ArtPanel;
				if (numberOfPanels > 1 && newPanelWidth < firstArtPanel.MinWidth)
				{
					//Can't fit the panels at this size, as it is under the minimum size, so decrease the number of panels by one, and recalc.
					newPanelWidth = ItemsPresenter.ActualWidth / --numberOfPanels;
					//Should always now fit, as this is the equivalent of rounding up instead of down.
				}
			}
			return newPanelWidth;
		}

		private Size mPreviousSize;
		private void OnFullSizeImageRequested(object sender, RoutedEventArgs e)
		{
			IAlbumArt art = ((IAlbumArt)ItemContainerGenerator.ItemFromContainer(((FrameworkElement)e.OriginalSource).TemplatedParent));
			mPreviousSize = new Size(art.ImageWidth, art.ImageHeight);
			art.RetrieveFullSizeImage(new System.Threading.WaitCallback(OnFullSizeImageDownloaded));
		}
		private void OnFullSizeImageDownloaded(object sender)
		{
			IAlbumArt art = (IAlbumArt)sender;
			if (art.ImageWidth != mPreviousSize.Width ||
				art.ImageHeight != mPreviousSize.Height)
			{
				//Causes a refresh. Note that .Refresh doesn't.
				Items.Filter = Items.Filter;
			}
		}

		#region Properties
		public static readonly DependencyProperty MinimumImageSizeProperty = DependencyProperty.Register("MinimumImageSize", typeof(double), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(0D, new PropertyChangedCallback(OnMinimumImageSizeChanged)));
		/// <summary>The minimum size of image to display. Images smaller than this will be filtered out.</summary>		
		public double MinimumImageSize
		{
			get { return (double)GetValue(MinimumImageSizeProperty); }
			set { SetValue(MinimumImageSizeProperty, value); }
		}
		private static void OnMinimumImageSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;
			artPanelList.Items.Filter = new Predicate<object>(artPanelList.SizeFilter); //Apply the new minimum image size filter
		}
		private bool SizeFilter(object item)
		{
			IAlbumArt albumArt = item as IAlbumArt;
			if (albumArt == null)
				return true; //Can't filter it, don't know what it is

			//Both width and height must be bigger, so use the smallest of the two
			double size = Math.Min(albumArt.ImageWidth, albumArt.ImageHeight);
			if (size == -1 || albumArt.ImageHeight == -1)
			{
				//No size has been provided, so filter on the actual thumbnail size
				size = Math.Min(albumArt.Image.Width, albumArt.Image.Height);
			}

			return size >= MinimumImageSize;
		}

		public static readonly DependencyProperty ThumbSizeProperty = DependencyProperty.Register("ThumbSize", typeof(double), typeof(ArtPanelList), new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		/// <summary>The size of the thumbnail images to display</summary>		
		public double ThumbSize
		{
			get { return (double)GetValue(ThumbSizeProperty); }
			set { SetValue(ThumbSizeProperty, value); }
		}

		public static readonly DependencyProperty PanelWidthProperty = DependencyProperty.Register("PanelWidth", typeof(double), typeof(ArtPanelList), new FrameworkPropertyMetadata(150D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, new CoerceValueCallback(CoercePanelWidth)));
		/// <summary>The width of each album art panel</summary>		
		public double PanelWidth
		{
			get { return (double)GetValue(PanelWidthProperty); }
			set { SetValue(PanelWidthProperty, value); }
		}
		private static object CoercePanelWidth(DependencyObject sender, object newValue)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;
			double value = (double)newValue;
			if (artPanelList.PanelWidthSnapping > 0 && artPanelList.ItemsPresenter != null)
			{
				//Check if the panel width is close (within PanelWidthSnapping) to a neat arrangment
				double nearestArrangedPanelWidth = artPanelList.GetNearestArrangedPanelWidth(value);
				if (Math.Abs(nearestArrangedPanelWidth - value) <= artPanelList.PanelWidthSnapping)
					return nearestArrangedPanelWidth;
			}
			//Return the value un-coerced
			return value;
		}

		public static readonly DependencyProperty PanelWidthSnappingProperty = DependencyProperty.Register("PanelWidthSnapping", typeof(double), typeof(ArtPanelList), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsArrange));
		/// <summary>The width of each album art panel</summary>		
		public double PanelWidthSnapping
		{
			get { return (double)GetValue(PanelWidthSnappingProperty); }
			set { SetValue(PanelWidthSnappingProperty, value); }
		}

		public static readonly DependencyProperty SortDescriptionProperty = DependencyProperty.Register("SortDescription", typeof(SortDescription), typeof(ArtPanelList),
					new FrameworkPropertyMetadata(default(SortDescription), new PropertyChangedCallback(OnSortDescriptionChanged)));
		/// <summary>The sorting to be applied to the list</summary>		
		public SortDescription SortDescription
		{
			get { return (SortDescription)GetValue(SortDescriptionProperty); }
			set { SetValue(SortDescriptionProperty, value); }
		}
		private static void OnSortDescriptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;
			artPanelList.Items.SortDescriptions.Clear();
			artPanelList.Items.SortDescriptions.Add((SortDescription)e.NewValue);
		}
		#endregion

		#region Elements
		private ItemsPresenter mCachedItemsPresenter;
		protected ItemsPresenter ItemsPresenter
		{
			get
			{
				if (mCachedItemsPresenter == null)
				{
					if (Template != null)
					{
						mCachedItemsPresenter = Template.FindName("PART_ItemsPresenter", this) as ItemsPresenter;
					}
				}

				return mCachedItemsPresenter;
			}
		}
		#endregion
	}
}
