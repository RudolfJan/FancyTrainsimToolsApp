using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
  {
  public class BluePrintFilterModel
    {
    public bool InGameFilter { get; set; } = false;
    public bool InArchiveFilter { get; set; } = false;
    public string ProviderFilter { get; set; } = string.Empty;
    public string ProductFilter { get; set; } = string.Empty;
    public string BluePrintFilter { get; set; } = string.Empty;
    public string PackFilter { get; set; } = string.Empty;
    }
  }
