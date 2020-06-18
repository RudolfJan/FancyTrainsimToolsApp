using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
  {
  public class ScenarioModel
    {
    public int Id { get; set; }
    public string ScenarioTitle { get; set; } = string.Empty;
    public string ScenarioGuid { get; set; }= string.Empty;
    public string ScenarioClass { get; set; } = string.Empty;
    public int RouteId { get; set; }
    public string Pack { get; set; } = string.Empty;
    public bool IsPacked { get; set; }
    public bool InGame { get; set; }
    public bool IsValidInGame { get; set; }
    public bool IsNotValid {
      get { return !IsValidInGame; }}
    public bool InArchive { get; set; }
    public bool IsValidInArchive { get; set; }
    public ScenarioPropertiesModel ScenarioProperties { get; set; }
    }
  }
