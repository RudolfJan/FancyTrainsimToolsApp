#region UsingStatements

using Assets.Library.Logic;
using Assets.Library.Models;
using Logging.Library;
using System;
using System.Data.Common;
using System.Dynamic;

#endregion

namespace Assets.Library.Models
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose:
  /// Created by: rudol
  /// Created on: 1/17/2020 10:11:57 PM 
  /// </summary>
  #endregion
  public class RouteAssetsModel
    {


    #region Properties

    public RouteModel Route { get; set; }
    public AssetModel Asset { get; set; }

    #endregion

    #region Constructors
  
    #endregion

    #region Methods

    #endregion

    #region Helpers
    public override string ToString()
      {
      if (Route == null || Asset == null)
        {
        Log.Trace("Incomplete RouteAssetsModel detected, not shown");
        return "Incomplete RouteAssetsModel";
        }
      return
        $"{Route.RouteName}:{Asset.ProviderProduct.Provider}/{Asset.ProviderProduct.Product}/{Asset.BluePrintPath}";
      }
    #endregion


    }
  }
