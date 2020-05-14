using Caliburn.Micro;
using FancyTrainsimTools.Desktop.Views;
using Logging.Library;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class ShellViewModel : Conductor<object>
    {
    private readonly IWindowManager _windowManager;

    public ShellViewModel(IWindowManager windowManager)
      {
      _windowManager = windowManager;
      }

    public async Task Exit()
      {
      await TryCloseAsync();
      }


    public async Task ShowRoutes()
      {
      var routesVM = IoC.Get<RoutesAndScenariosViewModel>();
      await _windowManager.ShowWindowAsync(routesVM);
      }

    public async Task ShowGameAssets()
      {
      Log.Trace("Called GameAssetsView");
      var gameAssetsVM = IoC.Get<GameAssetsViewModel>();
      await _windowManager.ShowWindowAsync(gameAssetsVM);
      }

    public async Task ShowAbout()
      {
      Log.Trace("Called AboutView using a very long text here to see if wrapping actually works, so I will be happy");
      var aboutVM = IoC.Get<AboutViewModel>();
      await _windowManager.ShowDialogAsync(aboutVM);
      }

    public async Task ShowTrainSimManuals()
      {
      var manualVM = IoC.Get<ManualsViewModel>();
      await _windowManager.ShowWindowAsync(manualVM);
      }


    public async Task ShowLogging()
      {
      Log.Trace("Called loggingView");
      var loggingVM = IoC.Get<LoggingViewModel>();
      await _windowManager.ShowWindowAsync(loggingVM);
      }


    }
  }
