// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf Heijink
// Created          : 01-18-2020
//
// Last Modified By : Rudolf Heijink
// Last Modified On : 01-18-2020
// ***********************************************************************
// <summary></summary>
// ***********************************************************************
using System;

namespace Assets.Library.Helpers
  {
  /// <summary>
  /// Class ProgressPercentage.
  /// </summary>
  public class ProgressPercentage
    {
    /// <summary>
    /// Gets or sets the total work.
    /// </summary>
    /// <value>The total work.</value>
    public Int32 TotalWork { get; set; }
    /// <summary>
    /// Gets or sets the current progress.
    /// </summary>
    /// <value>The current progress.</value>
    public Int32 CurrentProgress { get; set; }
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public String Message { get; set; }

    /// <summary>
    /// Reports the progress.
    /// </summary>
    /// <returns>String.</returns>
    public String ReportProgress()
      {
      return $"{Message}: {100 * CurrentProgress / TotalWork}";
      }
    }
  }
