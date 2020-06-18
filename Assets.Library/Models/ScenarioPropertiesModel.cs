using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Assets.Library.Models
  {
  public class ScenarioPropertiesModel
    {
    public string ScenarioTitle { get; set; }
    public string ScenarioGuid { get; set; }

    public int StartTime { get; set; } // start time of the scenario, located in scenarioproperties
    public string StartTimeString { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Briefing { get; set; } = String.Empty;
    public string Author { get; set; } = String.Empty;
    public string Duration { get; set; } = String.Empty;
    public string Season { get; set; } = String.Empty;
    public string ServiceClass { get; set; } = String.Empty;
    public string ScenarioClass { get; set; } = String.Empty;
    public string Rating { get; set; } = String.Empty;
    public string PlayerEngine { get; set; } = String.Empty;
    public string ServiceName { get; set; } = String.Empty;
    public List<ConsistModel> ConsistList { get; set; }
    public List<FullRailVehicleModel> RequiredRailVehicles { get; set; }
    public XDocument PropertiesDoc { get; set; }
    public XDocument BinDoc { get; set; }


    /*
    XML structure for ScenarioNetworkProperties.xml This file contains if the markers that are defined in the scenario
    */
    public XDocument NetworkDoc { get; set; }
  

    /*
    True if scenario has been edited and changes may be saved
    */
    public bool HasChanged { get; set; }
    }
  }
  