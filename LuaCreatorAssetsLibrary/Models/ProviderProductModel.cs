using Assets.Library.Logic;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;

namespace Assets.Library.Models
  {
  /// <summary>
  /// Provider and product data model for asset management
  /// </summary>
  public class ProviderProductModel
    {
    #region Properties

    public int Id { get; set; }

    /// <summary>
    /// Provider
    /// </summary>
    public String Provider { get; set; } = string.Empty;
    /// <summary>
    /// Product
    /// </summary>
    public String Product { get; set; } = string.Empty;

    public String Pack { get; set; } = string.Empty;
    /// <summary>
    /// Combined output provider and product, slash as separator
    /// </summary>
    public String ProviderProduct
      {
      get
        {
        if (String.IsNullOrWhiteSpace(Pack))
          {
          return $"{Provider}/{Product}";
          }
        return $"{Provider}/{Product}: in {Pack}";
        }
      }

    public String ArchiveFileName
      {
      get
        {
        if (Pack.Length > 0)
          {
          return $"{Provider}\\{Product}\\{Pack}.ap";
          }
        else
          {
          return $"{Provider}\\{Product}\\";
          }
        }
      }

    #endregion


    #region Methods
    public Boolean Filter(String providerPattern, String productPattern)
      {
      throw new NotImplementedException();
      return true;
      }

    internal ProviderProductModel ToProviderProduct(String fullName, Int32 prefixLength)
      {
      String[] parts = fullName.Substring(prefixLength).Split('\\');
      if (parts.Length != 3)
        {
        throw new ArgumentOutOfRangeException($"Filename format not correct {fullName}");
        }
      var output= new ProviderProductModel()
        {
        Provider = parts[0],
        Product =  parts[1]
        };
      output.Pack = Path.GetFileNameWithoutExtension(parts[2]);
      return output;
      }

    public Int32 GetDatabaseRecordId()
      {
      try
        {
        using IDbConnection Db = new SQLiteConnection(AssetDatabaseAccess.GetConnectionString());
        var output = Db.Query<Int32>($"select Id from ProviderProduct  WHERE Provider='{Provider}' AND Product='{Product}'", new DynamicParameters());
        return output.First();
        }
      catch (Exception e)
        {
        Console.WriteLine(e);
        throw;
        }
      }
    #endregion

    }
  }
