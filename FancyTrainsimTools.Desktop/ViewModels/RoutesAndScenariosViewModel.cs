using FancyTrainsimTools.Desktop.Models;
using Assets.Library.Logic;
using Mvvm.Library;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class RoutesAndScenariosViewModel: BindableBase
    {
    public RoutesModel Routes { get; set; }


    public RoutesAndScenariosViewModel()
      {
      Routes= new RoutesModel();
      RoutesCollectionDataAccess.UpdateRouteTableForAllRoutes(Settings.GameRoutesFolder, true,
        false);
      RoutesCollectionDataAccess.UpdateRouteTableForAllRoutes(Settings.ArchiveRoutesFolder, false,
        true);
      Routes.RouteList = RoutesCollectionDataAccess.ReadAllRoutesFromDatabase();
      }
    }
  }
