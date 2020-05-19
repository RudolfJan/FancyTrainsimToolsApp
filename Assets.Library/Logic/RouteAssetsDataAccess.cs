#region UsingStatements

using Assets.Library.Helpers;
using Assets.Library.Logic;
using Assets.Library.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
  /// Created on: 1/17/2020 10:23:47 PM 
  /// </summary>

  #endregion

  public class RouteAssetsDataAccess
    {


    #region Properties

    #endregion

    #region Constructors


    #endregion

    #region Methods

    public static async Task<List<FullRouteProviderProductsModel>> GetProviderProductsForRoute(
      int routeId)
      {
      string sql = "SELECT * FROM FullRouteProviderProducts WHERE RouteId=@routeId ORDER BY Provider ASC, Product ASC";
      var output =
        await AssetDatabaseAccess.LoadDataAsync<FullRouteProviderProductsModel, dynamic>(sql,
          new {routeId}, AssetDatabaseAccess.GetConnectionString());
      return output.ToList();
      }

    public static async Task<List<FullRouteAssetsModel>> GetAssetsForRoute(
      int routeId)
      {
      string sql = "SELECT * FROM FullRouteAssets WHERE RouteId=@routeId ORDER BY Provider ASC, Product ASC, BluePrintPath ASC";
      var output =
        await AssetDatabaseAccess.LoadDataAsync<FullRouteAssetsModel, dynamic>(sql,
          new {routeId}, AssetDatabaseAccess.GetConnectionString());
      return output.ToList();
      }



    public static void CreateAllRouteAssetsInDatabase(string routesBasePath)
      {
      int i = 0;

      var routesList = LoadRoutesToList();
      foreach (var route in routesList)
        {
        CreateRouteAssetsInDatabase(route, routesBasePath);
        Log.Trace(
          $"Finished route {route.RouteName} {i++}");
        }
      }

    public static void CreateRouteAssetsInDatabase(RouteModel route, string routesBasePath)
      {
     if (string.IsNullOrEmpty(route.Pack))
          {
          var binList = LoadUnpackedRouteBinFilesToList(routesBasePath, route);
          List<RouteAssetsModel> routeAssets = new List<RouteAssetsModel>();
          foreach (var binFile in binList)
            {
            routeAssets.AddRange(LoadRouteAssetsFromBinFile(route, binFile));
            }
          routeAssets = routeAssets.DistinctBy(x => x.Asset.AssetPath).ToList();
          SaveRouteAssetsBulkToDatabase(routeAssets);
 
          }
      else
        {
        //TODO
        }
      }


  private static List<RouteAssetsModel> LoadRouteAssetsFromBinFile(RouteModel route,
        FileInfo fileInfo)
    {
    var doc = BinHandler.SerzToDoc(fileInfo.FullName);
    List<RouteAssetsModel> output = GetBluePrintsFromXML(route, doc);
    return output;
    }

  public static List<RouteAssetsModel> GetBluePrintsFromXML(RouteModel route, XDocument Doc)
    {
    List<RouteAssetsModel> output = new List<RouteAssetsModel>();
    try
      {
      var ElementList =
        (IEnumerable)Doc.XPathEvaluate("//iBlueprintLibrary-cAbsoluteBlueprintID");
      foreach (XElement BluePrintNode in ElementList)
        {
        var result = GetBluePrintDetails(BluePrintNode);
        if (string.IsNullOrEmpty(result.BluePrint) == false)
          {
          var providerProduct = new ProviderProductModel
            {
            Provider = result.Provider,
            Product = result.Product
            };
          var asset = new AssetModel
            {
            ProviderProduct = providerProduct,
            BluePrintPath = result.BluePrint
            };
          var routeAsset = new RouteAssetsModel()
            {
            Route = route,
            Asset = asset
            };
          output.Add(routeAsset);
          }
        }

      return output.DistinctBy(x => x.Asset.AssetPath).ToList();
      }
    catch (Exception e)
      {
      Log.Trace("Failed to obtain blueprint details", e, LogEventType.Error);
      throw;
      }
    }

  public static (String Provider, String Product, String BluePrint) GetBluePrintDetails(
      XElement BluePrintNode)
      {
      try
        {
        var Provider = BluePrintNode.XPathSelectElement(@"//Provider")?.Value;
        var Product = BluePrintNode.XPathSelectElement(@"//Product")?.Value;
        // Note: BlueprintID occurs at two levels, in the code below you make sure to select the correct level
        // iBlueprintLibrary-cAbsoluteBlueprintID
        var BluePrintPathNode =
          BluePrintNode.XPathSelectElement(@"./BlueprintID");
        var BluePrint = BluePrintPathNode?.Value;
        BluePrint = BluePrint?.Replace('\\', '/').Replace(".bin", "").Replace(".xml", "");
        return (Provider, Product, BluePrint);
        }
      catch (Exception E)
        {
        Log.Trace("Error reading  XML node", E, LogEventType.Error);
        throw;
        }
      }

    public static List<RouteModel> LoadRoutesToList()
      {
      try
        {
        // Get all routes
        string sqlStatement = @$"SELECT * FROM Routes";
        var output = AssetDatabaseAccess.LoadData<RouteModel, dynamic>(sqlStatement, new { },
          AssetDatabaseAccess.GetConnectionString());
        if (output.Count == 0)
          {
          Log.Trace("No routes found in database. You need to save routes before using them.",
            null, LogEventType.Message);
          }

        return output;
        }
      catch (Exception e)
        {
        Log.Trace("Failed to load routes from database to list", e, LogEventType.Error);
        throw;
        }
      }

    public static List<FileInfo> LoadUnpackedRouteBinFilesToList(string routesBasePath,
      RouteModel route)
      {
      if (route == null || string.IsNullOrEmpty(route.RouteGuid))
        {
        Log.Trace("Route object is not valid", null, LogEventType.Error);
        return null;
        }

      if (string.IsNullOrEmpty(route.Pack) == false)
        {
        Log.Trace("Route object points to packed route. Please report this error.", null,
          LogEventType.Error);
        return null;
        }

      string path = @$"{routesBasePath}{route.RouteGuid}\";
      try
        {
        DirectoryInfo dir = new DirectoryInfo(@$"{path}Scenery\");
        var output = dir.GetFiles("*.bin", SearchOption.AllDirectories).ToList();
        dir = new DirectoryInfo(@$"{path}Networks\Loft Tiles\");
        output.AddRange(dir.GetFiles("*.bin", SearchOption.AllDirectories));
        dir = new DirectoryInfo(@$"{path}Networks\Road Tiles\");
        output.AddRange(dir.GetFiles("*.bin", SearchOption.AllDirectories));
        dir = new DirectoryInfo(@$"{path}Networks\Track Tiles\");
        output.AddRange(dir.GetFiles("*.bin", SearchOption.AllDirectories));
        return output;
        }
      catch (Exception e)
        {
        Log.Trace($"Failed to route bin files from path {path} to list", e, LogEventType.Error);
        throw;
        }
      }

    private static void SaveRouteAssetsBulkToDatabase(List<RouteAssetsModel> routeAssets)
      {
      // Step 1: add all assets tot he assets table
      // Step 2: insert provider products in the table
      // Step 3: get for each asset the id from the assets table (all assets will be there)
      // Step 4: insert route Id and asset id in the 

      List<AssetModel> assets = routeAssets.Select(x => x.Asset).ToList();
      List<ProviderProductModel> providerProducts =
        routeAssets.Select(x => x.Asset.ProviderProduct).DistinctBy(x => x.ProviderProduct)
          .ToList();
      ProviderProductCollectionDataAccess.SaveProviderProductsBulk(providerProducts);
      AssetCollectionDataAccess.SaveAssetsBulk(assets);

      providerProducts = assets.Select(x => x.ProviderProduct).ToList();
      // make sure all providerProducts have an Id 
      ProviderProductCollectionDataAccess.UpdateListIdsFromDatabase(providerProducts);
      // Bulk insert
      using (IDbConnection connection =
        new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"INSERT OR IGNORE INTO RouteAssets (RouteId,AssetId)
                                    SELECT Routes.Id, Assets.Id FROM Routes, Assets
	                                  WHERE Routes.Id= (SELECT Id FROM Routes WHERE RouteGuid= @RouteGuid) AND
                                    Assets.Id= (SELECT Id FROM Assets WHERE BlueprintPath=@BluePrintPath AND 
                                    Assets.ProvProdId=@Id); ";
          try
            {
            foreach (var item in routeAssets)
              {
              connection.Execute(sqlStatement,
                new {item.Route.RouteGuid, item.Asset.BluePrintPath, item.Asset.ProviderProduct.Id},
                transaction);
              }

            transaction.Commit();
            }
          catch (Exception e)
            {
            transaction.Rollback();
            Log.Trace($"Bulk save in database for assets failed, rolled back", e,
              LogEventType.Error);
            }
          }
        }
      }

    #endregion

    #region Helpers


    #endregion
    }
  }
