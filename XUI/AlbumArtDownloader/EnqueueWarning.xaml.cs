using System;
using System.Collections;
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
	public partial class EnqueueWarning : Window
	{
		public EnqueueWarning()
		{
			InitializeComponent();

			mEnqueueLimited.Click += new RoutedEventHandler(mEnqueueLimited_Click);
			mEnqueueAll.Click += new RoutedEventHandler(mEnqueueAll_Click);
		}

		public static readonly DependencyProperty NumberToEnqueueProperty = DependencyProperty.Register("NumberToEnqueue", typeof(int), typeof(EnqueueWarning));
		/// <summary>
		/// This is the number of items to be enqueued. When the dialog is shown, it should
		/// be populated with the total number of items requested to be enqueued. When the
		/// dialog is closed, this will contain the number of items the user has chosen to
		/// allow to be enqueued.
		/// </summary>
		public int NumberToEnqueue
		{
			get { return (int)GetValue(NumberToEnqueueProperty); }
			set { SetValue(NumberToEnqueueProperty, value); }
		}

		private void mEnqueueLimited_Click(object sender, RoutedEventArgs e)
		{
			//Limit the number to enqueue to the warned number
			NumberToEnqueue = Properties.Settings.Default.EnqueueWarning;
			DialogResult = true;
		}

		private void mEnqueueAll_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
