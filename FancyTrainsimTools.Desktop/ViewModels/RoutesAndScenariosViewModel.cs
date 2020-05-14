using FancyTrainsimTools.Desktop.Models;
using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimTools.Desktop.Views;
using System.Linq;
using System.Threading.Tasks;


namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class RoutesAndScenariosViewModel: Screen
    {
    private IWindowManager _windowManager;
    public RoutesUIModel Routes { get; set; }
    public ScenariosUIModel Scenarios { get; set; }

    private RouteModel _selectedRoute;

    public RouteModel SelectedRoute
      {
      get { return _selectedRoute; }
      set
        {
        if (_selectedRoute != value)
          {
          _selectedRoute = value;
          GetScenarios(_selectedRoute);
          NotifyOfPropertyChange(()=>SelectedRoute);
          NotifyOfPropertyChange(()=>Scenarios);
          }
        }
      }

    public bool CanSelectRoute
      {
      get
        {
        return true;
        }

      }

  
    public RoutesAndScenariosViewModel(IWindowManager windowManager)
      {
      _windowManager = windowManager;
      Routes= new RoutesUIModel();
      Scenarios= new ScenariosUIModel();

      }
    protected override void OnViewLoaded(object view)
      {
      base.OnViewLoaded(view);
      RoutesCollectionDataAccess.UpdateRouteTableForAllRoutes(Settings.GameRoutesFolder, true,
        false);
      RoutesCollectionDataAccess.UpdateRouteTableForAllRoutes(Settings.ArchiveRoutesFolder, false,
        true);
      Routes.RouteList = RoutesCollectionDataAccess.ReadAllRoutesFromDatabase();
      Routes.FilteredRouteList =
        RoutesCollectionDataAccess.ApplyAssetsFilter(Routes.RouteList, Routes.RouteFilter).OrderBy(x=>x.RouteName).ToList();
      }


    private void FilterRoutes()
      {
      Routes.FilteredRouteList =
        RoutesCollectionDataAccess.ApplyAssetsFilter(Routes.RouteList, Routes.RouteFilter).OrderBy(x=>x.RouteName).ToList();
      NotifyOfPropertyChange(()=>Routes);
      }

    private void FilterScenarios()
      {
      Scenarios.FilteredScenarioList = ScenarioCollectionDataAccess
        .ApplyAssetsFilter(Scenarios.ScenarioList,Scenarios.ScenarioFilter)
        .OrderBy(x=>x.ScenarioTitle).ToList();
      NotifyOfPropertyChange(()=>Scenarios);
      }

    public async Task GetRouteAssets(RouteModel route)
      {
      var routeAssetsVM = IoC.Get<RouteAssetsView>();
      routeAssetsVM.Route = route;
      await _windowManager.ShowWindowAsync(routeAssetsVM);
      }


    private void GetScenarios(RouteModel RouteModel)
      {
      // Two step approach. In the first step we only do this for the packed scenarios, then we retrieve the unpacked scenarios, combine all in the database
      // Finally, we read the database and apply filtering and sorting.

      if (RouteModel.IsPacked)
        {
        Scenarios.ScenarioList = ScenarioCollectionDataAccess.ReadPackedListScenariosFromDisk(
          Settings.GameRoutesFolder,
          SelectedRoute.RouteGuid, true, false);
        ScenarioCollectionDataAccess.SaveScenariosBulkList(Scenarios.ScenarioList, true, false);
        }
      Scenarios.ScenarioList = ScenarioCollectionDataAccess
          .ReadScenariosFromDisk(Settings.GameRoutesFolder,
            SelectedRoute.RouteGuid, true, false);
        ScenarioCollectionDataAccess.SaveScenariosBulkList(Scenarios.ScenarioList, true, false);
        Scenarios.ScenarioList = ScenarioCollectionDataAccess
                  .ReadScenariosFromDatabase(SelectedRoute.RouteGuid);
        Scenarios.FilteredScenarioList = ScenarioCollectionDataAccess
                  .ApplyAssetsFilter(Scenarios.ScenarioList,Scenarios.ScenarioFilter)
                  .OrderBy(x=>x.ScenarioTitle).ToList();
        }
      }
    }

