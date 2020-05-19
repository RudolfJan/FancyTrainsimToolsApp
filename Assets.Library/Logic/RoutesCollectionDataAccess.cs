#region UsingStatements

using Assets.Library.Helpers;
using Assets.Library.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Xml.XPath;
using Dapper;
using Logging.Library;
using Assets.Library.Models;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace Assets.Library.Logic
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose:
  /// Created by: rudol
  /// Created on: 1/17/2020 10:26:10 PM 
  /// </summary>
  #endregion
  public class RoutesCollectionDataAccess
    {


    #region Properties

    #endregion

    #region Constructors
   
    #endregion

    #region Methods

    public static List<RouteModel> UpdateRouteTableForAllRoutes(string routesBaseDirPath, bool InGame, bool InArchive)
      {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      List<RouteModel> routeList = new List<RouteModel>();
      try
        {
        DirectoryInfo baseDir = new DirectoryInfo(routesBaseDirPath);
        DirectoryInfo[] routeDirectories = baseDir.GetDirectories("*", SearchOption.TopDirectoryOnly);

        foreach (var routeDir in routeDirectories)
          {
          routeList.Add(UpdateRouteTableForSingleRoute(routeDir, InGame, InArchive));
          }
        SaveRoutesBulkList(routeList, InGame, InArchive);
        }
      catch (Exception ex)
        {
        Log.Trace("Failed to read all routes from disk", ex, LogEventType.Error);
        }
      stopWatch.Stop();
      Int64 elapsed = stopWatch.ElapsedMilliseconds / 1000;
      Log.Trace($"Elapsed time for saving Routes {elapsed} seconds");
      return routeList;
      }

    public static RouteModel UpdateRouteTableForSingleRoute(DirectoryInfo routeDir, bool InGame, bool InArchive)
      {
      RouteModel output = new RouteModel();
      output.RouteGuid = routeDir.Name;
      output.InGame = InGame;
      output.InArchive = InArchive;
      if (File.Exists(@$"{routeDir}\MainContent.ap"))
        {
        output.Pack = "MainContent.ap";
        output.RouteName = GetPackedRouteName(routeDir);
        output.IsPacked = true;
        }
      else
        {
        output.Pack = string.Empty;
        output.RouteName = GetUnpackedRouteName(routeDir);
        output.IsPacked = false;
        }
      output.IsValidInGame = GetValidStatus(output.RouteName, InGame);
      output.IsValidInArchive = GetValidStatus(output.RouteName, InArchive);
      return output;
      }

    static bool GetValidStatus(string routeName, bool location)
      {
      return !(routeName.StartsWith("ZZ") && location);
      }

    public static String GetPackedRouteName(DirectoryInfo routeDir)
      {
      try
        {
        using var archive = ZipFile.OpenRead(@$"{routeDir}\MainContent.ap");
        ZipArchiveEntry entry = archive.GetEntry("RouteProperties.xml");
        if (entry != null)
          {
          using (var Stream = entry.Open())
            {
            using (var Sr = new StreamReader(Stream)) //Copy the compressed stream
              {
                {
                var doc = XDocument.Load(Sr);
                var Node =
                  doc.XPathSelectElement(
                    "cRouteProperties/DisplayName/Localisation-cUserLocalisedString");
                return XmlHelpers.GetLocalisedString(Node);
                }
              }
            }
          }
        return $"ZZ_Invalid{routeDir.Name}"; // Not a valid route
        }
      catch (Exception e)
        {
        Log.Trace($"Failed to get Route Name for Route {routeDir.Name}", e, LogEventType.Error);
        throw;
        }
      }

    public static String GetUnpackedRouteName(DirectoryInfo routeDir)
      {
      try
        {
        if (!File.Exists(@$"{routeDir}\RouteProperties.xml"))
          {
          return $"ZZ_Invalid{routeDir.Name}"; // Not a valid route
          }

        XDocument doc = XDocument.Load(@$"{routeDir}\RouteProperties.xml");
        var Node =
          doc.XPathSelectElement("cRouteProperties/DisplayName/Localisation-cUserLocalisedString");
        return XmlHelpers.GetLocalisedString(Node);
        }
      catch (Exception e)
        {
        Log.Trace($"Failed to get Route Name for Route {routeDir.Name}", e, LogEventType.Error);
        throw;
        }
      }

    public static void SaveRoutesBulkList(List<RouteModel> routesList, Boolean inGame, Boolean inArchive)
      {
      string fieldName=Converters.LocationToString(inGame,inArchive);
      using (IDbConnection connection =
        new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement =
            @$"INSERT OR IGNORE INTO Routes (RouteGuid, RouteName, Pack, IsPacked, IsValid{fieldName}) VALUES (@RouteGuid, @RouteName, @Pack, @IsPacked, @IsValid{fieldName})";
          try
            {
            foreach (var item in routesList)
              {
              connection.Execute(sqlStatement, new { item.RouteGuid, item.RouteName, item.Pack, item.IsPacked, item.IsValidInGame, item.IsValidInArchive},
                transaction);
              }

            transaction.Commit();
            GetRouteIds(routesList);
            UpdateBulkStatus(routesList,fieldName);
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

    // TODO this can be a generic method
    public static void GetRouteIds(List<RouteModel> routesList)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"SELECT Id FROM Routes WHERE RouteGuid= @RouteGuid;";
          try
            {
            foreach (var item in routesList)
              {
              IdModel idModel=connection.Query<IdModel>(sqlStatement, new { item.RouteGuid}, transaction).First();
              item.Id = idModel.Id;
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during getting id's for Routes from database",ex, LogEventType.Error);
            }
          }
        }
      }

    public static int GetRouteId(string routeGuid)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"SELECT Id FROM Routes WHERE RouteGuid= @RouteGuid;";
          try
            {
            IdModel idModel=connection.Query<IdModel>(sqlStatement, new { routeGuid}, transaction).First();
            return idModel.Id;
            }
          catch (Exception ex)
            {
            Log.Trace($"Something went wrong during getting id for {routeGuid} for Routes from database",ex, LogEventType.Error);
            }
          }
        }
      return 0;
      }



    //TODO make this generic
    private static void UpdateBulkStatus(List<RouteModel> routesList,string fieldName)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"UPDATE OR IGNORE Routes SET {fieldName}=1 WHERE Id=@Id";
          try
            {
            foreach (var item in routesList)
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
            Log.Trace($"Something went wrong during bulk update InGame/InArchive field Routes to database",ex, LogEventType.Error);
            }
          }
        }
      }

    public static List<RouteModel> ReadAllRoutesFromDatabase()
      {
      using IDbConnection Db = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString());
      var output = Db.Query<RouteModel>("SELECT * FROM Routes", new { });
      return output.ToList();
      }

    public static async Task  UpdateRouteStatus(int routeId, bool inGame, bool isValidInGame, bool inArchive,
      bool isValidInArchive)
      {
      var sql = "UPDATE OR IGNORE Routes SET InGame=@inGame, IsValidInGame= @isValidInGame, InArchive=@inArchive, IsValidInArchive= @isValidInArchive WHERE Routes.Id=@routeId ";
      await AssetDatabaseAccess.SaveDataAsync(sql, new {routeId, inGame, isValidInGame, inArchive, isValidInArchive },
        AssetDatabaseAccess.GetConnectionString());
      }



    public static List<FileInfo> GetPackFilesForRoute(string Path)
      {
      DirectoryInfo dir= new DirectoryInfo(Path);
      FileInfo[] list = dir.GetFiles("*.ap", SearchOption.TopDirectoryOnly);
      List<FileInfo> output = list.ToList();
      return output;
      }
    #endregion

    #region filter

    //TODO make this genereric
    public static List<RouteModel> ApplyAssetsFilter(List<RouteModel> routesList, RouteFilterModel routesFilter)
      {
      if (routesFilter == null)
        {
        return routesList;
        }
      return routesList.Where(p => RoutesFilter(p, routesFilter)).ToList();
      }

    private static bool RoutesFilter(RouteModel route, RouteFilterModel filter)
      {
      // The filters always are set to true, so we select the item if nothing is set, once a filter has a non-default value, it must cause a match, else it fails.
      if (route == null || filter == null)
        {
        return false;
        }

      var output = Converters.EvaluatePackedLocationFilter(filter.InGameFilter, filter.InArchiveFilter, filter.IsPackedFilter,
                                                                        route.InGame, route.InArchive, route.IsPacked);

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.RouteNameFilter, route.RouteName);
        }
      else
        {
        return false;
        }

      return output;
      }


    #endregion

    #region Helpers
 

 

    #endregion


    }
  }
