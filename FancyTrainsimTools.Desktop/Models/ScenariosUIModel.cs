using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FancyTrainsimTools.Desktop.Models
  {
  public class ScenariosUIModel
    {
    public List<ScenarioModel> ScenarioList { get; set; }
    public List<ScenarioModel> FilteredScenarioList { get; set; }
    public ScenarioFilterModel ScenarioFilter { get; set; } = new ScenarioFilterModel();
    }
  }
