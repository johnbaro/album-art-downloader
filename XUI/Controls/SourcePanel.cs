using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AlbumArtDownloader.Controls
{
	public class SourcePanel : Control
	{
		static SourcePanel()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SourcePanel), new FrameworkPropertyMetadata(typeof(SourcePanel)));
		}

		public SourcePanel()
		{
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopCommandHandler)));
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ISource), typeof(SourcePanel));
		public ISource Source
		{
			get { return (ISource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		private void StopCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			Source.AbortSearch();
		}

		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Space)
			{
				Source.IsEnabled = !Source.IsEnabled;
			}
			base.OnKeyDown(e);
		}

		protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonUp(e);
			if (!(e.OriginalSource is Button)) //Button commands are screwed by focus change, it appears
			{
				Focus();
			}
		}
	}
}
