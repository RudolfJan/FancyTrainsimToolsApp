using Assets.Library.Models;
using System.Collections.Generic;

namespace FancyTrainsimTools.Desktop.Models
  {
  public class GameAssetsModel
    {
    public List<ProviderProductModel> ProviderProductList { get; set; }
    public List<ProviderProductModel> FilteredProviderProductList { get; set; }
    public ProviderProductFilterModel ProviderProductFilter { get; set; }

    public List<FlatAssetModel> BluePrintList { get; set; }
    public List<FlatAssetModel> FilteredBluePrintList { get; set; }
    public BluePrintFilterModel BluePrintFilter { get; set; }

    }
  }
