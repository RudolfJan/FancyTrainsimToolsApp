using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Library.Models
  {
  /// <summary>
  /// Interface for Asset collections
  /// </summary>
  public interface IAssetCollectionModel
    {
    /// <summary>
    /// Setup method e.g. to create a database, build a list, create test data etcetera ...
    /// </summary>
    /// <returns></returns>
    Boolean Create();
    /// <summary>
    /// Adds aa AssetModel object to the collection. Each one must be unique for the tuple provider, product and bluePrint
    /// </summary>
    /// <param name="asset"></param>
    /// <returns>true if added, false if not added</returns>
    Boolean AddUnique(AssetModel asset);
    /// <summary>
    /// Remove an AssetModel from the collection
    /// </summary>
    /// <param name="asset"></param>
    /// <returns>true if removed, false if not removed</returns>
    Boolean Remove(AssetModel asset);

    /// <summary>
    /// Apply a filter over the collection and return a filtered collection
    /// </summary>
    /// <param name="providerPattern"></param>
    /// <param name="productPattern"></param>
    /// <param name="bluePrintPattern"></param>
    /// <returns>Filtered collection</returns>
    IAssetCollectionModel Filter(String providerPattern, String productPattern, String bluePrintPattern);
    }
  }
