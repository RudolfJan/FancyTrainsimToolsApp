using Assets.Library.Helpers;
using Assets.Library.Models;
using Dapper;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Assets.Library.Logic
  {
  public class ScenarioCollectionDataAccess
    {
    public static List<ScenarioModel> ReadPackedListScenariosFromDisk(string assetsDirectory,
      string routeGuid,
      bool inGame, bool inArchive)
      {
      var scenarioList = new List<ScenarioModel>();
      int routeId = RoutesCollectionDataAccess.GetRouteId(routeGuid);
      if (routeId <= 0)
        {
        Log.Trace($"Cannot find RouteId for {routeGuid} No scenarios retrieved {Converters.LocationToString(inGame,inArchive)}", LogEventType.Message);
        return scenarioList;
        }
      DirectoryInfo path = new DirectoryInfo($"{assetsDirectory}{routeGuid}\\");
      if (Directory.Exists(path.FullName))
        {

        FileInfo[] apFiles = path.GetFiles("*.ap", SearchOption.TopDirectoryOnly);
        foreach (var file in apFiles)
          {

          // We cannot use the ZipAccess method here, because all logic but be in scope of the using statement for the archive
          using (ZipArchive archive = ZipFile.OpenRead(file.FullName))
            {
            var entries = archive.Entries;
            var filteredEntries = entries.Where(x => x.FullName.EndsWith("ScenarioProperties.xml")).ToList();

            foreach (var entry in filteredEntries)
              {
              var scenario = new ScenarioModel();
              scenario.IsPacked = true;
              scenario.Pack = file.Name;
              scenario.RouteId = routeId;
              if (inGame)
                {
                scenario.IsValidInGame = true;
                }

              if (inArchive)
                {
                scenario.IsValidInArchive = true;
                }

              scenario.ScenarioGuid = entry.FullName.Substring(10, 36);
              var scenarioProperties =
                ScenarioPropertiesDataAccess.ReadPackedScenarioNameAndClass(entry,
                  scenario.ScenarioGuid);
              scenario.ScenarioTitle = scenarioProperties.ScenarioTitle;
              scenario.ScenarioClass = scenarioProperties.ScenarioClass;
              scenarioList.Add(scenario);
              }
            }
          }
        }

      return scenarioList;
      }

    public static List<ScenarioModel> ReadScenariosFromDisk(string assetsDirectory, string routeGuid,
      bool inGame, bool inArchive)
      {
      var scenarioList = new List<ScenarioModel>();
      int routeId = RoutesCollectionDataAccess.GetRouteId(routeGuid);
      if (routeId <= 0)
        {
        Log.Trace($"Cannot find RouteId for {routeGuid} No scenarios retrieved {Converters.LocationToString(inGame,inArchive)}", LogEventType.Message);
        return scenarioList;
        }
      DirectoryInfo path = new DirectoryInfo($"{assetsDirectory}{routeGuid}\\Scenarios\\");
      if (Directory.Exists(path.FullName))
        {
        DirectoryInfo[] scenarioPathList = path.GetDirectories("*", SearchOption.TopDirectoryOnly);
        foreach (var scenarioDir in scenarioPathList)
          {
          var scenario = new ScenarioModel();
          scenario.ScenarioGuid = scenarioDir.Name;
          scenario.RouteId = routeId;
          var scenarioPropertiesPath = $"{scenarioDir}\\ScenarioProperties.xml";

          if (File.Exists(scenarioPropertiesPath))
            {
            if (inGame)
              {
              scenario.IsValidInGame = true;
              }

            if (inArchive)
              {
              scenario.IsValidInArchive = true;
              }

            var properties =
              ScenarioPropertiesDataAccess.ReadScenarioNameAndClass(scenarioPropertiesPath,
                scenario.ScenarioGuid);
            scenario.ScenarioTitle = properties.ScenarioTitle;
            scenario.ScenarioClass = properties.ScenarioClass;
            }
          else
            {
            scenario.ScenarioTitle =
              ScenarioPropertiesDataAccess.GetInvalidScenarioTitle(scenario.ScenarioGuid);
            if (inGame)
              {
              scenario.IsValidInGame = false;
              }
            if (inArchive)
              {
              scenario.IsValidInArchive = false;
              }
            }
          scenarioList.Add(scenario);
          }
        }

      return scenarioList;
      }

    public static void SaveScenariosBulkList(List<ScenarioModel> scenarioList, Boolean inGame, Boolean inArchive)
      {
      string fieldName=Converters.LocationToString(inGame,inArchive);
      using (IDbConnection connection =
        new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement =
            @$"INSERT OR IGNORE INTO Scenarios (ScenarioGuid, ScenarioTitle, ScenarioClass, RouteId, Pack, IsPacked, IsValid{fieldName}) VALUES (@ScenarioGuid, @ScenarioTitle, @ScenarioClass, @RouteId, @Pack, @IsPacked, @IsValid{fieldName})";
          try
            {
            foreach (var item in scenarioList)
              {
              connection.Execute(sqlStatement, new { item.ScenarioGuid, item.ScenarioTitle, item.ScenarioClass, item.RouteId, item.Pack, item.IsPacked, item.IsValidInGame, item.IsValidInArchive},
                transaction);
              }
            transaction.Commit();
            GetScenarioIds(scenarioList);
            UpdateBulkStatus(scenarioList,fieldName);
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during bulk save Routes to database", ex,
              LogEventType.Error);
            }
          }
        }
      }

    private static void GetScenarioIds(List<ScenarioModel> scenarioList)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"SELECT Id FROM Scenarios WHERE ScenarioGuid= @ScenarioGuid;";
          try
            {
            foreach (var item in scenarioList)
              {
              IdModel idModel=connection.Query<IdModel>(sqlStatement, new { item.ScenarioGuid}, transaction).First();
              item.Id = idModel.Id;
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during getting id's for Scenarios from database",ex, LogEventType.Error);
            }
          }
        }
      }

    private static void UpdateBulkStatus(List<ScenarioModel> scenarioList,string fieldName)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"UPDATE OR IGNORE Routes SET {fieldName}=1 WHERE Id=@Id";
          try
            {
            foreach (var item in scenarioList)
              {
              if (item.Id <= 0)
                {
                Log.Trace($"Internal error. Id for Routes not set in {item}", LogEventType.Error);
                throw new InvalidDataException($"Internal error. Id for Routes not set in {item}");
                }
              connection.Execute(sqlStatement, new { item.Id}, transaction);
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during bulk update InGame/InArchive field Scenarios to database",ex, LogEventType.Error);
            }
          }
        }
      }

    public static List<ScenarioModel> ReadScenariosFromDatabase(string routeGuid)
      {
      var routeId = RoutesCollectionDataAccess.GetRouteId(routeGuid);
      using IDbConnection Db = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString());
      var output = Db.Query<ScenarioModel>("SELECT * FROM Scenarios WHERE RouteId=@routeId", new {routeId });
      return output.ToList();
      }

    #region filters

    public static List<ScenarioModel> ApplyAssetsFilter(List<ScenarioModel> scenarioList, ScenarioFilterModel filter)
      {
      return scenarioList.Where(p => AssetsFilter(p, filter)).ToList();
      }

    private static bool AssetsFilter(ScenarioModel scenario, ScenarioFilterModel filter)
      {
      // The filters always are set to true, so we select the item if nothing is set, once a filter has a non-default value, it must cause a match, else it fails.
      if (scenario == null || filter == null)
        {
        return false;
        }

      var output = Converters.EvaluateLocationFilter(filter.IsPackedFilter, filter.IsNotValidFilter,
        scenario.IsPacked, scenario.IsNotValid);

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.ScenarioTitleFilter, scenario.ScenarioTitle);
        }
      else
        {
        return false;
        }

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.PackFilter, scenario.Pack);
        }
      else
        {
        return false;
        }

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.ScenarioClassFilter, scenario.ScenarioClass);
        }
      else
        {
        return false;
        }
  
      return output;
      }




    #endregion


    }
  }
