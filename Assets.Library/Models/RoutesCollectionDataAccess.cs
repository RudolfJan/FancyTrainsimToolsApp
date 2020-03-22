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
      string fieldName="";
      if (inGame)
        {
        fieldName = "InGame";
        }

      if (inArchive)
        {
        fieldName = "InArchive";
        }

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
            if (inGame)
              {
              UpdateBulkStatus(routesList,"InGame");
              }
            if(inArchive)
              {
              UpdateBulkStatus(routesList,"InArchive");
              }
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
    #endregion

    #region Helpers
    public override string ToString()
      {
      throw new NotImplementedException("You should implement ToString() in RoutesCollectionDataAccess");
      }


    #endregion


    }
  }
