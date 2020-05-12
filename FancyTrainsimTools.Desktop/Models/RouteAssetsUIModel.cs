using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FancyTrainsimTools.Desktop.Models
  {
  public class RouteAssetsUIModel
    {
    public RouteModel Route { get; set; } = new RouteModel();
    public List<ProviderProductModel> ProviderProductList { get; set; } = new List<ProviderProductModel>();
    public List<ProviderProductModel> FilteredProviderProductList { get; set; } = new List<ProviderProductModel>();
    public ProviderProductFilterModel ProviderProductFilter { get; set; } = new ProviderProductFilterModel();
    public List<RouteAssetsModel> RouteAssetList { get; set; } = new List<RouteAssetsModel>();
    public List<RouteAssetsModel> FilteredRouteAssetList { get; set; } = new List<RouteAssetsModel>();
    public RouteAssetFilterModel RouteAssetFilter { get; set; } = new RouteAssetFilterModel();
    }
  }
