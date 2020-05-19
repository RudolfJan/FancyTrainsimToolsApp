// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf Heijink
// Created          : 12-30-2019
//
// Last Modified By : Rudolf Heijink
// Last Modified On : 01-18-2020
// ***********************************************************************
// <summary></summary>
// ***********************************************************************
#region UsingStatements
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using Logging.Library;
using System.Threading.Tasks;
using Assets.Library.Helpers;

#endregion

namespace Assets.Library.Logic
  {

  /// <summary>Class AssetDatabaseAccess.</summary>
  public class AssetDatabaseAccess
    {

    #region Properties

    /// <summary>
    /// Gets or sets the database path.
    /// </summary>
    /// <value>The database path.</value>
    public static string _databasePath;

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    private static string _connectionString; 


    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetDatabaseAccess"/> class.
    /// </summary>
    //public AssetDatabaseAccess(string connectionString, string databasePath)
    //  {
    //  _connectionString = connectionString;
    //  _databasePath = databasePath;
    //  TableFactory();
    //  }


    public static void InitDatabase(string connectionString, string databasePath)
      {
      _connectionString = connectionString;
       _databasePath = databasePath;
      TableFactory();
      }
    #endregion

    #region Methods

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <returns>String.</returns>
    public static String GetConnectionString()
      {
      return _connectionString;
      }
    /// <summary>
    /// Creates the database.
    /// </summary>
    protected static void CreateDatabase()
      {
      try
        {
        if (!File.Exists(_databasePath))
          {
          String Dir = Path.GetDirectoryName(_databasePath);
          if (Dir != null)
            {
            Directory.CreateDirectory(Dir);
            }
          SQLiteConnection.CreateFile(_databasePath);
          }
        }
      catch (Exception ex)
        {
        Log.Trace($"Exception during creating Asset database{_databasePath}",ex,LogEventType.Error);
        throw ex;
        }
      }

    /// <summary>
    /// Creates the table.
    /// </summary>
    /// <param name="command">The command.</param>
    protected static void CreateTable(String command)
      {
      try
        {
        String reader = File.ReadAllText(command);
        using IDbConnection DbConnection = new SQLiteConnection(_connectionString);
          {
          Int32 result = DbConnection.Execute(reader);
          }
        }
      catch (SQLiteException sqLiteException)
        {
        Log.Trace($"Exception during create database table command {command}",sqLiteException,LogEventType.Error);
        throw sqLiteException;
        }
      catch (Exception ex)
        {
        Log.Trace($"Exception during create database table command {command}",ex,LogEventType.Error);
        throw ex;
        }
      }

    /// <summary>
    /// Tables the factory.
    /// </summary>
    public static void TableFactory()
      {
      try
        {
        // Make sure database exists
        CreateDatabase();

        // TableCreation
        CreateTable("SQL\\CreateProviderProductTable.sql");
        CreateTable("SQL\\CreateAssetsTable.sql");

        CreateTable("SQL\\CreateRoutesTable.sql");
        CreateTable("SQL\\CreateRouteAssetsTable.sql");

        CreateTable("SQL\\CreateScenariosTable.sql");


        // ViewCreation
        CreateTable("SQL\\CreateBluePrintView.sql");
        CreateTable("SQL\\CreateFullRouteAssets.sql");
        CreateTable("SQL\\CreateFullRouteProviderProducts.sql");

        //Index creation
        CreateTable("SQL\\CreateAssetsIndex.sql");
        CreateTable("SQL\\CreateProviderProductIndex.sql");
        }
      catch (Exception e)
        {
        Log.Trace($"Exception during initialization of database",e,LogEventType.Error);
        throw;
        }
      }

    /// <summary>
    /// Clears the table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    public static void ClearTable(String tableName)
      {
      try
        {
        using IDbConnection DbConnection = new SQLiteConnection(GetConnectionString());
        DbConnection.Execute($"DELETE FROM {tableName}");
        }
      catch (Exception ex)
        {
        Log.Trace($"Cannot clear table {tableName}",ex,LogEventType.Error);
        throw ex;
        }
      }

    /// <summary>
    /// Loads the data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="sqlStatement">The SQL statement.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> LoadData<T, U>(string sqlStatement, U parameters, string connectionString)
      {
      try
        {
        using (IDbConnection connection = new SQLiteConnection(connectionString))
          {
          List<T> rows = connection.Query<T>(sqlStatement, parameters).ToList();
          return rows;
          }
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot execute query {sqlStatement}",e,LogEventType.Error);
        throw;
        }

      }

    public static async Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionString)
      {
      try
        {
        using (IDbConnection connection = new SQLiteConnection(connectionString))
          {
          var rows = await connection.QueryAsync<T>(sqlStatement, parameters);
          return await rows.ToListAsync();  // TODO find out why this is not working properly as extension method
          }
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot execute query {sqlStatement}",e,LogEventType.Error);
        throw;
        }

      }

    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement">The SQL statement.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionString">The connection string.</param>
    public static void SaveData<T>(string sqlStatement, T parameters, string connectionString)
      {
      try
        {
        using (IDbConnection connection = new SQLiteConnection(connectionString))
          {
          connection.Execute(sqlStatement, parameters);
          }
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot save data in database using {sqlStatement}",e,LogEventType.Error);
        throw;
        }
      }

    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement">The SQL statement.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionString">The connection string.</param>
    public static async Task SaveDataAsync<T>(string sqlStatement, T parameters, string connectionString)
      {
      try
        {
        using (IDbConnection connection = new SQLiteConnection(connectionString))
          {
          await connection.ExecuteAsync(sqlStatement, parameters);
          }
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot save data in database using {sqlStatement}",e,LogEventType.Error);
        throw;
        }
      }



    #endregion

    #region Helpers

    #endregion

    }
  }
