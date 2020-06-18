using Assets.Library.Models;
using Caliburn.Micro;
using System.Collections.Generic;

namespace FancyTrainsimToolsDesktop.Models
  {
  public class GameAssetsModel
    {
    public List<ProviderProductModel> ProviderProductList { get; set; }
    public BindableCollection<ProviderProductModel> FilteredProviderProductList { get; set; }
    public ProviderProductFilterModel ProviderProductFilter { get; set; }

    public List<FlatAssetModel> BluePrintList { get; set; }
    public BindableCollection<FlatAssetModel> FilteredBluePrintList { get; set; }
    public BluePrintFilterModel BluePrintFilter { get; set; }

    }
  }
