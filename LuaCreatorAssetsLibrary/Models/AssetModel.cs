﻿#region UsingStatements
using System;
using System.IO;

#endregion

namespace Assets.Library.Models
{
#region AboutThisFile
/// <summary>
/// Purpose: Holds asset data for assets library
/// Created by: Rudolf Heijink
/// Created on: 12/26/2019 10:15:35 AM 
/// </summary>
#endregion
    public class AssetModel
  {

  #region Enums

  /// <summary>
  /// enum representing the file type for a blueprintpath
  /// </summary>
  public enum AssetFileType
    {
    none,
    bin,
    xml
    }
  #endregion

  #region Properties

  public int Id { get; set; }

  //TODO set BlueprintPath as normalized
  /// <summary>
  /// Normalized BluePrint path, without extension
  /// </summary>
  public String BluePrintPath { get; set; }
 
  /// <summary>
  /// References ProviderProduct part of the complete asset
  /// </summary>
  public ProviderProductModel ProviderProduct { get; set; } = new ProviderProductModel();
  /// <summary>
  /// string property representing asset (only get)
  /// </summary>
  public String AssetPath
    {
    get { return $"{ProviderProduct}/{BluePrintPath}"; }
    }

    #endregion

    #region Constructors
    /// <summary>Initializes a new instance of the <see cref="AssetModel"/> class.</summary>
    public AssetModel()
    {
    }

  #endregion

  #region Methods

  /// <summary>
  /// Returns if asset matches a simple filter (*, ? are recognized, not the full regex
  /// </summary>
  /// <param name="providerPattern">Filter pattern for Provider</param>
  /// <param name="productPattern">Filter pattern for Product</param>
  /// <param name="bluePrintPathPattern">Filter pattern for blueprint</param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public Boolean Filter(String providerPattern, String productPattern, String bluePrintPathPattern)
    {
    throw new NotImplementedException();
    }
    #endregion

    #region Helpers
    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public override String ToString()
    {
    throw new NotImplementedException();
    }
  #endregion
  }

#region ExensionMethods

public static class AssetModelExtensions
  {
  /// <summary>
  /// modifies AssetPath applying forward instead of back slashes
  /// </summary>
  /// <returns>converted string</returns>
  public static String ConvertToForwardSlashes(this String assetPath)
    {
    return assetPath.Replace("\\", "/");;
    }

  /// <summary>
  /// modifies AssetPath applying backslashes instead of forward slashes
  /// </summary>
  /// <returns>converted string</returns>
  public static  String ConvertToBackSlashes(this String assetPath)
    {
    return assetPath.Replace("/", "\\");;
    }

  /// <summary>
  /// Appends the appropriate file type to an asset
  /// </summary>
  /// <param name="assetPath">input string</param>
  /// <param name="fileType">output</param>
  /// <returns></returns>
  public static String AppendFileType(this String assetPath, AssetModel.AssetFileType fileType=AssetModel.AssetFileType.none)
    {
    String output = assetPath;
    if (fileType != AssetModel.AssetFileType.none)
      {
      output += $".{fileType}";
      }
    return output;
    }

  /// <summary>
  /// Remove the file type from the asset 
  /// </summary>
  /// <param name="assetPath"></param>
  /// <returns></returns>
  public static String RemoveFileType(this String assetPath)
    {
    String output = Path.GetFileNameWithoutExtension(assetPath);
    return output;
    }
  }


#endregion
}
