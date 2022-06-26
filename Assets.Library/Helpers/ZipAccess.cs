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

		public static List<string> GetAllZipEntries(String assetBasePath, String archiveFileName)
			{
			throw new NotImplementedException();
			}



		#endregion

		}
  }
