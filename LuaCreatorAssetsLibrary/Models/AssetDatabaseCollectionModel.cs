// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf Heijink
// Created          : 12-30-2019
//
// Last Modified By : Rudolf Heijink
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="AssetDatabaseCollectionModel.cs" company="LuaCreatorAssetsLibrary">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#region UsingStatements

using Assets.Library.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dapper;
using Logging.Library;

#endregion

namespace Assets.Library.Models
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose: Manages asset database
  /// Created by: Rudolf Heijink
  /// Created on: 12/27/2019 12:09:16 PM
  /// </summary>
  #endregion
  public class AssetDatabaseCollectionModel : IAssetCollectionModel
    {
    #region Properties

    /// <summary>
    /// Gets the asset database.
    /// </summary>
    /// <value>The asset database.</value>
    protected AssetDatabaseAccess AssetDatabase { get; } = new AssetDatabaseAccess();
    /// <summary>
    /// Gets the asset base path.
    /// </summary>
    /// <value>The asset base path.</value>
    public String AssetBasePath
      {
      get { return $@"C:\LuaCreatorTestdata\Game\Assets\"; }
      }
    #endregion
 
    #region Events
    /// <summary>
    /// Gets or sets the save asset progress reporter.
    /// </summary>
    /// <value>The save asset progress reporter.</value>
    public EventHandler<ProgressPercentage> SaveAssetProgressReporter { get; set; }
    #endregion
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetDatabaseCollectionModel"/> class.
    /// </summary>
    public AssetDatabaseCollectionModel()
      {

      }

    #endregion

    #region Methods

    /// <summary>
    /// Setup method e.g. to create a database, build a list, create test data etcetera ...
    /// </summary>
    /// <returns>Boolean.</returns>
    public Boolean Create()
      {
      return false;
      }

    /// <summary>
    /// Adds an AssetModel object to the collection. Each one must be unique for the tuple provider, product and bluePrint
    /// </summary>
    /// <param name="asset">The asset.</param>
    /// <returns>true if added, false if not added</returns>
    public Boolean AddUnique(AssetModel asset)
      {
      return false;
      }

    /// <summary>
    /// Remove an AssetModel from the collection
    /// </summary>
    /// <param name="asset">The asset.</param>
    /// <returns>true if removed, false if not removed</returns>
    public Boolean Remove(AssetModel asset)
      {
      return false;
      }

    /// <summary>
    /// Apply a filter over the collection and return a filtered collection
    /// </summary>
    /// <param name="providerPattern">The provider pattern.</param>
    /// <param name="productPattern">The product pattern.</param>
    /// <param name="bluePrintPattern">The blue print pattern.</param>
    /// <returns>Filtered collection</returns>
    public IAssetCollectionModel Filter(String providerPattern, String productPattern,
      String bluePrintPattern)
      {
      return null;
      }

    public static void SaveAssetsBulk(List<AssetModel> assets, int providerProductId)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"INSERT OR IGNORE INTO Assets (ProvProdId, BluePrintPath) 
                                   VALUES (@providerProductId, @BluePrintPath)";
          try
            {
            foreach (var item in assets)
              {
              connection.Execute(sqlStatement, new { providerProductId, item.BluePrintPath }, transaction);
              }
            transaction.Commit();
            }
          catch (Exception e)
            {
            transaction.Rollback();
            Log.Trace($"Bulk save in database for assets failed, rolled back",e,LogEventType.Error);
            }
          }
        }
      }

    //Note: this version will pickup the providerProductId
    public static void SaveAssetsBulk(List<AssetModel> assets)
      {
      int providerProductId;
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"INSERT OR IGNORE INTO Assets (ProvProdId, BluePrintPath) 
                                   VALUES (@providerProductId, @BluePrintPath)";
          
          try
            {
            foreach (var item in assets)
              {
              providerProductId = connection.Query<int>(@$"select Id from ProviderProduct 
                          WHERE Provider=@Provider 
                          AND Product=@Product", new {item.ProviderProduct.Provider,item.ProviderProduct.Product}).First();
              connection.Execute(sqlStatement, new { providerProductId, item.BluePrintPath }, transaction);
              }
            transaction.Commit();
            }
          catch (Exception e)
            {
            transaction.Rollback();
            Log.Trace($"Bulk save in database for assets failed, rolled back",e,LogEventType.Error);
            }
          }
        }
      }

    public List<AssetModel> ReadAllAssetsFromDirectory(DirectoryInfo providerProductDir, ProviderProductModel providerProduct, int pathLength)
      {
      try
        {
        FileInfo[] bluePrintFiles =
          providerProductDir.GetFiles("*.bin", SearchOption.AllDirectories);

        List<AssetModel> assets = new List<AssetModel>();

        foreach (FileInfo bluePrintFile in bluePrintFiles)
          {
          String bluePrintPath = bluePrintFile.FullName.Substring(pathLength + 1);
          assets.Add(new AssetModel()
            {
            ProviderProduct = providerProduct,
            BluePrintPath = bluePrintPath
            });
          }
        return assets;
        }
      catch (Exception e)
        {
        Log.Trace($"Failed to read asset files from directory {providerProductDir}",e,LogEventType.Error);
        throw;
        }
      }

    public void SaveAllUnpackedAssets()
      {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      try
        {
        List<ProviderProductModel> ProviderProductList =ProviderProductDatabaseCollectionModel.ReadAllProviderProductsFromDatabase();
        ProviderProductList= ProviderProductList.Where(x=>x.Pack.Length==0).ToList();
        
        ProgressPercentage progress = new ProgressPercentage()
          {
          TotalWork = ProviderProductList.Count,
          CurrentProgress = 0,
          Message = "Saving assets progress"
          };

        foreach (ProviderProductModel providerProduct in ProviderProductList)
          {
          Int32 providerProductId = providerProduct.GetDatabaseRecordId();
          String tempPath = $@"{AssetBasePath}{providerProduct.ArchiveFileName}";
          if (Directory.Exists(tempPath))
            {
            DirectoryInfo providerProductDir = new DirectoryInfo(tempPath);
            var assets =
              ReadAllAssetsFromDirectory(providerProductDir, providerProduct, tempPath.Length);
            SaveAssetsBulk(assets, providerProductId);
            progress.CurrentProgress++;
            SaveAssetProgressReporter?.Invoke(this, progress);
            }
          }
        }
      catch (Exception ex)
        {
        Log.Trace($"Saving unpacked assets failed ",ex,LogEventType.Error);
        }

 
      }

    public List<AssetModel> ZipEntriesToAssetList(List<string> entries,
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

    public void SaveAllPackedAssets()
      {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
 
        try
          {
          // filter this list, to make sure it actually is packed
          List<ProviderProductModel> ProviderProductList = (ZipAccess.GetPackedProviderProducts(new DirectoryInfo(AssetBasePath))).Where(x => x.Pack.Length >0).ToList();

          ProgressPercentage progress = new ProgressPercentage()
            {
            TotalWork = ProviderProductList.Count,
            CurrentProgress = 0,
            Message = "Saving assets progress"
            };

          foreach (ProviderProductModel providerProduct in ProviderProductList)
            {
            Int32 providerProductId = providerProduct.GetDatabaseRecordId();
            var entries =
              ZipAccess.GetAllZipEntries(AssetBasePath, providerProduct.ArchiveFileName);
            var assets = ZipEntriesToAssetList(entries, providerProduct);
            SaveAssetsBulk(assets, providerProductId);
            progress.CurrentProgress++;
            SaveAssetProgressReporter?.Invoke(this, progress);
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

    #endregion

    #region Helpers
    public override String ToString()
      {
      throw new NotImplementedException("You should implement ToString() in AssetDatabaseCollectionModel");
      }

    #endregion
    }
  }
