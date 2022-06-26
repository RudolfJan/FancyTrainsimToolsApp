using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
  {
  public class RouteAssetsViewModel : Screen
    {

    #region Properties
    private BindableCollection<FullRouteProviderProductsModel> filteredProviderProductList;
    private BindableCollection<FullRouteAssetsModel> filteredRouteAssetList;

    public RouteModel Route { get; set; } = new RouteModel();
    public List<FullRouteProviderProductsModel> ProviderProductList { get; set; } = new List<FullRouteProviderProductsModel>();
    public BindableCollection<FullRouteProviderProductsModel> FilteredProviderProductList
      {
      get
        {
        return filteredProviderProductList;
        }
      set
        {
        filteredProviderProductList = value;
        NotifyOfPropertyChange(() => FilteredProviderProductList);
        }
      }
    public ProviderProductFilterModel ProviderProductFilter { get; set; } = new ProviderProductFilterModel();
    public List<FullRouteAssetsModel> RouteAssetList { get; set; } = new List<FullRouteAssetsModel>();
    public BindableCollection<FullRouteAssetsModel> FilteredRouteAssetList
      {
      get
        {
        return filteredRouteAssetList;
        }
      set
        {
        filteredRouteAssetList = value;
        NotifyOfPropertyChange(()=>FilteredRouteAssetList);
        }
      }

    public RouteAssetFilterModel RouteAssetFilter { get; set; } = new RouteAssetFilterModel();

    readonly CancellationToken cancellation= new CancellationToken();

    #endregion
    #region Initialization

    protected override async void OnViewLoaded(object view)
      {
      base.OnViewLoaded(view);
      await LoadAllRouteAssets();
      }

    #endregion

    #region Methods

    public void SetProductFilter()
      {
      // Sets filter on provider/product part of route assets
      }

    public void SetAssetsFilter()
      {
      // Sets filter on assets
      }
 
    public async Task GetSceneryItems()
      {
      // Get all scenery items for the route with id RouteId
      string routePath=$"{Settings.TrainSimGamePath}Content\\Routes\\";
      RouteAssetsDataAccess.CreateRouteAssetsInDatabase(Route, routePath, Settings.TempFolder);
      var progress = new Progress<BasicProgressModel>();
      await LoadAllRouteAssets();
      }

    private async Task LoadAllRouteAssets()
      {
      ProviderProductList = await RouteAssetsDataAccess.GetProviderProductsForRoute(Route.Id);
      FilteredProviderProductList= new BindableCollection<FullRouteProviderProductsModel>(ProviderProductList);
      RouteAssetList = await RouteAssetsDataAccess.GetAssetsForRoute(Route.Id);
      FilteredRouteAssetList= new BindableCollection<FullRouteAssetsModel>(RouteAssetList);
      }

    public void CancelGetSceneryItems()
      {
      
      }

    public async Task Exit()
      {
      await TryCloseAsync();
      }

    #endregion
    }
  }
