﻿#region UsingStatements
using System;

#endregion

namespace Assets.Library.Models
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose:
  /// Created by: rudol
  /// Created on: 1/17/2020 10:08:28 PM 
  /// </summary>
  #endregion
  public class RouteModel
    {
    #region Properties

    public int Id { get; set; }
    public string RouteName { get; set; }
    public string RouteGuid { get; set; }
    public string Pack { get; set; }
    public bool IsPacked { get; set; }
    public bool InGame { get; set; }
    public bool IsValidInGame { get; set; }
    public bool InArchive { get; set; }
    public bool IsValidInArchive { get; set; }

    #endregion
 

    #region Methods

    #endregion

    #region Helpers
    public override string ToString()
      {
      return $"{RouteName}";
      }

    #endregion


    }
  }
