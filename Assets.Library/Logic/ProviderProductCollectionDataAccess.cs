// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf Heijink
// Created          : 12-30-2019
//
// Last Modified By : rudol
// Last Modified On : 01-17-2020
// ***********************************************************************
// 
// <summary>
// Libray to manage assets for TrainSimulator game
// </summary>
// ***********************************************************************
#region UsingStatements

using Assets.Library.Helpers;
using Assets.Library.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Dapper;
using Logging.Library;
using System.Text.RegularExpressions;
using Assets.Library.Models;

#endregion

namespace Assets.Library.Logic
  {
 
  public class ProviderProductCollectionDataAccess
    {
    #region Properties
    /// <summary>
    /// Gets the asset base path.
    /// </summary>
    /// <value>The asset base path.</value>
    public String AssetBasePath
      {
      get { return $@"C:\LuaCreatorTestdata\Game\Assets\"; }
      }
     
    #endregion

    #region Constructors
 
    #endregion

    #region Methods

    /// <summary>
    /// Bulk save into database for ProviderProductModels, using transaction
    /// </summary>
    /// <param name="providerProducts">List of ProviderProducts</param>
    public static void SaveProviderProductsBulk(List<ProviderProductModel> providerProducts)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"INSERT OR IGNORE INTO ProviderProducts (Provider, Product, Pack, InGame, InArchive) VALUES (@Provider, @Product, @Pack, @InGame, @InArchive)";
          try
            {
            foreach (var item in providerProducts)
              {
              connection.Execute(sqlStatement, new { item.Provider, item.Product, item.Pack, item.InGame, item.InArchive}, transaction);
              var lastRow = (int) (long) connection.ExecuteScalar("SELECT last_insert_rowid();",new{},transaction);
              if (lastRow > 0)
                {
                item.Id = lastRow;
                }
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during bulk save ProviderProduct to database",ex, LogEventType.Error);
            }
          }
        }
      }

    public static int InsertLooseProviderProduct(string provider, string product, string pack)
      {
      var result = ProviderProductCollectionDataAccess.GetProviderProductId(provider, product, pack);
      if (result > 0)
        {
        return result;
        }
      string sqlStatement = @$"INSERT OR IGNORE INTO ProviderProducts (Provider, Product, Pack) VALUES (@Provider, @Product, @Pack);";
      AssetDatabaseAccess.SaveData(sqlStatement, new {provider, product, pack},
        AssetDatabaseAccess.GetConnectionString());
     return ProviderProductCollectionDataAccess.GetProviderProductId(provider, product, pack);
      }

    public static ProviderProductModel UpsertProviderProduct(string provider, string product, string pack)
      {
      var result = ProviderProductCollectionDataAccess.GetProviderProductModel(provider, product, pack);
      if (result?.Id > 0)
        {
        return result;
        }
      string sqlStatement = @$"INSERT OR IGNORE INTO ProviderProducts (Provider, Product, Pack) VALUES (@Provider, @Product, @Pack);";
      AssetDatabaseAccess.SaveData(sqlStatement, new {provider, product, pack},
        AssetDatabaseAccess.GetConnectionString());
      return ProviderProductCollectionDataAccess.GetProviderProductModel(provider, product, pack);
      }

    private static Int32 GetProviderProductId(string provider, string product, string pack)
      {
      string sql="SELECT id FROM ProviderProducts WHERE Provider=@provider AND Product=@product AND Pack = @pack;";
      return AssetDatabaseAccess.LoadData<int, dynamic>(sql, new {provider, product, pack},AssetDatabaseAccess.GetConnectionString()).First();
      }

    private static ProviderProductModel GetProviderProductModel(string provider, string product, string pack)
      {
      string sql="SELECT Id, Provider, Product, Pack FROM ProviderProducts WHERE Provider=@provider AND Product=@product AND Pack = @pack;";
      return AssetDatabaseAccess.LoadData<ProviderProductModel, dynamic>(sql, new {provider, product, pack},AssetDatabaseAccess.GetConnectionString()).FirstOrDefault();
      }


    /// <summary>
    /// Reads the provider product from directory.
    /// </summary>
    /// <param name="productDirectory">The product directory.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <returns>ProviderProductModel.</returns>
    public static ProviderProductModel ReadProviderProductFromDirectory(DirectoryInfo productDirectory, string providerName)
      {
      ProviderProductModel providerProduct = new ProviderProductModel()
        {
        Provider= providerName,
        Product = productDirectory.Name
        };
      var packs=productDirectory.GetFiles("*.ap", SearchOption.TopDirectoryOnly);
      if (packs.Length == 0)
        {
        providerProduct.Pack = "";
        }
      else
        {
        providerProduct.Pack = Path.GetFileNameWithoutExtension(packs[0].Name);
        }
      if (packs.Length > 1)
        {
        Log.Trace($"SaveAllProviderProducts warning duplicate .ap file found"); 
        }
      return providerProduct;
      }

    /// <summary>
    /// Reads all provider/product combinations from the assets folder.
    /// </summary>
    /// <param name="assetDirectory">The asset directory.</param>
    /// <returns>List&lt;ProviderProductModel&gt;.</returns>
    public static List<ProviderProductModel> ReadProviderProductListFromDirectory(
      DirectoryInfo assetDirectory)
      {
      List<ProviderProductModel> output = new List<ProviderProductModel>();
      try
        {
        DirectoryInfo[] providerDirectories =
          assetDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly);
        
        foreach (DirectoryInfo providerDirectory in providerDirectories)
          {
          DirectoryInfo[] productDirectories =
            providerDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly);

          foreach (var productDirectory in productDirectories)
            {
            output.Add(
              ReadProviderProductFromDirectory(productDirectory, providerDirectory.Name));
            }
          }
        }
      catch (Exception ex)
        {
        Log.Trace($"Something went wrong when trying to obtain Providers and Products",ex, LogEventType.Error);
        }
      return output;
      }

    /// <summary>
    /// Reads and saves all provider products.
    /// </summary> 
    public static void SaveAllProviderProducts(string assetBasePath, bool inGame, bool inArchive)
      {
      try
        {
        DirectoryInfo assetDirectory = new DirectoryInfo(assetBasePath);
        List<ProviderProductModel> providerProductList = ReadProviderProductListFromDirectory(assetDirectory);
        SaveProviderProductsBulk(providerProductList);
        UpdateListIdsFromDatabase(providerProductList);
        UpdateBulkStatus(providerProductList,Converters.LocationToString(inGame, inArchive));
        }
      catch (Exception ex)
        {
        Log.Trace($"Something went wrong during saving ProviderProduct to database",ex,LogEventType.Error);
        }
      }

    private static void UpdateBulkStatus(List<ProviderProductModel> providerProducts,string fieldName)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"UPDATE OR IGNORE ProviderProducts SET {fieldName}=1 WHERE Id=@Id";
          try
            {
            foreach (var item in providerProducts)
              {
              if (item.Id <= 0)
                {
                Log.Trace($"Internal error. Id for ProviderProduct not set in {item}", LogEventType.Error);
                throw new InvalidDataException($"Internal error. Id for ProviderProduct not set in {item}");
                }
              connection.Execute(sqlStatement, new { item.Id}, transaction);
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during bulk update InGame/InArchive field ProviderProduct to database",ex, LogEventType.Error);
            }
          }
        }
      }

    /// <summary>
    /// Reads all provider products from the database.
    /// </summary>
    /// <returns>List&lt;ProviderProductModel&gt;.</returns>
    public static List<ProviderProductModel> ReadAllProviderProductsFromDatabase()
      {
      using IDbConnection Db = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString());
      var output = Db.Query<ProviderProductModel>("SELECT * FROM ProviderProducts", new {});
      return output.ToList();
      }

    public static void UpdateListIdsFromDatabase(List<ProviderProductModel> providerProducts)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"SELECT Id FROM ProviderProducts WHERE Provider=@Provider AND Product=@Product;";
          try
            {
            foreach (var item in providerProducts)
              {
              if (item.Id < 1)
                {
                int idModel = connection.Query<int>(sqlStatement,
                  new {item.Provider, item.Product}, transaction).First();
                item.Id = idModel;
                }
              }
            transaction.Commit();
            }
          catch (Exception ex)
            {
            transaction.Rollback();
            Log.Trace($"Something went wrong during bulk getting id's for ProviderProduct from database",ex, LogEventType.Error);
            }
          }
        }
      }

    #endregion

    #region Copy
    public static void CopyAssets(List<ProviderProductModel> providerProductList, Boolean toGame, string gameAssetsFolder, string archiveAssetsFolder)
      {
      try
        {
        foreach (var providerProduct in providerProductList)
          {
          string gameProviderProductFolder =
            $"{gameAssetsFolder}{providerProduct.Provider}\\{providerProduct.Product}";
          string archiveProviderProductFolder =
            $"{archiveAssetsFolder}{providerProduct.Provider}\\{providerProduct.Product}";
          string DestinationDirectory;
          string SourceDirectory;

          if (toGame)
            {
            SourceDirectory = archiveProviderProductFolder;
            DestinationDirectory = gameProviderProductFolder;
            providerProduct.InGame = true;
            }
          else
            {
            SourceDirectory = gameProviderProductFolder;
            DestinationDirectory = archiveProviderProductFolder;
            providerProduct.InArchive = true;
            }
          // This is a copy function that updates all files and makes sure both folder structures are identical after performing the copy operation.
          FilesAndDirectories.CopyDir(SourceDirectory, DestinationDirectory, toGame);
          }
        }
      catch (Exception E)
        {
        Log.Trace("Failed to move assets ", E, LogEventType.Error);
        }
      // If it all works, update the status for the InGame field
      UpdateBulkStatus(providerProductList,"InGame");
      // UpdateArchiveLocationsInList(ProviderProduct, ToGame); // List update we will not do this, but leave it to the UI to get the update from the database.
      }
 
    #endregion

    #region Filters

    public static bool ProductsFilter(ProviderProductModel providerProduct, ProviderProductFilterModel filter)
      {
      // The filters always are set to true, so we select the item if nothing is set, once a filter has a non-default value, it must cause a match, else it fails.

      if (providerProduct == null|| filter==null)
        {
        return false;
        }

      var output = Converters.EvaluateLocationFilter(filter.InGameFilter, filter.InArchiveFilter,
        providerProduct.InGame, providerProduct.InArchive);

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.ProviderFilter, providerProduct.Provider);
        }
      else
        {
        return false;
        }

      if (output)
        {
        output = Converters.EvaluateTextFilter(filter.ProductFilter, providerProduct.Product);
        }
      return output;
      }


    public static List<ProviderProductModel> ApplyProductsFilter(List<ProviderProductModel> providerProducts,
      ProviderProductFilterModel filter)
      {
      return providerProducts.Where(p => ProductsFilter(p, filter)).ToList();
      }
    #endregion

    #region Helpers
 
    #endregion
    }
  }
