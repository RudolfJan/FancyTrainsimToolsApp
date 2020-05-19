// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf  Heijink
// Created          : 01-24-2020
//
// Last Modified By : Rudolf  Heijink
// Last Modified On : 01-24-2020
// ***********************************************************************
// <summary></summary>
// ***********************************************************************
using System.Diagnostics;

namespace Assets.Library.Logic
  {
  /// <summary>
  /// Class BasicProgressModel.
  /// Usage in IProgress objects e.g. IProgress<BasicProgressModel></BasicProgressModel>
  /// </summary>
  public class BasicProgressModel
    {
    /// <summary>
    /// Gets or sets the description of the stuff progress is reported.
    /// </summary>
    /// <value>The description of the progress item.</value>
    public string Description { get; set; }
    /// <summary>
    /// Gets or sets the amount of work to do.
    /// </summary>
    /// <value>The amount of work to do.</value>
    public int AmountToDo { get; set; }
    /// <summary>
    /// Gets or sets the amount of work done.
    /// </summary>
    /// <value>The amount of work done.</value>
    public int AmountDone { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this task is completed.
    /// </summary>
    /// <value><c>true</c> if this task is done; otherwise, <c>false</c>.</value>
    public bool IsDone { get; set; }
    /// <summary>
    /// Gets or sets the text to show when the task is done.
    /// </summary>
    /// <value>The completion text  to show when the task is done.</value>
    public string CompletionText { get; set; }
    /// <summary>Gets or sets the watch.</summary>
    /// <value>  Stopwatch for performance monitoring</value>
    public Stopwatch watch { get; set; }= new Stopwatch();

    /// <summary>
    /// Gets the percentage completed as a string.
    /// </summary>
    /// <value>The percentage completed.</value>
    public string PercentageCompleted
      {
      get
        {
        if (AmountToDo > 0)
          {
          return $"{AmountDone * 100 / AmountToDo}%";
          }
        return $"{AmountDone}";
        }
      }
    }
  }
