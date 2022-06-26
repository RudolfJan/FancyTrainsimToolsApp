using Caliburn.Micro;
using Logging.Library;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
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


    public async Task ChangeSettings()
      {
      SettingsViewModel settingsVM = IoC.Get<SettingsViewModel>();
      await _windowManager.ShowDialogAsync(settingsVM);
      }


    public async Task DatabaseManagement()
      {
      var databaseManagementVM = IoC.Get<DatabaseManagementViewModel>();
      await _windowManager.ShowDialogAsync(databaseManagementVM);
      }

    public async Task InstallerTool()
      {
      var installerVM = IoC.Get<InstallerViewModel>();
      await _windowManager.ShowWindowAsync(installerVM);
      }

    public async Task AddOnCatalog()
      {
      var addOnCatalogVM = IoC.Get<AddOnCatalogViewModel>();
      await _windowManager.ShowWindowAsync(addOnCatalogVM);
      }

    public async Task ShowLogging()
      {
      Log.Trace("Called loggingView");
      var loggingVM = IoC.Get<LoggingViewModel>();
      await _windowManager.ShowWindowAsync(loggingVM);
      }

    public async Task ShowRoutes()
      {
      var routesVM = IoC.Get<RoutesAndScenariosViewModel>();
      await _windowManager.ShowWindowAsync(routesVM);
      }

    public async Task Launcher()
      {
      var launcherVM = IoC.Get<LauncherViewModel>();
      await _windowManager.ShowWindowAsync(launcherVM);
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
    }
  }
