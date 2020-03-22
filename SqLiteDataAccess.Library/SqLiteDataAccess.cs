using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Logging.Library;

namespace SqLiteDataAccess.Library
  {
  public interface ISqLiteDataAccess
    {
    List<T> LoadData<T, U>(String sqlStatement, U parameters, String connectionStringName);
    void SaveData<T>(String sqlStatement, T parameters, String connectionStringName);
    }

  public class SqLiteDataAccess : ISqLiteDataAccess
    {
  private readonly IConfiguration _config;

      public SqLiteDataAccess(IConfiguration config)
        {
        _config = config;
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

    }

