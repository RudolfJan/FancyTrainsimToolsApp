using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Library.Models
  {
  /// <summary>
  /// A collection of unique provider/products
  /// </summary>
  public interface IProviderProductCollectionModel
    {
    /// <summary>
    /// Setup method e.g. to create a database, build a list, create test data etcetera ...
    /// </summary>
    /// <returns></returns>
    Boolean Create();
    /// <summary>
    /// Adds a ProviderProductModel object to the collection. Each one must be unique for the tuple provider and product
    /// </summary>
    /// <param name="providerProduct"></param>
    /// <returns>true if added, false if not added</returns>
    Boolean AddUnique(ProviderProductModel providerProduct);
    /// <summary>
    /// Remove a ProviderProductModel from the collection
    /// </summary>
    /// <param name="providerProduct"></param>
    /// <returns>true if removed, fase if not removed</returns>
    Boolean Remove(ProviderProductModel providerProduct);
    /// <summary>
    /// Apply a filter over the collection and return a filtered collection
    /// </summary>
    /// <param name="providerPattern"></param>
    /// <param name="productPattern"></param>
    /// <returns>Filtered collection</returns>
    IProviderProductCollectionModel Filter(String providerPattern, String productPattern);
    }
  }
