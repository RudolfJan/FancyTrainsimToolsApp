using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
  {
  public class RouteFilterModel
    {    
    public bool InGameFilter { get; set; } = false;
    public bool InArchiveFilter { get; set; } = false;
    public string RouteNameFilter { get; set; } = string.Empty;
    public string RoutePackFilter { get; set; } = string.Empty;
    }
  }
