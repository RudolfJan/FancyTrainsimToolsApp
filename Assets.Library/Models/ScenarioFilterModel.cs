using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
  {
  public class ScenarioFilterModel
    {
    public string ScenarioTitleFilter { get; set; } = string.Empty;
    public string ScenarioClassFilter { get; set; } = string.Empty;
    public string PackFilter { get; set; } = string.Empty;
    public bool IsPackedFilter { get; set; } = false;
    public bool IsNotValidFilter { get; set; } = false;
    public bool InGameFilter { get; set; } = false;
    public bool InArchiveFilter { get; set; } = false;
    }
  }
