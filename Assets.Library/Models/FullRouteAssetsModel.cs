using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Logic
  {
  public class FullRouteAssetsModel
    {
    public int RouteId { get; set; }
    public bool InGame { get; set; }
    public bool InArchive { get; set; }
    public string RouteName { get; set; }
    public string Provider { get; set; }
    public string Product { get; set; }
    public string Pack { get; set; }
    public string BluePrintPath { get; set; }
    }
  }
