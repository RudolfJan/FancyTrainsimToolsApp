using Assets.Library.Helpers;
using Assets.Library.Models;
using Logging.Library;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Assets.Library.Logic
  {
  public class ScenarioPropertiesDataAccess
    {

    public static void ReadScenarioProperties(string path, ScenarioPropertiesModel properties)
      {
      try
        {
        var propertiesDoc = XDocument.Load(path);
        BuildAllScenarioProperties(propertiesDoc, properties);
        }
      catch (Exception ex)
        {
        Log.Trace("Cannot process ScenarioProperties.xml ",ex, LogEventType.Error);
        properties.ScenarioTitle = $"ZZ_Invalid_{properties.ScenarioGuid}";
        }
      }

    public static XDocument ZipEntryToDoc(ZipArchiveEntry entry)
      {
      try
        {
        if (entry != null)
          {
          using var inStream = entry.Open();
          using var reader = new StreamReader(inStream);
          var doc = XDocument.Load(reader);
          return doc;
          }
        }
      catch(Exception ex)
        {
        Log.Trace($"Cannot open archive entry {entry?.FullName}", ex, LogEventType.Error);
        throw;
        }
      return null;
      }

    public static ScenarioPropertiesModel ReadPackedScenarioNameAndClass(ZipArchiveEntry entry, string scenarioGuid)
      {
      var properties= new ScenarioPropertiesModel();
      try
        {
        var propertiesDoc = ZipEntryToDoc(entry);
        GetScenarioNameAndClass(propertiesDoc, properties);
        }
      catch (Exception ex)
        {
        Log.Trace("Cannot process ScenarioProperties.xml ",ex, LogEventType.Error);
        properties.ScenarioTitle = $"ZZ_Invalid_{scenarioGuid}";
        }
      return properties;
      }


    public static ScenarioPropertiesModel ReadScenarioNameAndClass(string path, string scenarioGuid)
      {
      var properties= new ScenarioPropertiesModel();
      try
        {
        var propertiesDoc = XDocument.Load(path);
        GetScenarioNameAndClass(propertiesDoc, properties);
        }
      catch (Exception ex)
        {
        Log.Trace("Cannot process ScenarioProperties.xml ",ex, LogEventType.Error);
        properties.ScenarioTitle = $"ZZ_Invalid_{scenarioGuid}";
        }
      return properties;
      }



    private static String TranslateScenarioClass(String ScenarioClass)
      {
      switch (ScenarioClass)
        {
          case "eTimetableScenarioClass":
            return "Timetabled";
          case "eStandardScenarioClass":
            return "Standard";
          case "eCareerScenarioClass":
            return "Career";
          case "eFreeRoamScenarioClass":
            return "Free roam";
          case "eTutorialScenarioClass":
            return "Tutorial";
          case "eTemplateScenarioClass":
            return "Quick drive";
          case "eRelayScenarioClass":
            return "Relay (retired!)";
          default:
            return "?";
        }
      }

    public static String TranslateSeason(String Season)
      {
      switch (Season)
        {
          case "SEASON_SPRING":
            return "Spring";
          case "SEASON_SUMMER":
            return "Summer";
          case "SEASON_AUTUMN":
            return "Autumn";
          case "SEASON_WINTER":
            return "Winter";
          default:
            return "?";
        }
      }


    public static String TranslateSeasonReverse(String Season)
      {
      switch (Season)
        {
          case "Spring":
            return "SEASON_SPRING";
          case "Summer":
            return "SEASON_SUMMER";
          case "Autumn":
            return "SEASON_AUTUMN";
          case "Winter":
            return "SEASON_WINTER";
          default:
            return "?";
        }
      }

    public static void GetScenarioNameAndClass(XDocument Doc, ScenarioPropertiesModel properties)
      {
      try
        {
        var PropertiesNode = Doc.Element("cScenarioProperties");
        if (PropertiesNode != null)
          {
          var Node = PropertiesNode.XPathSelectElement("DisplayName/Localisation-cUserLocalisedString");
          properties.ScenarioTitle = Converters.GetLocalisedString(Node);
          properties.ScenarioClass = TranslateScenarioClass(PropertiesNode.Element("ScenarioClass")?.Value);
          }
        else
          {
          properties.ScenarioTitle=GetInvalidScenarioTitle(properties.ScenarioGuid);
          }
        }
      catch (Exception ex)
        { 
        Log.Trace("Cannot process ScenarioProperties.xml ", ex, LogEventType.Error);
        properties.ScenarioTitle=GetInvalidScenarioTitle(properties.ScenarioGuid);
        }
      }

    public static string GetInvalidScenarioTitle(string scenarioGuid)
      {
      return  $"ZZ_Invalid_{scenarioGuid}";
      }


    //XDocument version
    public static void BuildAllScenarioProperties(XDocument Doc, ScenarioPropertiesModel properties)
      {
      try
        {
        var PropertiesNode = Doc.Element("cScenarioProperties");
        if (PropertiesNode != null)
          {
          var Node =
            PropertiesNode.XPathSelectElement("DisplayName/Localisation-cUserLocalisedString");
          properties.ScenarioTitle = Converters.GetLocalisedString(Node);
          var DescriptionNodeList =
            PropertiesNode.XPathSelectElement("Description/Localisation-cUserLocalisedString");
          properties.Description = Converters.GetLocalisedString(DescriptionNodeList);
          var BriefingNodeList =
            PropertiesNode.XPathSelectElement("Briefing/Localisation-cUserLocalisedString");
          properties.Briefing = Converters.GetLocalisedString(BriefingNodeList);
          properties.ScenarioClass =
            TranslateScenarioClass(PropertiesNode.Element("ScenarioClass")?.Value);
          properties.Season = TranslateSeason(PropertiesNode.Element("Season")?.Value);
          properties.Author = PropertiesNode.Element("Author")?.Value;
          properties.Duration = PropertiesNode.Element("DurationMins")?.Value;
          properties.Rating = PropertiesNode.Element("Rating")?.Value;
          properties.StartTime = (int) Convert.ToDouble(PropertiesNode.Element("StartTime")?.Value);
          properties.StartTimeString = Converters.TimeToString(properties.StartTime);
          // BuildDriverDetails(PropertiesNode);
          // CareerRuleManager = new CCareerRuleManager(Doc);
          }
        else
          {
          properties.ScenarioTitle = GetInvalidScenarioTitle(properties.ScenarioGuid);
          }
        }
      catch (Exception ex)
        {
        Log.Trace("Cannot process ScenarioProperties.xml ", ex, LogEventType.Error);
        properties.ScenarioTitle = GetInvalidScenarioTitle(properties.ScenarioGuid);
        }
      }
    }
  }
