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
    public static String DatabasePath { get; set; } = ".\\AssetDatabase.db";
    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    private static String ConnectionString
      {
      get {return  $"Data Source={DatabasePath};Version=3;";}
      }

    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetDatabaseAccess"/> class.
    /// </summary>
    public AssetDatabaseAccess()
      {
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
      return ConnectionString;
      }
    /// <summary>
    /// Creates the database.
    /// </summary>
    protected void CreateDatabase()
      {
      try
        {
        if (!File.Exists(DatabasePath))
          {
          String Dir = Path.GetDirectoryName(DatabasePath);
          if (Dir != null)
            {
            Directory.CreateDirectory(Dir);
            }
          SQLiteConnection.CreateFile(DatabasePath);
          }
        }
      catch (Exception ex)
        {
        Log.Trace($"Exception during creating Asset database{DatabasePath}",ex,LogEventType.Error);
        throw ex;
        }
      }

    /// <summary>
    /// Creates the table.
    /// </summary>
    /// <param name="command">The command.</param>
    protected void CreateTable(String command)
      {
      try
        {
        String reader = File.ReadAllText(command);
        using IDbConnection DbConnection = new SQLiteConnection(ConnectionString);
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
    public void TableFactory()
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

        // ViewCreation
        CreateTable("SQL\\CreateBluePrintView.sql");
        CreateTable("SQL\\CreateRouteAssetsView.sql");

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
    public List<T> LoadData<T, U>(string sqlStatement, U parameters, string connectionString)
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

    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement">The SQL statement.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionString">The connection string.</param>
    public void SaveData<T>(string sqlStatement, T parameters, string connectionString)
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


    #endregion

    #region Helpers
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    /// <exception cref="System.NotImplementedException">You should implement ToString() in AssetDatabaseAccess</exception>
    public override String ToString()
      {
      throw new NotImplementedException("You should implement ToString() in AssetDatabaseAccess");
      }
    #endregion

    }
  }
