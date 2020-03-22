#region UsingStatements

using System;
using System.Xml.Linq;

#endregion

namespace Assets.Library.Models
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose: Holds data about a blueprint file
  /// Created by: Rudolf Heijink
  /// Created on: 12/26/2019 3:05:05 PM 
  /// </summary>
  #endregion
  public class BluePrintModel
  {
  #region Properties

  public virtual AssetModel Asset { get; set; }
  public XDocument bluePrintDoc { get; set; }
  #endregion

  #region Constructors
  public BluePrintModel()
    {

    }

  #endregion

  #region Methods


  #endregion

  #region Helpers
  public override String ToString()
    {
    throw new NotImplementedException("You should implement ToString() in BluePrintModel");
    }


  #endregion


  }
}
