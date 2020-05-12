#region UsingStatements

using Assets.Library.Logic;
using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;

#endregion

namespace Assets.Library.Logic
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose: Give access to ZIP file contents
  /// Created by: Rudolf Heijink
  /// Created on: 1/12/2020 1:20:53 PM 
  /// </summary>
  #endregion
  public class ZipAccess
    {

    #region Properties

    #endregion

    #region Constructors
    public ZipAccess()
      {

      }

    #endregion

    #region Methods

    public static List<ProviderProductModel> GetPackedProviderProducts(
      DirectoryInfo baseDirectory, String pattern = "*.ap")
      {
      var output = new List<ProviderProductModel>();
      var zipFiles=baseDirectory.GetFiles(pattern, SearchOption.AllDirectories);
      foreach(var file in zipFiles)
        {
        output.Add(new ProviderProductModel().ToProviderProduct(file.FullName, baseDirectory.FullName.Length));
        }
      return output;
      }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="basePath">Directory path, normally assets library directory, should end with backslash</param>
    /// <param name="archiveFileName">Archive file name, normally provider, product and pack (including .ap)</param>
    /// <param name="endsWith">Extension to look for</param>
    /// <returns></returns>
    public static List<string> GetAllZipEntries(String basePath, 
                                    String archiveFileName,
                                    String endsWith = ".bin")
      {
      var archiveFullName = $"{basePath}{archiveFileName}";
      using var archive = ZipFile.OpenRead( archiveFullName);
          {
          var entries = archive.Entries;
          List<string> output = entries.Select(x=> x.FullName).Where(x=>x.EndsWith(endsWith)).ToList();
          return output;
          }
      }







    #endregion

    #region Helpers
    public override String ToString()
      {
      throw new NotImplementedException("You should implement ToString() in ZipAccess");
      }


    #endregion


    }
  }
