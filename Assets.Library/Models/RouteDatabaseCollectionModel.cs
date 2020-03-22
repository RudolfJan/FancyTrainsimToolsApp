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
  public class RouteDatabaseCollectionModel
    {


    #region Properties
    
    #endregion

    #region Constructors
    public RouteDatabaseCollectionModel()
      {

      }

    #endregion

    #region Methods

    public static void UpdateRouteTableForAllRoutes(string routesBaseDirPath)
      {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

      DirectoryInfo baseDir= new DirectoryInfo(routesBaseDirPath);
      DirectoryInfo[] routeDirectories = baseDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
      List<RouteModel> routeList= new List<RouteModel>();
      foreach (var routeDir in routeDirectories)
        {
        routeList.Add(UpdateRouteTableForSingleRoute(routeDir));
        }
      SaveRoutesBulkList(routeList);
      stopWatch.Stop();
      Int64 elapsed = stopWatch.ElapsedMilliseconds / 1000;
      Log.Trace($"Elapsed time for saving Routes {elapsed} seconds");
      }

    public static  RouteModel UpdateRouteTableForSingleRoute(DirectoryInfo routeDir)
      {
      RouteModel output= new RouteModel();
      output.RouteGuid = routeDir.Name;
      if (File.Exists(@$"{routeDir}\MainContent.ap"))
        {
        output.Pack = "MainContent.ap";
        output.RouteName = GetPackedRouteName(routeDir);
        }
      else
        {
        output.RouteName = GetUnpackedRouteName(routeDir);
        }
      return output;
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
        return String.Empty;
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
        Log.Trace($"Failed to get Route Name for Route {routeDir.Name}",e,LogEventType.Error);
        throw;
        }
      }


    public static void SaveRoutesBulkList(List<RouteModel>  routesList)
      {
      using (IDbConnection connection =
        new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement =
            @$"INSERT OR IGNORE INTO Routes (RouteGuid, RouteName, Pack) VALUES (@RouteGuid, @RouteName, @Pack)";
          try
            {
            foreach (var item in routesList)
              {
              connection.Execute(sqlStatement, new {item.RouteGuid, item.RouteName, item.Pack},
                transaction);
              }

            transaction.Commit();
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
    #endregion

    #region Helpers
    public override string ToString()
      {
      throw new NotImplementedException("You should implement ToString() in RouteDatabaseCollectionModel");
      }


    #endregion


    }
  }
