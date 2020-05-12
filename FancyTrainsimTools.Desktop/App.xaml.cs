using Assets.Library.Logic;
using Assets.Library.Models;
using FancyTrainsimTools.Desktop.ViewModels;
using FancyTrainsimTools.Desktop.Views;
using Logging.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.Library;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop
  {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
    {
    public static ServiceProvider serviceProvider; // for support of non-modal windows
    public static ILogCollectionManager logger;

    public IServiceProvider GetServiceProvider()
      {
      return serviceProvider;
      }
    public static ILogCollectionManager GetLogger()
      {
      return logger;
      }
    private void CreateViewModelServices(ServiceCollection services)
      {
      //services.AddTransient<LogCollectionManager>(p=> new LogCollectionManager());
      // pass in parameter
      // https://cmatskas.com/net-core-dependency-injection-with-constructor-parameters-2/

      services.AddTransient<IShellViewModel,ShellViewModel>();
      services.AddTransient(p => new ShellView());
      services.AddTransient<AboutViewModel>();
      services.AddTransient(p => new AboutView());
      services.AddTransient<LoggingViewModel>();
      services.AddTransient(p => new LoggingView());
      services.AddTransient<ManualsViewModel>();
      services.AddTransient(p => new ManualsView());
      services.AddTransient<GameAssetsViewModel>();
      services.AddTransient(p => new GameAssetsView());
      services.AddTransient<RoutesAndScenariosViewModel>();
      services.AddTransient(p => new RoutesAndScenariosView());
      services.AddTransient<RouteAssetsViewModel>();
      services.AddTransient(p => new RouteAssetsView());
      }   
 
    private void CreateLogicServices(ServiceCollection services)
      {
      //services.AddTransient(p => new AssetCollectionDataAccess(Settings.GameAssetFolder));
      services.AddTransient<ProviderProductFilterModel>();
      services.AddTransient<BluePrintFilterModel>();
      }

    protected override void OnStartup(StartupEventArgs e)
      {
      base.OnStartup(e); 
      // services
      var services = new ServiceCollection();
      CreateViewModelServices(services);
      CreateLogicServices(services);
      LogEventHandler.LogEvent += OnLogEvent;

      // create service provider
      serviceProvider = services.BuildServiceProvider();

      // Todo init database, logging etcetera
      //CreateLogger();
      // start logging registration
      //logger = serviceProvider.GetService<LogCollectionManager>();
      // Initialize the database here.
      AssetDatabaseAccess.InitDatabase(Settings.ConnectionString,Settings.AssetDatabasePath);
      var window=serviceProvider.GetService<ShellView>();
      window.Show();
      }

    private void OnLogEvent(object sender, LogEventArgs args)
      {
      if (args.EntryClass.EventType == LogEventType.Error)
        {
        var message = args.EntryClass.LogEntry;
        var form = new NotificationView(message);
        form.Show();
        }
      }
    }
  }
