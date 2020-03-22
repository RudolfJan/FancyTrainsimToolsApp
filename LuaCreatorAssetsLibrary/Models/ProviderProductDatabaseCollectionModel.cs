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

#endregion

namespace Assets.Library.Models
  {
 
  public class ProviderProductDatabaseCollectionModel
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
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderProductDatabaseCollectionModel"/> class.
    /// </summary>
    public ProviderProductDatabaseCollectionModel()
      {

      }

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
          string sqlStatement = @$"INSERT OR IGNORE INTO ProviderProduct (Provider, Product, Pack) VALUES (@Provider, @Product, @Pack)";
          try
            {
            foreach (var item in providerProducts)
              {
              connection.Execute(sqlStatement, new { item.Provider, item.Product, item.Pack}, transaction);
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

    /// <summary>
    /// Reads the provider product from directory.
    /// </summary>
    /// <param name="productDirectory">The product directory.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <returns>ProviderProductModel.</returns>
    public ProviderProductModel ReadProviderProductFromDirectory(DirectoryInfo productDirectory, string providerName)
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
    public List<ProviderProductModel> ReadProviderProductListFromDirectory(
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
    public void SaveAllProviderProducts()
      {
      try
        {
        DirectoryInfo assetDirectory = new DirectoryInfo(AssetBasePath);
        List<ProviderProductModel> providerProductList =
          ReadProviderProductListFromDirectory(assetDirectory);
        SaveProviderProductsBulk(providerProductList);
        }
      catch (Exception ex)
        {
        Log.Trace($"Something went wrong during saving ProviderProduct to database",ex,LogEventType.Error);
        }
      }

    /// <summary>
    /// Reads all provider products from the database.
    /// </summary>
    /// <returns>List&lt;ProviderProductModel&gt;.</returns>
    public static List<ProviderProductModel> ReadAllProviderProductsFromDatabase()
      {
      using IDbConnection Db = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString());
      var output = Db.Query<ProviderProductModel>("SELECT * FROM ProviderProduct", new {});
      return output.ToList();
      }

    public static void UpdateListIdsFromDatabase(List<ProviderProductModel> providerProducts)
      {
      using (IDbConnection connection = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
        {
        connection.Open();
        using (IDbTransaction transaction = connection.BeginTransaction())
          {
          string sqlStatement = @$"SELECT Id FROM ProviderProduct WHERE Provider=@Provider AND Product=@Product;";
          try
            {
            foreach (var item in providerProducts)
              {
              IdModel idModel=connection.Query<IdModel>(sqlStatement, new { item.Provider, item.Product}, transaction).First();
              item.Id = idModel.Id;
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

    #region Helpers
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    /// <exception cref="NotImplementedException">You should implement ToString() in ProviderProductDatabaseCollectionModel</exception>
    public override String ToString()
      {
      throw new NotImplementedException("You should implement ToString() in ProviderProductDatabaseCollectionModel");
      }



    #endregion
    }
  }
