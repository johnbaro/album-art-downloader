using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name="PART_MenuDropper", Type=typeof(ButtonBase))]
	public class SplitButton : Button
	{
		static SplitButton()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
		}

		public override void  OnApplyTemplate()
		{
 			base.OnApplyTemplate();

			ButtonBase menuDropper = Template.FindName("PART_MenuDropper", this) as ButtonBase;
			if(menuDropper != null)
			{
				menuDropper.Click += new RoutedEventHandler(OnMenuDropperClicked);
			}
		}

		private void OnMenuDropperClicked(object sender, RoutedEventArgs e)
		{
 			if(Menu != null)
			{
				Menu.PlacementTarget = this;
				Menu.Placement = PlacementMode.Bottom;
				Menu.IsOpen = true;
			}
		}

		#region MenuToolTip
		public static readonly DependencyProperty MenuToolTipProperty = DependencyProperty.Register("MenuToolTip", typeof(object), typeof(SplitButton), new FrameworkPropertyMetadata(null));

		/// <summary>
		/// The tooltip to display over the menu dropper part of the button.
		/// </summary>
		public object MenuToolTip
		{
			get { return (object)GetValue(MenuToolTipProperty); }
			set { SetValue(MenuToolTipProperty, value); }
		}
		#endregion

		#region Menu
		public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(SplitButton), new FrameworkPropertyMetadata(null));

		/// <summary>
		/// The menu to displaye when the menu dropper part of the button is clicked
		/// </summary>
		public ContextMenu Menu
		{
			get { return (ContextMenu)GetValue(MenuProperty); }
			set { SetValue(MenuProperty, value); }
		}
		#endregion

		#region ShowMenuDroppper
		public static readonly DependencyProperty ShowMenuDroppperProperty = DependencyProperty.Register("ShowMenuDroppper", typeof(Boolean), typeof(SplitButton), new FrameworkPropertyMetadata(true));

		public Boolean ShowMenuDroppper
		{
			get { return (Boolean)GetValue(ShowMenuDroppperProperty); }
			set { SetValue(ShowMenuDroppperProperty, value); }
		}
		#endregion
	}
}
