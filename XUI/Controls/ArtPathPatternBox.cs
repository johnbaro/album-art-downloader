using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Win32;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name = "PART_PathEditorHistory", Type = typeof(ComboBox))]
	[TemplatePart(Name = "PART_PathEditor", Type = typeof(TextBox))]
	[TemplatePart(Name = "PART_MenuButton", Type = typeof(ToggleButton))]
	[TemplatePart(Name = "PART_BrowseMenuItem", Type = typeof(MenuItem))]
	public class ArtPathPatternBox : Control
	{
		private static readonly int sMaxHistoryLength = 8;
		public static class Commands
		{
			public static RoutedUICommand Browse = new RoutedUICommand("Browse", "Browse", typeof(Commands));
			public static RoutedUICommand InsertPlaceholder = new RoutedUICommand("Insert Placeholder", "InsertPlaceholder", typeof(Commands));
		}
		static ArtPathPatternBox()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(typeof(ArtPathPatternBox)));
		}

		public ArtPathPatternBox()
		{
			AdditionalPlaceholders.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAdditionalPlaceholdersChanged);

			CommandBindings.Add(new CommandBinding(Commands.Browse, new ExecutedRoutedEventHandler(BrowseExec), new CanExecuteRoutedEventHandler(CanBrowse)));
			CommandBindings.Add(new CommandBinding(Commands.InsertPlaceholder, new ExecutedRoutedEventHandler(InsertPlaceholderExec)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (MenuButton != null)
			{
				RecreatePlaceholdersMenu();

				MenuButton.ContextMenuOpening += new ContextMenuEventHandler(MenuButtonContextMenuOpening);
				MenuButton.ContextMenu.Placement = PlacementMode.Bottom;
				MenuButton.ContextMenu.PlacementTarget = MenuButton;
			}
			if (PathEditor != null)
			{
				//Make it mimic the editable area of a combo box
				PathEditor.Margin = new Thickness(0, 0, SystemParameters.VerticalScrollBarWidth, 0);
				PathEditor.KeyDown += new KeyEventHandler(OnPathEditorKeyDown);
				if (PathEditorHistory != null)
				{
					PathEditorHistory.SelectionChanged += new SelectionChangedEventHandler(OnPathEditorHistorySelectionChanged);
					PathEditorHistory.DropDownClosed += new EventHandler(OnPathEditorHistoryDropDownClosed);
				}
			}
		}

		private void CanBrowse(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = BrowseDialog != null;
		}
		private void BrowseExec(object sender, ExecutedRoutedEventArgs e)
		{
			BrowseDialog.FileName = PathPattern;
			if (BrowseDialog.ShowDialog().GetValueOrDefault(false))
			{
				PathPattern = BrowseDialog.FileName;
			}
			if(PathEditor != null)
				PathEditor.Focus();
		}

		private void InsertPlaceholderExec(object sender, ExecutedRoutedEventArgs e)
		{
			if (PathEditor == null)
			{
				PathPattern += (string)e.Parameter; //Fallback position, just append the placeholder to the path text
			}
			else
			{
				//If there is a path editor text box, put the placeholder at the insertion point
				PathEditor.SelectedText = (string)e.Parameter;
				PathEditor.Select(PathEditor.SelectionStart + PathEditor.SelectionLength, 0);
				PathEditor.Focus();
			}
		}

		private void MenuButtonContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			//Cancel context menu opening
			e.Handled = true;
		}

		#region Combo box mimicry for PathEditor
		private void OnPathEditorKeyDown(object sender, KeyEventArgs e)
		{
			//Mimic combo box behaviour
			Key systemKey = e.Key;
			if (systemKey == Key.System)
			{
				systemKey = e.SystemKey;
			}
			switch (systemKey)
			{
				case Key.F4:
					if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
					{
						PathEditorHistory.IsDropDownOpen = !PathEditorHistory.IsDropDownOpen;
						e.Handled = true;
					}
					return;

				case Key.Return:
				case Key.Escape:
					if (PathEditorHistory.IsDropDownOpen)
					{
						PathEditorHistory.IsDropDownOpen = false;
						e.Handled = true;
					}
					return;

				case Key.Up:
				case Key.Down:
					if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
					{
						PathEditorHistory.IsDropDownOpen = !PathEditorHistory.IsDropDownOpen;
					}
					e.Handled = true;
					return;
			}
		}

		private void OnPathEditorHistorySelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PathEditor.Text = (string)PathEditorHistory.SelectedValue;
		}
		private void OnPathEditorHistoryGotFocus(object sender, RoutedEventArgs e)
		{
			PathEditor.Focus();
		}
		private void OnPathEditorHistoryDropDownClosed(object sender, EventArgs e)
		{
			PathEditor.Focus();
		}
		#endregion

		private FileDialog mBrowseDialog;
		public FileDialog BrowseDialog
		{
			get 
			{
				return mBrowseDialog; 
			}
			set 
			{
				mBrowseDialog = value;
			}
		}

		public static readonly DependencyProperty PathPatternProperty = DependencyProperty.Register("PathPattern", typeof(string), typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public string PathPattern
		{
			get { return (string)GetValue(PathPatternProperty); }
			set { SetValue(PathPatternProperty, value); }
		}

		private ObservableCollection<PatternPlaceholder> mAdditionalPlaceholders = new ObservableCollection<PatternPlaceholder>();
		public ObservableCollection<PatternPlaceholder> AdditionalPlaceholders
		{
			get { return mAdditionalPlaceholders; }
		}

		private void OnAdditionalPlaceholdersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Template != null) //If the template has not been assigned yet, then don't bother updating, it will be updated when the template is applied anyway
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					//Just add the item
					if (AdditionalPlaceholders.Count == 1)
					{
						//This is the first item, so add a separator
						PlaceholdersMenu.Add(new Separator());
					}
					foreach (PatternPlaceholder placeholder in e.NewItems)
					{
						PlaceholdersMenu.Add(CreatePlaceholderMenuItem(placeholder));
					}
				}
				else
				{
					RecreatePlaceholdersMenu();
				}
			}
		}

		private ObservableCollection<Control> mPlaceholdersMenu = new ObservableCollection<Control>();
		public ObservableCollection<Control> PlaceholdersMenu
		{
			get { return mPlaceholdersMenu; }
		}
		private void RecreatePlaceholdersMenu()
		{
			mPlaceholdersMenu.Clear();
			MenuItem browse = MenuButton.TryFindResource("PART_BrowseMenuItem") as MenuItem;
			if (browse != null && Commands.Browse.CanExecute(null, this))
			{
				PlaceholdersMenu.Add(browse);
				PlaceholdersMenu.Add(new Separator());
			}
			PlaceholdersMenu.Add(CreatePlaceholderMenuItem(new PatternPlaceholder("A_rtist", "The artist searched for", "%artist%")));
			PlaceholdersMenu.Add(CreatePlaceholderMenuItem(new PatternPlaceholder("A_lbum", "The album searched for", "%album%")));

			if (AdditionalPlaceholders.Count > 0)
			{
				PlaceholdersMenu.Add(new Separator());

				foreach (PatternPlaceholder placeholder in AdditionalPlaceholders)
				{
					PlaceholdersMenu.Add(CreatePlaceholderMenuItem(placeholder));
				}
			}
		}

		private MenuItem CreatePlaceholderMenuItem(PatternPlaceholder placeholder)
		{
			MenuItem menuItem = new MenuItem();
			menuItem.Header = placeholder.MenuItemHeader;
			menuItem.ToolTip = placeholder.ToolTip;
			menuItem.CommandParameter = placeholder.Placeholder;
			menuItem.Command = Commands.InsertPlaceholder;
			return menuItem;
		}

		private ObservableCollection<String> mHistory = new ObservableCollection<string>();
		public ObservableCollection<String> History
		{
			get { return mHistory; }
		}

		/// <summary>
		/// Adds the current <see cref="PathPattern"/> to the <see cref="History"/>, or brings it
		/// to the top of the history list if it is already there.
		/// </summary>
		public void AddPatternToHistory()
		{
			//Make sure the changing of the history doesn't affect the text
			string pathPattern = PathPattern;
			if (!String.IsNullOrEmpty(pathPattern)) //Don't add a blank entry
			{
				int index = History.IndexOf(pathPattern);
				if (index == -1)
				{
					//Not yet in history, so add it.
					History.Insert(0, pathPattern);
					//If the history list is too long, trim it
					if (History.Count > sMaxHistoryLength)
					{
						History.RemoveAt(History.Count - 1);
					}
				}
				else if(index != 0)
				{
					//Already in the history, so bring it to the top
					History.Move(index, 0);
				}
			}
		}

		#region Elements
		private ToggleButton mCachedMenuButton;
		protected ToggleButton MenuButton
		{
			get
			{
				if (mCachedMenuButton == null)
				{
					if (Template != null)
					{
						mCachedMenuButton = Template.FindName("PART_MenuButton", this) as ToggleButton;
					}
				}

				return mCachedMenuButton;
			}
		}
		private ComboBox mCachedPathEditorHistory;
		protected ComboBox PathEditorHistory
		{
			get
			{
				if (mCachedPathEditorHistory == null)
				{
					if (Template != null)
					{
						mCachedPathEditorHistory = Template.FindName("PART_PathEditorHistory", this) as ComboBox;
					}
				}

				return mCachedPathEditorHistory;
			}
		}

		private TextBox mCachedPathEditor;
		protected TextBox PathEditor
		{
			get
			{
				if (mCachedPathEditor == null)
				{
					if (Template != null)
					{
						mCachedPathEditor = Template.FindName("PART_PathEditor", this) as TextBox;
					}
				}

				return mCachedPathEditor;
			}
		}
		#endregion
	}
}
