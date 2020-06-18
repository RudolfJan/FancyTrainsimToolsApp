// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf Heijink
// Created          : 12-30-2019
//
// Last Modified By : Rudolf Heijink
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="AssetCollectionDataAccess.cs" company="LuaCreatorAssetsLibrary">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#region UsingStatements

using Assets.Library.Helpers;
using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dapper;
using Logging.Library;
using System.IO.Compression;

#endregion

namespace Assets.Library.Logic
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose: Manages asset database
  /// Created by: Rudolf Heijink
  /// Created on: 12/27/2019 12:09:16 PM
  /// </summary>
  #endregion
  public class AssetCollectionDataAccess
    {
    #region Properties

    #endregion

    #region Events
    /// <summary>
    /// Gets or sets the save asset progress reporter.
    /// </summary>
    /// <value>The save asset progress reporter.</value>
    public static EventHandler<ProgressPercentageModel> SaveAssetProgressReporter { get; set; }
    #endregion

    #region Methods

    public static void SaveAssetsBulk(List<AssetModel> assets, int providerProductId)
      {
      AssetModel itemForLog = new AssetModel();
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"INSERT OR IGNORE INTO Assets (ProvProdId, BluePrintPath) 
                                   VALUES (@providerProductId, @BluePrintPath )";
          try
            {
            foreach (var item in assets)
              {
              itemForLog = item;
              connection.Execute(sqlStatement, new { providerProductId, item.BluePrintPath }, transaction);
              var lastRow = connection.ExecuteScalar("SELECT last_insert_rowid();",new{},transaction);
              item.Id = (int) (long) lastRow;
              }
            transaction.Commit();
            }
          catch (Exception e)
            {
            transaction.Rollback();
            Log.Trace($"Bulk save in database for assets failed {itemForLog}, rolled back", e, LogEventType.Error);
            }
          }
        }
      }


    //Note: this version will pickup the providerProductId, providerProducts MUST be in database
    public static void SaveAssetsBulk(List<AssetModel> assets)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"INSERT OR IGNORE INTO Assets (ProvProdId, BluePrintPath, InGame, InArchive) 
                                   VALUES (@ProvProdId, @BluePrintPath, @InGame, @InArchive)";

          try
            {
            foreach (var item in assets)
              {
              if (item.ProviderProduct.Id < 1
                ) // Normally this is filled while the provider/products are created just to be sure ...
                {
                var providerProductId = connection.Query<int>(@$"SELECT Id FROM ProviderProducts 
                          WHERE Provider=@Provider 
                          AND Product=@Product",
                  new {item.ProviderProduct.Provider, item.ProviderProduct.Product}).First();
                item.ProviderProduct.Id = providerProductId;
                }
              var result = connection.Execute(sqlStatement,
                  new {ProvProdId = item.ProviderProduct.Id, item.BluePrintPath, item.InGame, item.InArchive},
                  transaction);
                item.IsNew = result > 0; // DEBUG
                
              }
            transaction.Commit();
            }
          catch (Exception e)
            {
            transaction.Rollback();
            Log.Trace($"Bulk save in database for assets failed, rolled back", e, LogEventType.Error);
            }
          }
        }
      }

    public static List<AssetModel> ReadAllAssetsFromDirectory(DirectoryInfo providerProductDir, ProviderProductModel providerProduct, int pathLength, bool inGame, bool inArchive)
      {
      try
        {
        FileInfo[] bluePrintFiles =
          providerProductDir.GetFiles("*.bin", SearchOption.AllDirectories);

        List<AssetModel> assets = new List<AssetModel>();

        foreach (FileInfo bluePrintFile in bluePrintFiles)
          {
          string bluePrintPath = bluePrintFile.FullName.Substring(pathLength);
          assets.Add(new AssetModel()
            {
            ProviderProduct = providerProduct,
            BluePrintPath = bluePrintPath,
            InGame = inGame,
            InArchive = inArchive
            });
          }
        return assets;
        }
      catch (Exception ex)
        {
        Log.Trace($"Failed to read asset files from directory {providerProductDir}", ex, LogEventType.Error);
        throw;
        }
      }

    public static void SaveAllUnpackedAssets(string assetBasePath, bool InGame, bool InArchive)
      {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      try
        {
        List<ProviderProductModel> ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
        ProviderProductList = ProviderProductList.Where(x => x.Pack.Length == 0).ToList();

        ProgressPercentageModel progress = new ProgressPercentageModel()
          {
          TotalWork = ProviderProductList.Count,
          CurrentProgress = 0,
          Message = "Saving assets progress"
          };

        foreach (ProviderProductModel providerProduct in ProviderProductList)
          {
          providerProduct.Id = providerProduct.GetDatabaseRecordId();
          String tempPath = $@"{assetBasePath}{providerProduct.ArchiveFileName}";
          if (Directory.Exists(tempPath))
            {
            DirectoryInfo providerProductDir = new DirectoryInfo(tempPath);
            List<AssetModel> assets = null;
            assets =
              ReadAllAssetsFromDirectory(providerProductDir, providerProduct, tempPath.Length, InGame, InArchive);
            SaveAssetsBulk(assets, providerProduct.Id);
            UpdateBulkStatus(assets, Converters.LocationToString(InGame,InArchive));
            progress.CurrentProgress++;
            SaveAssetProgressReporter?.Invoke(null, progress);
            }
          }
        }
      catch (Exception ex)
        {
        Log.Trace($"Saving unpacked assets failed ", ex, LogEventType.Error);
        }
      }

    public static List<AssetModel> ZipEntriesToAssetList(List<string> entries,
      ProviderProductModel providerProduct)
      {
      List<AssetModel> assets = new List<AssetModel>();
      foreach (var entry in entries)
        {
        assets.Add(new AssetModel()
          {
          ProviderProduct = providerProduct,
          BluePrintPath = entry.ConvertToForwardSlashes().RemoveFileType()
          });
        }
      return assets;
      }

    public static void SaveAllPackedAssets(string assetBasePath, bool InGame, bool InArchive)
      {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

      try
        {
        // filter this list, to make sure it actually is packed
        List<ProviderProductModel> ProviderProductList = (ZipAccess.GetPackedProviderProducts(new DirectoryInfo(assetBasePath))).Where(x => x.Pack.Length > 0).ToList();

        ProgressPercentageModel progress = new ProgressPercentageModel()
          {
          TotalWork = ProviderProductList.Count,
          CurrentProgress = 0,
          Message = "Saving assets progress"
          };

        foreach (ProviderProductModel providerProduct in ProviderProductList)
          {
          providerProduct.Id = providerProduct.GetDatabaseRecordId();
          if (providerProduct.Id <= 0)
            {
            Log.Trace($"ProviderProduct {providerProduct} not found in Database. Please file a ticket",
              LogEventType.Error);
            throw new InvalidDataException("ProviderProduct not found in Database. Please file a ticket");
            }
          var entries =
            ZipAccess.GetAllZipEntries(assetBasePath, providerProduct.ArchiveFileName);
          var assets = ZipEntriesToAssetList(entries, providerProduct);
          SaveAssetsBulk(assets, providerProduct.Id);
          UpdateBulkStatus(assets, Converters.LocationToString(InGame,InArchive));
          progress.CurrentProgress++;
          SaveAssetProgressReporter?.Invoke(null, progress);
          }
        }
      catch (Exception ex)
        {
        Log.Trace($"Saving packed assets failed", ex, LogEventType.Error);
        }

      stopWatch.Stop();
      Int64 elapsed = stopWatch.ElapsedMilliseconds / 1000;
      Log.Trace($"Elapsed time for saving assets {elapsed} seconds");
      }


    public static int GetAssetIdFromDatabase(string provider, string product, string bluePrint)
      {
      string sqlStatement =
        $"SELECT Assets.Id FROM Assets, ProviderProducts WHERE BluePrintPath=@BluePrint AND Assets.ProvProdId= ProviderProducts.Id AND ProviderProducts.Provider=@Provider AND ProviderProducts.Product=@Product";
      return (int) AssetDatabaseAccess.LoadData<Int64,dynamic>(sqlStatement, new { provider, product, bluePrint },AssetDatabaseAccess.GetConnectionString()).FirstOrDefault();
      }

    public static int GetAssetIdFromDatabase(AssetModel item, IDbConnection connection,
      IDbTransaction transaction)
      {
      string sqlStatement =
        $"SELECT Id FROM Assets WHERE ProvProdId=@ProvProdId AND BluePrintPath=@BluePrintPath";
      return connection.QueryFirst<int>(sqlStatement, new { ProvProdId = item.ProviderProduct.Id, item.BluePrintPath }, transaction);
      }

    public static int InsertLooseAsset(int providerProductId, string bluePrint, bool inGame=false, bool inArchive=false)
      {
      string sql = "INSERT OR IGNORE INTO Assets (ProvProdId, BlueprintPath,InGame,InArchive) VALUES(@providerProductId, @bluePrint,@InGame,@InArchive);" +
                   "SELECT last_insert_rowid();";
      return (int) AssetDatabaseAccess.LoadData<Int64,dynamic>(sql,new{providerProductId,bluePrint,inGame, inArchive},AssetDatabaseAccess.GetConnectionString()).FirstOrDefault();
      }

    public static void UpdateBulkStatus(List<AssetModel> assetList, string fieldName)
      {
      AssetModel itemForLog = new AssetModel();
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"UPDATE OR IGNORE Assets SET {fieldName}=1 WHERE Id=@Id";
          try
            {
            foreach (var item in assetList)
              {
              itemForLog = item;
              if (item.Id <= 0)
                {
                item.Id = GetAssetIdFromDatabase(item, connection, transaction);
                }
              if (item.Id <= 0)
                {
                // if we now do not have an Id, we have a big problem, the record is not in the database.
                Log.Trace($"Internal error. Id for Assets not set in {item}", LogEventType.Error);
                throw new InvalidDataException($"Internal error. Id for Assets not set in {item}");
                }

              try
                {
                var result = connection.Execute(sqlStatement, new {item.Id}, transaction);
                if (result != 1)
                  {
                  Log.Trace($"Location not updated for {item}", LogEventType.Error);
                  }
                }
              catch(Exception ex)
                {
                Log.Trace($"Location update error {item} skipped",ex, LogEventType.Message);
                }
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during bulk update {itemForLog.ProviderProduct.ProviderProduct} {itemForLog.BluePrintPath} {itemForLog.Id} {itemForLog.ProviderProduct.Id} InGame/InArchive field Assets to database", ex, LogEventType.Error);
            Log.Trace($" {itemForLog.Id} {itemForLog.ProviderProduct.Id} InGame/InArchive field Assets to database", ex, LogEventType.Error);
            }
          }
        }
      }




    #endregion

    public static List<FlatAssetModel> ReadAllAssetsFromDatabase()
      {
      using IDbConnection Db = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString());
      var output = Db.Query<FlatAssetModel>("SELECT * FROM BluePrintView", new { });
      return output.ToList();
      }

    #region Filters

    public static List<FlatAssetModel> ApplyAssetsFilter(List<FlatAssetModel> bluePrintList, BluePrintFilterModel bluePrintFilter)
      {
      return bluePrintList.Where(p => AssetsFilter(p, bluePrintFilter)).ToList();
      }

    private static Boolean AssetsFilter(FlatAssetModel bluePrint, BluePrintFilterModel filter)
      {
      // The filters always are set to true, so we select the item if nothing is set, once a filter has a non-default value, it must cause a match, else it fails.
      if (bluePrint == null || filter == null)
        {
        return false;
        }

      var output = Converters.EvaluateLocationFilter(filter.InGameFilter, filter.InArchiveFilter,
        bluePrint.InGame, bluePrint.InArchive);

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.ProviderFilter, bluePrint.Provider);
        }
      else
        {
        return false;
        }

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.ProductFilter, bluePrint.Product);
        }
      else
        {
        return false;
        }

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.BluePrintFilter, bluePrint.BluePrintPath);
        }
      else
        {
        return false;
        }

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.PackFilter, bluePrint.Pack);
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
