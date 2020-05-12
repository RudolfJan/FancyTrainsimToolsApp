#region UsingStatements

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

  public class RouteAssetsCollectionDataAccess
    {


    #region Properties

    #endregion

    #region Constructors


    #endregion

    #region Methods

    public static async Task CreateAllRouteAssetsInDatabaseAsync(string routesBasePath, IProgress<BasicProgressModel> progress, CancellationToken cancellationToken)
      {
      int i = 0;
      progress = new Progress<BasicProgressModel>();
      var report = new BasicProgressModel
        {
        Description = "Create route assets"
        };
      report.watch.Start();

      var routesList = LoadRoutesToList();
      report.AmountToDo = routesList.Count;

      await Task.Run(() =>
        {
          Parallel.ForEach<RouteModel>(routesList, async (route) =>
            {
              if (string.IsNullOrEmpty(route.Pack))
                {
                var binList = LoadUnpackedRouteBinFilesToList(routesBasePath, route);
                List<RouteAssetsModel> routeAssets = new List<RouteAssetsModel>();
                foreach (var binFile in binList)
                  {
                  routeAssets.AddRange(LoadRouteAssetsFromBinFile(route, binFile));
                  // CLog.Trace($"Working route file {route.RouteName} {j++} {routeAssets.Count}");
                  }

                routeAssets = routeAssets.DistinctBy(x => x.Asset.AssetPath).ToList();
                SaveRouteAssetsBulkToDatabase(route, routeAssets);
                Log.Trace(
                $"Finished route {route.RouteName} {i++} total objects {routeAssets.Count}");
                }
              else
                {
                //TODO
                }

              report.AmountDone++;
              report.IsDone = (report.AmountDone == report.AmountToDo);
              progress.Report(report);
            });
        }, cancellationToken);
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
          // ReSharper disable once UseDeconstruction
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

    private static void SaveRouteAssetsBulkToDatabase(RouteModel route,
      List<RouteAssetsModel> routeAssets)
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
              connection.Execute(sqlStatement, new { item.Route.RouteGuid, item.Asset.BluePrintPath, item.Asset.ProviderProduct.Id }, transaction);
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

    public override string ToString()
      {
      throw new NotImplementedException(
        "You should implement ToString() in RouteAssetsDatabaseCollectionModel");
      }

    }

  #endregion
  // https://stackoverflow.com/questions/10632776/fastest-way-to-remove-duplicate-value-from-a-list-by-lambda
  /// IEnumerable<Foo> distinctList = sourceList.DistinctBy(x => x.FooName);

  static class DistinctList
    {
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector)
      {
      var knownKeys = new HashSet<TKey>();
      return source.Where(element => knownKeys.Add(keySelector(element)));
      }
    }
  }
