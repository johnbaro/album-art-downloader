using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Configuration;
using System.ComponentModel;

namespace AlbumArtDownloader
{
	/// <summary>
	/// A weak event manager for the ApplicationSettingsBase.PropertyChanged event
	/// </summary>
	public class PropertyChangedEventManager : WeakEventManager
	{
		public static void AddListener(ApplicationSettingsBase source, IWeakEventListener listener)
		{
			PropertyChangedEventManager.CurrentManager.ProtectedAddListener(source, listener);
		}

		public static void RemoveListener(ApplicationSettingsBase source, IWeakEventListener listener)
		{
			PropertyChangedEventManager.CurrentManager.ProtectedRemoveListener(source, listener);
		}

		protected override void StartListening(Object source)
		{
			ApplicationSettingsBase applicationSettingsBase = (ApplicationSettingsBase)source;
			applicationSettingsBase.PropertyChanged += OnPropertyChanged;
		}

		protected override void StopListening(Object source)
		{
			ApplicationSettingsBase applicationSettingsBase = (ApplicationSettingsBase)source;
			applicationSettingsBase.PropertyChanged -= OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.DeliverEvent(sender, e);
		}

		private static PropertyChangedEventManager CurrentManager
		{
			get
			{
				Type managerType = typeof(PropertyChangedEventManager);
				PropertyChangedEventManager manager = (PropertyChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if (manager == null)
				{
					manager = new PropertyChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, manager);
				}
				return manager;
			}
		}
	}
}
