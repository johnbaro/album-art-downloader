using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;

namespace AlbumArtDownloader.Controls
{
	public class ArtPanelList : ItemsControl
	{
		static ArtPanelList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPanelList), new FrameworkPropertyMetadata(typeof(ArtPanelList)));

			PropertyMetadata baseMetadata = ItemsSourceProperty.GetMetadata(typeof(ItemsControl));
			System.Diagnostics.Debug.Assert(baseMetadata.CoerceValueCallback == null, "Not expecting any pre-existing coercion");
			ItemsSourceProperty.OverrideMetadata(typeof(ArtPanelList), new FrameworkPropertyMetadata(baseMetadata.DefaultValue, baseMetadata.PropertyChangedCallback, new CoerceValueCallback(CoerceItemsSource)));
		}

		public ArtPanelList()
		{
			CommandBindings.Add(new CommandBinding(EditingCommands.AlignJustify, new ExecutedRoutedEventHandler(AlignJustifyCommandHandler)));
			AddHandler(ArtPanel.FullSizeImageRequestedEvent, new RoutedEventHandler(OnFullSizeImageRequested));
		}

		#region Mouse shifting
		private Point mPanelResizeDragOffset;
		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			base.OnGotMouseCapture(e);

			Suspended = true;

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

			Suspended = false;
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

		#region Auto Size panels
		private void AlignJustifyCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			AutoSizePanels();
		}

		public void AutoSizePanels()
		{
			//Auto-size the panel widths by finding the closest width that fits neatly
			if (ItemsPresenter != null)
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
		#endregion

		#region Full Size Image download
		private double mPreviousImageWidth, mPreviousImageHeight;
		private void OnFullSizeImageRequested(object sender, RoutedEventArgs e)
		{
			IAlbumArt art = ((IAlbumArt)ItemContainerGenerator.ItemFromContainer(((FrameworkElement)e.OriginalSource).TemplatedParent));
			mPreviousImageWidth = art.ImageWidth;
			mPreviousImageHeight = art.ImageHeight;
			art.RetrieveFullSizeImage(new System.Threading.WaitCallback(OnFullSizeImageDownloaded));
		}
		private void OnFullSizeImageDownloaded(object sender)
		{
			IAlbumArt art = (IAlbumArt)sender;
			if (art.ImageWidth != mPreviousImageWidth ||
				art.ImageHeight != mPreviousImageHeight)
			{
				RefreshFilter();
			}
		}
		#endregion

		#region Properties
		public static readonly DependencyProperty UseMinimumImageSizeProperty = DependencyProperty.Register("UseMinimumImageSize", typeof(bool), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>Whether to use the Minimum size setting.</summary>		
		public bool UseMinimumImageSize
		{
			get { return (bool)GetValue(UseMinimumImageSizeProperty); }
			set { SetValue(UseMinimumImageSizeProperty, value); }
		}
		public static readonly DependencyProperty UseMaximumImageSizeProperty = DependencyProperty.Register("UseMaximumImageSize", typeof(bool), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>Whether to use the Maximum size setting.</summary>		
		public bool UseMaximumImageSize
		{
			get { return (bool)GetValue(UseMaximumImageSizeProperty); }
			set { SetValue(UseMaximumImageSizeProperty, value); }
		}
		public static readonly DependencyProperty MinimumImageSizeProperty = DependencyProperty.Register("MinimumImageSize", typeof(int), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>The minimum size of image to display. Images smaller than this will be filtered out.</summary>		
		public int MinimumImageSize
		{
			get { return (int)GetValue(MinimumImageSizeProperty); }
			set { SetValue(MinimumImageSizeProperty, value); }
		}
		public static readonly DependencyProperty MaximumImageSizeProperty = DependencyProperty.Register("MaximumImageSize", typeof(int), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>The Maximum size of image to display. Images larger than this will be filtered out.</summary>		
		public int MaximumImageSize
		{
			get { return (int)GetValue(MaximumImageSizeProperty); }
			set { SetValue(MaximumImageSizeProperty, value); }
		}
		private static void OnImageSizeLimitChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;

			if (!artPanelList.UseMaximumImageSize && !artPanelList.UseMinimumImageSize)
			{
				artPanelList.Items.Filter = null; //No filtering required
			}
			else
			{
				artPanelList.Items.Filter = new Predicate<object>(artPanelList.SizeFilter); //Apply the new minimum image size filter
			}
		}
		private bool SizeFilter(object item)
		{
			IAlbumArt albumArt = item as IAlbumArt;
			if (albumArt == null)
				return true; //Can't filter it, don't know what it is

			if (!UseMaximumImageSize && !UseMinimumImageSize)
				return true; //No filtering required

			//Both width and height must be bigger, so use the smallest of the two
			double size = Math.Min(albumArt.ImageWidth, albumArt.ImageHeight);
			if (size == -1 || albumArt.ImageHeight == -1)
			{
				//No size has been provided, so filter on the actual thumbnail size
				size = Math.Min(albumArt.Image.Width, albumArt.Image.Height);
			}

			//Valid if there is no limit specified, or the size is within the limit. Both limits must apply if both are present
			return (!UseMinimumImageSize || size >= MinimumImageSize) &&
				   (!UseMaximumImageSize || size <= MaximumImageSize);
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
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ||
				Keyboard.GetKeyStates(Key.Left) == KeyStates.Down ||
				Keyboard.GetKeyStates(Key.Up) == KeyStates.Down ||
				Keyboard.GetKeyStates(Key.Right) == KeyStates.Down ||
				Keyboard.GetKeyStates(Key.Down) == KeyStates.Down)
			{
				//Return the value unsnapped if Shift is held down (to mean unsnapped)
				//or if any of the direction keys are pressed (which means this is being
				//adjusted via the keyboard)
				return newValue;
			}
			ArtPanelList artPanelList = (ArtPanelList)sender;
			if (artPanelList.PanelWidthSnapping > 0 && artPanelList.ItemsPresenter != null)
			{
				//Check if the panel width is close (within PanelWidthSnapping) to a neat arrangment
				double value = (double)newValue;
				double nearestArrangedPanelWidth = artPanelList.GetNearestArrangedPanelWidth(value);
				if (Math.Abs(nearestArrangedPanelWidth - value) <= artPanelList.PanelWidthSnapping)
					return nearestArrangedPanelWidth;
			}
			//Return the value un-coerced
			return newValue;
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

		public static readonly DependencyProperty InformationLocationProperty = DependencyProperty.Register("InformationLocation", typeof(InformationLocation), typeof(ArtPanelList), new FrameworkPropertyMetadata(InformationLocation.Right, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		/// <summary>Where to position the information, relative to the thumbnail</summary>
		public InformationLocation InformationLocation
		{
			get { return (InformationLocation)GetValue(InformationLocationProperty); }
			set { SetValue(InformationLocationProperty, value); }
		}

		private static object CoerceItemsSource(DependencyObject sender, object newValue)
		{
			IList itemsSource = newValue as IList;
			if (itemsSource != null && itemsSource is INotifyCollectionChanged && !(itemsSource is SuspendedNotificationCollection))
			{
				return new SuspendedNotificationCollection(itemsSource);
			}
			return newValue;
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

		#region Suspension and resuming of modifications to the list
		//TODO: Should this do ref counting? How about an IDisposable suspension pattern?
		//Currently this is only used from Mouse Captured and Lost Capture, so isn't necessary.
		private bool Suspended
		{
			get
			{
				SuspendedNotificationCollection suspender = ItemsSource as SuspendedNotificationCollection;
				if (suspender != null)
					return suspender.Suspended;

				return false;
			}
			set
			{
				SuspendedNotificationCollection suspender = ItemsSource as SuspendedNotificationCollection;
				if (suspender != null)
					suspender.Suspended = value;

				if (!value && mNeedsRefresh)
				{
					RefreshFilter();
				}
			}
		}

		private bool mNeedsRefresh;
		private void RefreshFilter()
		{
			if (Suspended)
			{
				mNeedsRefresh = true; //Perform the refresh when suspension is lifted
			}
			else
			{
				//Perform it immediately
				Items.Filter = Items.Filter; //Causes a refresh. Note that .Refresh doesn't.
				mNeedsRefresh = false;
			}
		}
		#endregion
		/// <summary>
		/// Wrapper around an <see cref="INotifyCollectionChanged"/> that can suspend
		/// the notifications that the collection has changed, batch them up, then
		/// release them when the suspension is lifted. The collection must also implement
		/// <see cref="IList"/>.
		/// </summary>
		private class SuspendedNotificationCollection : INotifyCollectionChanged, IList
		{
			public event NotifyCollectionChangedEventHandler CollectionChanged;

			private IList mWrappedCollection;
			private Queue<NotifyCollectionChangedEventArgs> mQueuedChanges = new Queue<NotifyCollectionChangedEventArgs>();

			public SuspendedNotificationCollection(IList collection)
			{
				mWrappedCollection = collection;

				INotifyCollectionChanged notify = collection as INotifyCollectionChanged;
				if (notify == null)
					throw new ArgumentException("The collection must implement INotifyCollectionChanged", "collection");
				
				notify.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBaseCollectionChanged);
			}

			private void OnBaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (Suspended)
				{
					//Batch up the change for later use
					mQueuedChanges.Enqueue(e);
				}
				else
				{
					//Pass it on immediately
					RaiseCollectionChanged(e);
				}
			}

			private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				NotifyCollectionChangedEventHandler temp = CollectionChanged;
				if (temp != null)
				{
					temp(mWrappedCollection, e);
				}
			}

			private bool mSuspended;
			public bool Suspended
			{
				get { return mSuspended; }
				set 
				{
					if (value != mSuspended)
					{
						mSuspended = value;
						if (!mSuspended)
						{
							//Raise all the enqued changes
							while (mQueuedChanges.Count > 0)
							{
								RaiseCollectionChanged(mQueuedChanges.Dequeue());
							}
						}
					}
				}
			}

			#region IList wrappers
			public int Add(object value) { return mWrappedCollection.Add(value); }
			public void Clear() { mWrappedCollection.Clear(); }
			public bool Contains(object value) { return mWrappedCollection.Contains(value); }
			public int IndexOf(object value) { return mWrappedCollection.IndexOf(value); }
			public void Insert(int index, object value) { mWrappedCollection.Insert(index, value); }
			public bool IsFixedSize { get { return mWrappedCollection.IsFixedSize; } }
			public bool IsReadOnly { get { return mWrappedCollection.IsReadOnly; } }
			public void Remove(object value) { mWrappedCollection.Remove(value); }
			public void RemoveAt(int index) { mWrappedCollection.RemoveAt(index); }
			public void CopyTo(Array array, int index) { mWrappedCollection.CopyTo(array, index); }
			public int Count { get { return mWrappedCollection.Count; } }
			public bool IsSynchronized { get { return mWrappedCollection.IsSynchronized; } }
			public object SyncRoot { get { return mWrappedCollection.SyncRoot; } }
			public IEnumerator GetEnumerator() { return mWrappedCollection.GetEnumerator(); }
			public object this[int index]
			{
				get
				{
					return mWrappedCollection[index];
				}
				set
				{
					mWrappedCollection[index] = value;
				}
			}
			#endregion
		}
	}
}
