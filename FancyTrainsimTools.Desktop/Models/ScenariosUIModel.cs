using Assets.Library.Models;
using System.Collections.Generic;

namespace FancyTrainsimToolsDesktop.Models
  {
  public class ScenariosUIModel
    {
    public List<ScenarioModel> ScenarioList { get; set; }
    public List<ScenarioModel> FilteredScenarioList { get; set; }
    public ScenarioFilterModel ScenarioFilter { get; set; } = new ScenarioFilterModel();
    }
  }
