using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name = "PART_ImageDisplay", Type = typeof(Image))]
	[TemplatePart(Name = "PART_ImagePopup", Type = typeof(Popup))]
	[TemplatePart(Name = "PART_ImageResizer", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_ImageArea", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_PanelResizer", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_Information", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_FilePathDisplay", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_FilePathEditor", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_FilePathBrowse", Type = typeof(Button))]
	[TemplatePart(Name = "PART_FilePathTextBox", Type = typeof(TextBox))]
	internal class ArtPanel : Control
	{
		[System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
		private static extern int GetDoubleClickTime(); 

		private static readonly double sKeyboardSizingStep = 5d;

		static ArtPanel()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPanel), new FrameworkPropertyMetadata(typeof(ArtPanel)));
		}

		public ArtPanel()
		{
			CommandBindings.Add(new CommandBinding(ArtPanelList.Commands.ToggleInformationLocation, new ExecutedRoutedEventHandler(ToggleInformationLocationExec)));

			SetBinding(ImagePopupWidthProperty, new Binding()
			{
				Source = this,
				Path = new PropertyPath("AlbumArt.ImageWidth"),
				Mode = BindingMode.OneWay
			});
			SetBinding(ImagePopupHeightProperty, new Binding()
			{
				Source = this,
				Path = new PropertyPath("AlbumArt.ImageHeight"),
				Mode = BindingMode.OneWay
			});
			//IsSaved held separately to the AlbumArt.IsSaved as it can be coerced based on whether the file path editor is active
			SetBinding(IsSavedProperty, new Binding()
			{
				Source = this,
				Path = new PropertyPath("AlbumArt.IsSaved"),
				Mode = BindingMode.OneWay
			});
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (Information != null)
			{
				Information.LayoutUpdated += new EventHandler(SetMinimumGridSizeForLabelColumn);
				SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
			}

			if (FilePathDisplay != null && FilePathTextBox != null)
			{
				FilePathDisplay.TextInput += new TextCompositionEventHandler(FilePathDisplay_TextInput);
				FilePathDisplay.MouseLeftButtonDown += new MouseButtonEventHandler(FilePathDisplay_MouseLeftButtonDown);
				FilePathTextBox.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(FilePathTextBox_LostKeyboardFocus);
				FilePathBrowse.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(FilePathBrowse_LostKeyboardFocus);
				FilePathTextBox.KeyDown += new KeyEventHandler(FilePathTextBox_KeyDown);
				FilePathTextBox.TextChanged += new TextChangedEventHandler(FilePathTextBox_TextChanged);
			}

			if (ImageResizer != null)
			{
				ImageResizer.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ImageResizer_PreviewMouseDown);
				ImageResizer.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ImageResizer_PreviewMouseUp);
				ImageResizer.PreviewMouseMove += new MouseEventHandler(ImageResizer_PreviewMouseMove);
			}

			if (PanelResizer != null)
			{
				PanelResizer.PreviewKeyDown += new KeyEventHandler(PanelResizer_PreviewKeyDown);
				PanelResizer.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PanelResizer_PreviewMouseDown);
				PanelResizer.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PanelResizer_PreviewMouseUp);
				PanelResizer.PreviewMouseMove += new MouseEventHandler(PanelResizer_PreviewMouseMove);
			}

			if (FilePathBrowse != null)
			{
				FilePathBrowse.Click += new RoutedEventHandler(FilePathBrowse_Click);
			}

			if (ImageDisplay != null)
			{
				ImageDisplay.PreviewKeyDown += new KeyEventHandler(ImageDisplay_PreviewKeyDown);
				
				if (ImagePopup != null)
				{
					ImageDisplay.PreviewKeyUp += new KeyEventHandler(ImageDisplay_PreviewKeyUp);
					ImageDisplay.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ImageDisplay_PreviewMouseLeftButtonDown);
					ImageDisplay.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ImageDisplay_PreviewMouseLeftButtonUp);
					ImagePopup.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ImagePopup_PreviewMouseLeftButtonUp);
				}
			}

			Binding minWidthBinding = new Binding();
			minWidthBinding.Source = this.GetVisualChild(0);
			minWidthBinding.Path = new PropertyPath(Grid.MinWidthProperty);
			minWidthBinding.Mode = BindingMode.OneWay;
			SetBinding(MinWidthProperty, minWidthBinding);
		}

		private void ImageDisplay_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
				ShowImagePopup();
			}
			else if (e.Key == Key.Left || e.Key == Key.Up)
			{
				if (InformationLocation == InformationLocation.Bottom)
				{
					//Resize the panel instead
					Width = Math.Max(0, Width - sKeyboardSizingStep);
					UpdateWidthFromBinding();
				}
				else
				{
					ThumbSize = Math.Max(0, ThumbSize - sKeyboardSizingStep);
				}
				e.Handled = true;
			}
			else if (e.Key == Key.Right || e.Key == Key.Down)
			{
				if (InformationLocation == InformationLocation.Bottom)
				{
					//Resize the panel instead
					Width += sKeyboardSizingStep;
					UpdateWidthFromBinding();
				}
				else
				{
					ThumbSize += sKeyboardSizingStep;
				}
				e.Handled = true;
			}
		}

		private void ToggleInformationLocationExec(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Should this set the information location based on the command parameter rather than just switching?
			if (InformationLocation == InformationLocation.Right)
			{
				InformationLocation = InformationLocation.Bottom;
			}
			else
			{
				InformationLocation = InformationLocation.Right;
			}
		}

		#region Image Popup
		private void ImageDisplay_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
				CloseImagePopup();
			}
		}

		private void ImageDisplay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				//This is a double click, so treat as a Save.
				ApplicationCommands.Save.Execute(null, this);
			}
			else
			{
				ImageDisplay.CaptureMouse();
				ShowImagePopup();
			}
		}
		
		private void ImageDisplay_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CloseImagePopup();
			//Delay to ensure that this isn't about to be a double click
			ThreadPool.QueueUserWorkItem(new WaitCallback(DelayedRelease));
		}
		private void DelayedRelease(object state)
		{
			Thread.Sleep(GetDoubleClickTime());

			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(delegate
			{
				if (Mouse.LeftButton != MouseButtonState.Pressed)
				{
					ImageDisplay.ReleaseMouseCapture();
				}
			}));
		}

		private void ImagePopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CloseImagePopup();
		}

		private void ShowImagePopup()
		{
			AlbumArt.RetrieveFullSizeImage();
			if (ImagePopup != null && !ImagePopup.IsOpen)
			{
				CoerceValue(ImagePopupWidthProperty);
				CoerceValue(ImagePopupHeightProperty);
				ImagePopup.IsOpen = true;
			}
		}


		private void CloseImagePopup()
		{
			if (ImagePopup != null && ImagePopup.IsOpen)
			{
				ImagePopup.IsOpen = false;
			}
		}

		public static readonly DependencyProperty ImagePopupWidthProperty = DependencyProperty.Register("ImagePopupWidth", typeof(double), typeof(ArtPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnImagePopupWidthChanged), new CoerceValueCallback(CoerceImagePopupWidth)));
		/// <summary>The width to show the image in the popup (restricted on screen size)</summary>
		public double ImagePopupWidth
		{
			get { return (double)GetValue(ImagePopupWidthProperty); }
			set { SetValue(ImagePopupWidthProperty, value); }
		}
		private static void OnImagePopupWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			//When the width changes, the height needs to be re-coerced too, as it is dependent on aspect ratio
			sender.CoerceValue(ArtPanel.ImagePopupHeightProperty);
		}
		private static object CoerceImagePopupWidth(DependencyObject sender, object value)
		{
			double maxWidth = ((ArtPanel)sender).CalculateMaxImagePopupSize().Width;
			if ((double)value > maxWidth)
				value = maxWidth;

			return value;
		}

		public static readonly DependencyProperty ImagePopupHeightProperty = DependencyProperty.Register("ImagePopupHeight", typeof(double), typeof(ArtPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnImagePopupHeightChanged), new CoerceValueCallback(CoerceImagePopupHeight)));
		/// <summary>The height to show the image in the popup (restricted on screen size)</summary>		
		public double ImagePopupHeight
		{
			get { return (double)GetValue(ImagePopupHeightProperty); }
			set { SetValue(ImagePopupHeightProperty, value); }
		}
		private static void OnImagePopupHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			//When the height changes, the width needs to be re-coerced too, as it is dependent on aspect ratio
			sender.CoerceValue(ArtPanel.ImagePopupWidthProperty);
		}
		private static object CoerceImagePopupHeight(DependencyObject sender, object value)
		{
			double maxHeight = ((ArtPanel)sender).CalculateMaxImagePopupSize().Height;
			if ((double)value > maxHeight)
				value = maxHeight;
			
			return value;
		}

		/// <summary>
		/// Calculates the maximum allowable image popup size, whilst keeping within the restrictions for WPF Popup, and
		/// maintaining the image aspect ratio.
		/// </summary>
		private Size CalculateMaxImagePopupSize()
		{
			//TODO: Multimonitor support

			double imageAspectRatio = 1; //If no album art yet, then assume it will be square.
			if (AlbumArt != null)
			{
				imageAspectRatio = AlbumArt.ImageWidth / AlbumArt.ImageHeight;
			}
			
			//Popup may not take up more than 75% screen area (matches WPF Popup restriction)
			double maxArea = 0.75 * SystemParameters.PrimaryScreenHeight * SystemParameters.PrimaryScreenWidth;

			//Max width, then is sqrt of max area x aspect ratio. Or screen width (less a small amount for borders), whichever is smaller.
			double maxWidth = Math.Min( SystemParameters.PrimaryScreenWidth - 10D,
										Math.Sqrt(maxArea * imageAspectRatio));

			double maxHeight = maxWidth / imageAspectRatio;
			//Don't allow the height to exceed the screen height (less a small amount for borders)
			if (maxHeight > SystemParameters.PrimaryScreenHeight - 10D)
			{
				maxHeight = SystemParameters.PrimaryScreenHeight - 10D;
				maxWidth = maxHeight * imageAspectRatio; //This should always be a decrease, as maxHeight was decreased.
			}

			return new Size(maxWidth, maxHeight);
		}
		#endregion

		#region Information sizing
		private double mMinimumGridSizeForLabelColumn;
		private void SetMinimumGridSizeForLabelColumn(object sender, EventArgs e)
		{
			mMinimumGridSizeForLabelColumn = Information.ColumnDefinitions[0].ActualWidth * 3;
			if (mMinimumGridSizeForLabelColumn > 0)
				Information.LayoutUpdated -= SetMinimumGridSizeForLabelColumn; //Unhook after successfully finding the width.
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (Information != null)
			{
				//Check to see whether there is enough space to show the label column
				if (Information.ActualWidth < mMinimumGridSizeForLabelColumn)
				{
					if (Information.ColumnDefinitions[0].Width.IsAuto)
						Information.ColumnDefinitions[0].Width = new GridLength(0);
				}
				else
				{
					if (!Information.ColumnDefinitions[0].Width.IsAuto)
						Information.ColumnDefinitions[0].Width = GridLength.Auto;
				}
			}
			if (InformationLocation == InformationLocation.Bottom)
			{
				CoerceValue(ThumbSizeProperty);
			}
		}
		#endregion

		#region Thumbnail resizing
		Vector mImageResizeOffset;
		private void ImageResizer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement parent = (FrameworkElement)((FrameworkElement)sender).Parent;

			mImageResizeOffset = (Vector)e.GetPosition(parent) - new Vector(ThumbSize, ThumbSize);
			((FrameworkElement)sender).CaptureMouse();
		}
		private void ImageResizer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			((FrameworkElement)sender).ReleaseMouseCapture();
		}
		private void ImageResizer_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				FrameworkElement parent = (FrameworkElement)((FrameworkElement)sender).Parent;
				Point mousePos = e.GetPosition(parent) - mImageResizeOffset;

				if (InformationLocation == InformationLocation.Bottom)
				{
					//Resize the whole panel instead
					double delta = Math.Max(0, Math.Max(mousePos.X, mousePos.Y)) - ThumbSize;
					//Do not allow zero or negative width
					Width = Math.Max(1, Width + delta);
					UpdateWidthFromBinding();				
				}
				else
				{
					//Keep square aspect ratio, don't have negative size.
					ThumbSize = Math.Max(0, Math.Max(mousePos.X, mousePos.Y));
				}
			}
		}
		#endregion

		#region Panel Resizing
		double mPanelResizeOffset;
		private void PanelResizer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount > 1)
			{
				//Double click, raise the auto-size command
				EditingCommands.AlignJustify.Execute(null, this);
			}
			else
			{
				FrameworkElement parent = (FrameworkElement)((FrameworkElement)sender).Parent;
				mPanelResizeOffset = e.GetPosition(parent).X - ActualWidth;

				((FrameworkElement)sender).CaptureMouse();
			}
		}
		private void PanelResizer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			((FrameworkElement)sender).ReleaseMouseCapture();
		}
		private void PanelResizer_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && ((FrameworkElement)sender).IsMouseCaptured)
			{
				FrameworkElement parent = (FrameworkElement)((FrameworkElement)sender).Parent;
				Width = Math.Max(0, e.GetPosition(parent).X - mPanelResizeOffset);
				UpdateWidthFromBinding();				
			}
		}
		private void PanelResizer_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Left)
			{
				Width = Math.Max(0, Width - sKeyboardSizingStep);
				UpdateWidthFromBinding();
				e.Handled = true;
			}
			else if (e.Key == Key.Right)
			{
				Width += sKeyboardSizingStep;
				UpdateWidthFromBinding();
				e.Handled = true;
			}
		}
		private void UpdateWidthFromBinding()
		{
			BindingExpression bindingExpression = GetBindingExpression(WidthProperty);
			if (bindingExpression != null)
			{
				bindingExpression.UpdateTarget(); //Update in case of snapping
			}
		}
		#endregion

		#region File path editing
		private void FilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CoerceValue(IsSavedProperty);
		}

		private void FilePathDisplay_TextInput(object sender, TextCompositionEventArgs e)
		{
			ShowFilePathEdit();

			if (e.Text != " " && e.Text != "\r" && FilePathTextBox != null)
			{
				FilePathTextBox.Text = e.Text;
				FilePathTextBox.CaretIndex = e.Text.Length;
			}
			e.Handled = true;
		}

		private void FilePathDisplay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			ShowFilePathEdit();
		}
		private void FilePathTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != null && e.NewFocus == FilePathBrowse)
			{
				//The only other thing that can be focused without closing the editor
				return;
			}
			HideFilePathEdit(true);
		}
		private void FilePathBrowse_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != null && e.NewFocus == FilePathTextBox)
			{
				//The only other thing that can be focused without closing the editor
				return;
			}
			HideFilePathEdit(true);
		}
		private void FilePathTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				//Cancel the edit
				HideFilePathEdit(false);
				e.Handled = true;
			}
			else if (e.Key == Key.Enter)
			{
				//Confirm the edit
				HideFilePathEdit(true);
				e.Handled = true;
			}
		}

		private void ShowFilePathEdit()
		{
			if (FilePathDisplay != null && FilePathEditor != null)
			{
				FilePathDisplay.Visibility = Visibility.Hidden;
				FilePathEditor.Visibility = Visibility.Visible;
			}
			if (FilePathTextBox != null)
			{
				FilePathTextBox.Text = AlbumArt.FilePath;
				FilePathTextBox.SelectAll();
				FilePathTextBox.Focus();
			}
		}

		private void HideFilePathEdit(bool confirm)
		{
			if (FilePathDisplay != null && FilePathEditor != null)
			{
				if (confirm)
					AlbumArt.FilePath = FilePathTextBox.Text;

				FilePathDisplay.Visibility = Visibility.Visible;
				FilePathEditor.Visibility = Visibility.Hidden;
			}
			CoerceValue(IsSavedProperty);
		}

		private void FilePathBrowse_Click(object sender, RoutedEventArgs e)
		{
			HideFilePathEdit(true);
			ApplicationCommands.SaveAs.Execute(null, this);
		}
		#endregion

		#region Focus
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			Focus();
		}
		#endregion

		#region Properties
		public static readonly DependencyProperty AlbumArtProperty = DependencyProperty.Register("AlbumArt", typeof(AlbumArt), typeof(ArtPanel));
		/// <summary>The AlbumArt that this panel displays</summary>
		public AlbumArt AlbumArt
		{
			get { return (AlbumArt)GetValue(AlbumArtProperty); }
			set { SetValue(AlbumArtProperty, value); }
		}

		public static readonly DependencyProperty IsSavedProperty = DependencyProperty.Register("IsSaved", typeof(bool), typeof(ArtPanel), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback(CoerceIsSaved)));
		/// <summary>True if the image has already been saved to the location specified by <see cref="FilePath"/></summary>
		public bool IsSaved
		{
			get { return (bool)GetValue(IsSavedProperty); }
			set { SetValue(IsSavedProperty, value); }
		}
		private static object CoerceIsSaved(DependencyObject sender, object value)
		{
			ArtPanel artPanel = (ArtPanel)sender;
			if (artPanel.FilePathEditor != null && artPanel.FilePathTextBox != null &&
				artPanel.FilePathEditor.Visibility == Visibility.Visible && artPanel.FilePathTextBox.Text != artPanel.AlbumArt.FilePath)
				return false; //Always appear unsaved when editing

			return value;
		}

		public static readonly DependencyProperty ThumbSizeProperty = DependencyProperty.Register("ThumbSize", typeof(double), typeof(ArtPanel), new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, new CoerceValueCallback(CoerceThumbSize)));
		/// <summary>The size to show the thumbnail at</summary>
		public double ThumbSize
		{
			get { return (double)GetValue(ThumbSizeProperty); }
			set { SetValue(ThumbSizeProperty, value); }
		}
		
		private static object CoerceThumbSize(DependencyObject sender, object value)
		{
			ArtPanel artPanel = (ArtPanel)sender;
			if(artPanel.InformationLocation == InformationLocation.Bottom && artPanel.ActualHeight > 0)
			{
				//Take up the full width available
				//(This has to match the Grid MinWidth calculation)
				return artPanel.ActualWidth - artPanel.PanelResizer.ActualWidth - 10;
			}
			return value;
		}

		public static readonly DependencyProperty InformationLocationProperty = DependencyProperty.Register("InformationLocation", typeof(InformationLocation), typeof(ArtPanel), new FrameworkPropertyMetadata(InformationLocation.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnInformationLocationChanged)));
		/// <summary>Where to position the information, relative to the thumbnail</summary>
		public InformationLocation InformationLocation
		{
			get { return (InformationLocation)GetValue(InformationLocationProperty); }
			set { SetValue(InformationLocationProperty, value); }
		}
		private static void OnInformationLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanel artPanel = (ArtPanel)sender;
			artPanel.CoerceValue(ThumbSizeProperty);
			if ((InformationLocation)e.NewValue == InformationLocation.Right)
			{
				//If switching to Right, and the Information panel has disappeared, make sure it is at least visible
				if (artPanel.Width - artPanel.ThumbSize < 30)
				{
					artPanel.Width = artPanel.ThumbSize + artPanel.mMinimumGridSizeForLabelColumn + 10;
				}
			}
		}

		public static readonly DependencyProperty PresetsContextMenuProperty = DependencyProperty.Register("PresetsContextMenu", typeof(ContextMenu), typeof(ArtPanel), new FrameworkPropertyMetadata(null));
		/// <summary>The menu to display when the Save button dropper is clicked</summary>
		public ContextMenu PresetsContextMenu
		{
			get { return (ContextMenu)GetValue(PresetsContextMenuProperty); }
			set { SetValue(PresetsContextMenuProperty, value); }
		}

		#endregion

		#region Elements

		private Image mCachedImageDisplay;
		protected Image ImageDisplay
		{
			get
			{
				if (mCachedImageDisplay == null)
				{
					if (Template != null)
					{
						mCachedImageDisplay = Template.FindName("PART_ImageDisplay", this) as Image;
					}
				}

				return mCachedImageDisplay;
			}
		}

		private Popup mCachedImagePopup;
		protected Popup ImagePopup
		{
			get
			{
				if (mCachedImagePopup == null)
				{
					if (Template != null)
					{
						mCachedImagePopup = Template.FindName("PART_ImagePopup", this) as Popup;
					}
				}

				return mCachedImagePopup;
			}
		}

		private FrameworkElement mCachedImageResizer;
		protected FrameworkElement ImageResizer
		{
			get
			{
				if (mCachedImageResizer == null)
				{
					if (Template != null)
					{
						mCachedImageResizer = (FrameworkElement)Template.FindName("PART_ImageResizer", this);
					}
				}

				return mCachedImageResizer;
			}
		}

		private FrameworkElement mCachedImageArea;
		protected FrameworkElement ImageArea
		{
			get
			{
				if (mCachedImageArea == null)
				{
					if (Template != null)
					{
						mCachedImageArea = (FrameworkElement)Template.FindName("PART_ImageArea", this);
					}
				}

				return mCachedImageArea;
			}
		}

		private FrameworkElement mCachedPanelResizer;
		protected FrameworkElement PanelResizer
		{
			get
			{
				if (mCachedPanelResizer == null)
				{
					if (Template != null)
					{
						mCachedPanelResizer = (FrameworkElement)Template.FindName("PART_PanelResizer", this);
					}
				}

				return mCachedPanelResizer;
			}
		}

		private Grid mCachedInformation;
		protected Grid Information
		{
			get
			{
				if (mCachedInformation == null)
				{
					if (Template != null)
					{
						mCachedInformation = Template.FindName("PART_Information", this) as Grid;
					}
				}

				return mCachedInformation;
			}
		}

		private FrameworkElement mCachedFilePathDisplay;
		protected FrameworkElement FilePathDisplay
		{
			get
			{
				if (mCachedFilePathDisplay == null)
				{
					if (Template != null)
					{
						mCachedFilePathDisplay = (FrameworkElement)Template.FindName("PART_FilePathDisplay", this);
					}
				}

				return mCachedFilePathDisplay;
			}
		}

		private FrameworkElement mCachedFilePathEditor;
		protected FrameworkElement FilePathEditor
		{
			get
			{
				if (mCachedFilePathEditor == null)
				{
					if (Template != null)
					{
						mCachedFilePathEditor = (FrameworkElement)Template.FindName("PART_FilePathEditor", this);
					}
				}

				return mCachedFilePathEditor;
			}
		}

		private Button mCachedFilePathBrowse;
		protected Button FilePathBrowse
		{
			get
			{
				if (mCachedFilePathBrowse == null)
				{
					if (Template != null)
					{
						mCachedFilePathBrowse = Template.FindName("PART_FilePathBrowse", this) as Button;
					}
				}

				return mCachedFilePathBrowse;
			}
		}

		private TextBox mCachedFilePathTextBox;
		protected TextBox FilePathTextBox
		{
			get
			{
				if (mCachedFilePathTextBox == null)
				{
					if (Template != null)
					{
						mCachedFilePathTextBox = Template.FindName("PART_FilePathTextBox", this) as TextBox;
					}
				}

				return mCachedFilePathTextBox;
			}
		}
		#endregion
	}
}
