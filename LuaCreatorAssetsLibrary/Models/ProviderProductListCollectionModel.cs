#region UsingStatements
using System;
using System.Collections.ObjectModel;

#endregion

namespace Assets.Library.Models
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose: Holds data fro a provider/product collection
  /// Created by: Rudolf Heijink
  /// Created on: 12/26/2019 5:13:34 PM 
  /// </summary>
  #endregion
  public class ProviderProductListCollectionModel: IProviderProductCollectionModel
    {

    #region Properties

    public ObservableCollection<ProviderProductModel> ProviderProductList { get; set; }= new ObservableCollection<ProviderProductModel>();

    #endregion

    #region Constructors
    public ProviderProductListCollectionModel()
      {

      }

    #endregion

    #region Methods

    public Boolean Create()
      {
      throw new NotImplementedException();
      return false;
      }

    public Boolean AddUnique(ProviderProductModel providerProduct)
      {
      throw new NotImplementedException();
      return false;
      }

    public Boolean Remove(ProviderProductModel providerProduct)
      {
      throw new NotImplementedException();
      return false;
      }

    public IProviderProductCollectionModel Filter(String providerPattern, String productPattern)
      {
      throw new NotImplementedException();
      return null;
      }


    #endregion

    #region Helpers
    public override String ToString()
      {
      throw new NotImplementedException("You should implement ToString() in ProviderProductListCollectionModel");
      }


    #endregion

 
    }
  }
