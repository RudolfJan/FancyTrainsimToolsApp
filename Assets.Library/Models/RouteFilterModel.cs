using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
  {
  public class RouteFilterModel
    { 
    public bool IsPackedFilter { get; set; } = false;
    public bool InGameFilter { get; set; } = false;
    public bool InArchiveFilter { get; set; } = false;
    public bool IsValidInGameFilter { get; set; } = false;
    public bool IsValidInArchiveFilter { get; set; } = false;
    public string RouteNameFilter { get; set; } = string.Empty;
    public string RoutePackFilter { get; set; } = string.Empty;
    }
  }
