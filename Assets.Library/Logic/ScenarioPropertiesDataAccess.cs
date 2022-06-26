using Assets.Library.Helpers;
using Assets.Library.Models;
using Logging.Library;
using System;
using System.IO;
using System.IO.Compression;
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
        properties.PropertiesDoc = propertiesDoc;
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
      return ScenarioClass switch
        {
        "eTimetableScenarioClass" => "Timetabled",
        "eStandardScenarioClass" => "Standard",
        "eCareerScenarioClass" => "Career",
        "eFreeRoamScenarioClass" => "Free roam",
        "eTutorialScenarioClass" => "Tutorial",
        "eTemplateScenarioClass" => "Quick drive",
        "eRelayScenarioClass" => "Relay (retired!)",
        _ => "?"
        };
      }

    public static String TranslateSeason(String Season)
      {
      return Season switch
        {
        "SEASON_SPRING" => "Spring",
        "SEASON_SUMMER" => "Summer",
        "SEASON_AUTUMN" => "Autumn",
        "SEASON_WINTER" => "Winter",
        _ => "?"
        };
      }


    public static String TranslateSeasonReverse(String Season)
      {
      return Season switch
        {
        "Spring" => "SEASON_SPRING",
        "Summer" => "SEASON_SUMMER",
        "Autumn" => "SEASON_AUTUMN",
        "Winter" => "SEASON_WINTER",
        _ => "?"
        };
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
          BuildDriverDetails(PropertiesNode, properties);
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

    static void BuildDriverDetails(XElement BaseNode, ScenarioPropertiesModel properties)
      {
      try
        {
        if (BaseNode == null)
          {
          return;
          }

        var DriverDetailsNodes = BaseNode.XPathSelectElements("FrontEndDriverList/sDriverFrontEndDetails");
        foreach (XElement Node in DriverDetailsNodes)
          {
          if (Node.Element("PlayerDriver")?.Value == "1")
            {
            var ServiceNameNodeList = Node.XPathSelectElement("ServiceName/Localisation-cUserLocalisedString");
            properties.ServiceName = Converters.GetLocalisedString(ServiceNameNodeList);

            var PlayerEngineNodeList = Node.XPathSelectElement("LocoName/Localisation-cUserLocalisedString");
            properties.PlayerEngine = Converters.GetLocalisedString(PlayerEngineNodeList);
            }
          }
        }

      catch (Exception E)
        {
        Log.Trace("Cannot process ScenarioProperties.xml " + E.Message, LogEventType.Error);
        }
      }

    public static void DeleteBackupFiles(string path)

      {
      try
        {
        var FileList = Directory.GetFiles(path, "*.bak?");
        foreach (string F in FileList)
          {
          File.Delete(F);
          }
        }
      catch (DirectoryNotFoundException DirNotFound)
        {
        Log.Trace(DirNotFound.Message, LogEventType.Error);
        }
      }
    public static void DeleteScriptFiles(string path)

      {
      try
        {
        var fileList = Directory.GetFiles(path, "*.lua?");
        foreach (string F in fileList)
          {
          File.Delete(F);
          }

        fileList = Directory.GetFiles(path, "*.out?");
        foreach (string F in fileList)
          {
          File.Delete(F);
          }

        fileList = Directory.GetFiles(path, "*.lua.MD5");
        foreach (string F in fileList)
          {
          File.Delete(F);
          }
        fileList = Directory.GetFiles(path, "*.luac.MD5");
        foreach (string F in fileList)
          {
          File.Delete(F);
          }
        fileList = Directory.GetFiles(path, "*.out.MD5");
        foreach (string F in fileList)
          {
          File.Delete(F);
          }
        }
      catch (DirectoryNotFoundException dirNotFound)
        {
        Log.Trace(dirNotFound.Message, LogEventType.Error);
        }
      }

    }
  }
