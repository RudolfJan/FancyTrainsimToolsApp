#region UsingStatements
using System;
using System.Collections.ObjectModel;

#endregion

namespace Assets.Library.Models
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose: implements list type of asset collections
  /// Created by: Rudolf Jan
  /// Created on: 12/27/2019 11:58:36 AM 
  /// </summary>
  #endregion
  public class AssetListCollectionModel: IAssetCollectionModel
    {
    #region Properties

    /// <summary>
    /// List holding Asset model and children
    /// </summary>
    public ObservableCollection<AssetModel> AssetList { get; set; } = new ObservableCollection<AssetModel>();

    #endregion

    #region Constructors
    public AssetListCollectionModel()
      {

      }

    #endregion

    #region Methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Boolean Create()
      {
      return false;
      }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public Boolean AddUnique(AssetModel asset)
      {
      return false;
      }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public Boolean Remove(AssetModel asset)
      {
      return false;
      }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="providerPattern"></param>
    /// <param name="productPattern"></param>
    /// <param name="bluePrintPattern"></param>
    /// <returns></returns>
    public IAssetCollectionModel Filter(String providerPattern, String productPattern,
      String bluePrintPattern)
      {
      return null;
      }
    #endregion

    #region Helpers
    public override String ToString()
      {
      throw new NotImplementedException("You should implement ToString() in AssetListCollectionModel");
      }


    #endregion

  
    }
  }
