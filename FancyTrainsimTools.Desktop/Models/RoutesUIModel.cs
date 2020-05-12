using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FancyTrainsimTools.Desktop.Models
  {
  public class RoutesUIModel
    {
    public List<RouteModel> RouteList { get; set; }
    public List<RouteModel> FilteredRouteList { get; set; }
    public RouteFilterModel RouteFilter { get; set; } = new RouteFilterModel();
    }
  }
