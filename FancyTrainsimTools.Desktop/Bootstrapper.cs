using Assets.Library.Logic;
using Caliburn.Micro;
using FancyTrainsimTools.Desktop.ViewModels;
using FancyTrainsimTools.Desktop.Views;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FancyTrainsimTools.Desktop
	{
	public class Bootstrapper: BootstrapperBase
		{
		private readonly SimpleContainer _container = new SimpleContainer();

		public Bootstrapper()
			{
			Initialize();
			}

		protected override void Configure()
			{
			_container
				.Singleton<IWindowManager, WindowManager>()
				.Singleton<IEventAggregator, EventAggregator>();

			GetType().Assembly.GetTypes()
				.Where(type => type.IsClass)
				.Where(type => type.Name.EndsWith("ViewModel"))
				.ToList()
				.ForEach(viewModelType => _container.RegisterPerRequest(
					viewModelType, viewModelType.ToString(), viewModelType));
			}

		protected override void OnStartup(object sender, StartupEventArgs e)
			{
			LogEventHandler.LogEvent += OnLogEvent;
			Settings.ReadFromRegistry();
			AssetDatabaseAccess.InitDatabase(Settings.ConnectionString, Settings.AssetDatabasePath);
			DisplayRootViewFor<ShellViewModel>();
			}

		private void OnLogEvent(object Sender, LogEventArgs args)
			{
			if (args.EntryClass.EventType == LogEventType.Error || args.EntryClass.EventType == LogEventType.Event)
				{
				LogCollectionManager.LogEvents.Add(args.EntryClass);
				var message = args.EntryClass.LogEntry;
				var form = new NotificationView(message);
				form.Show();
				}
			}

		protected override object GetInstance(Type service, string key)
			{
			return _container.GetInstance(service, key);
			}

		protected override IEnumerable<object> GetAllInstances(Type service)
			{
			return _container.GetAllInstances(service);
			}

		protected override void BuildUp(object instance)
			{
			_container.BuildUp(instance);
			}
		}
	}
