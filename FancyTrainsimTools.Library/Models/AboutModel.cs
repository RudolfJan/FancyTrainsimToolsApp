using System;
using System.Collections.Generic;
using System.Text;

namespace FancyTrainsimTools.Library.Models
  {
  public class AboutModel
    {
    public static string Company { get; } = "Holland Hiking";
    public static string Product { get; } = "Fancy Trainsim Tools";
    public static string Copyright { get; } = "(C) 2015-2020 Rudolf Heijink, Netherlands";

    public static string Description { get; } =
      "Toolkit covering (almost) the complete lifecycle of a scenario";
    public static string Author { get; }="Rudolf Heijink";
    public static string Version{ get; } ="1.0";
    public static Uri DownloadLocation { get; } = new Uri("https://www.hollandhiking.nl/trainsimulator");
    }
  }
