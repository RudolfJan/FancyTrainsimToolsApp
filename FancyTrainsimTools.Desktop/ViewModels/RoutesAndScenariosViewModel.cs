using FancyTrainsimTools.Desktop.Models;
using Assets.Library.Logic;
using Assets.Library.Models;
using FancyTrainsimTools.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class RoutesAndScenariosViewModel: BindableBase
    {
    private readonly IServiceProvider _serviceProvider;
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
          OnPropertyChanged("SelectedRoute");
          OnPropertyChanged("Scenarios");
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

    public ICommand FilterRoutesCommand { get; }
    public ICommand FilterScenariosCommand { get; }
    public ICommand RouteAssetsCommand { get; }

    public RoutesAndScenariosViewModel()
      {
      _serviceProvider = App.serviceProvider;
      Routes= new RoutesUIModel();
      Scenarios= new ScenariosUIModel();

      FilterRoutesCommand= new RelayCommand(FilterRoutes);
      FilterScenariosCommand= new RelayCommand(FilterScenarios);
      RouteAssetsCommand= new RelayCommand<RouteModel>(GetRouteAssets);

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
      OnPropertyChanged("Routes");
      }

    private void FilterScenarios()
      {
      Scenarios.FilteredScenarioList = ScenarioCollectionDataAccess
        .ApplyAssetsFilter(Scenarios.ScenarioList,Scenarios.ScenarioFilter)
        .OrderBy(x=>x.ScenarioTitle).ToList();
      OnPropertyChanged("Scenarios");
      }

    private void GetRouteAssets(RouteModel route)
      {
      var routeAssetsView = _serviceProvider.GetService<RouteAssetsView>();
      routeAssetsView.Route = route;
      routeAssetsView.Show();
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

