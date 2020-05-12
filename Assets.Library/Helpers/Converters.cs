using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Assets.Library.Helpers
  {
  public static class Converters
    {

    #region Filters

    // https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard
    // Last solution in this article
    // If you want to implement both "*" and "?"

    /* Usage examples:
      String test = "Some Data X";

  Boolean endsWithEx = Regex.IsMatch(test, WildCardToRegular("*X"));
  Boolean startsWithS = Regex.IsMatch(test, WildCardToRegular("S*"));
  Boolean containsD = Regex.IsMatch(test, WildCardToRegular("*D*"));

  // Starts with S, ends with X, contains "me" and "a" (in that order) 
  Boolean complex = Regex.IsMatch(test, WildCardToRegular("S*me*a*X"));*/
    public static String WildCardToRegular(String value)
      {
      return $"^{Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*")}$";
      }

    //If this returns true, you did not deselect the object based on this partial result. 
    //if this returns false, you are definitely done.
    public static bool EvaluateTextFilter(string filter, string value)
      {
      var output = true;
      if (filter.Length == 0)
        {
        return true;
        }

      if (filter.Length > 0)
        {
        output = Regex.IsMatch(value, WildCardToRegular(filter));
        }
      return output;
      }

    public static bool EvaluatePackedLocationFilter(bool gameFilter, bool archiveFilter, bool isPackedFilter, bool inGame,
      bool inArchive, bool isPacked)
      {
      if (gameFilter == false && archiveFilter == false && isPackedFilter==false)
        {
        return true;
        }

      if (gameFilter == inGame && archiveFilter == inArchive && isPackedFilter==isPacked)
        {
        return true;
        }
      return false;
      }
    public static bool EvaluateLocationFilter(bool gameFilter, bool archiveFilter, bool inGame,
      bool inArchive)
      {
      if (gameFilter == false && archiveFilter == false)
        {
        return true;
        }

      if (gameFilter == inGame && archiveFilter == inArchive)
        {
        return true;
        }
      return false;
      }



    #endregion

    #region FormatConverters
    public static string LocationToString(bool InGame, bool InArchive)
      {
      string output;
      if (InGame && InArchive)
        {
        output = "Both (ERROR should not happen!";
        return output;
        }
      if(!InGame&& !InArchive)
        {
        output = "None (ERROR should not happen!";
        return output;
        }

      if (InGame)
        {
        output = "InGame";
        }
      else
        {
        output = "InArchive";
        }
      return output;
      }

    // Convert integer time in seconds to h:mm:ss formatted string
    public static string TimeToString(int time)
      {
      var hours = time / 3600;
      var minutes = (time - hours * 3600) / 60;
      var seconds = time - hours * 3600 - minutes * 60;
      return $"{hours,1:d2}:{minutes,1:d2}:{seconds,1:d2}";
      }


#endregion

    public static string GetLocalisedString(XElement Element, string Default = "")
      {
      if (Element == null)
        {
        return Default;
        }

      foreach (var Children in Element.Elements())
        {
        var Value = Children.Value;
        if (Value.Length > 0 && Children.Name != "Key")
          {
          return Value;
          }
        }

      return string.Empty;
      }

    #region Localisation
    // Replace a localized text for all supported languages. Currently, localization is not supported (and probably never will be supported.
    public static void SetLocalisedText(XElement node, string text)
      {
      foreach (XElement subNode in node.Elements())
        {
        if (!(string.CompareOrdinal(subNode.Name.ToString(), "Other") == 0 ||
              string.CompareOrdinal(subNode.Name.ToString(), "Key") == 0))
          {
          subNode.Value = text;
          continue;
          }

        if (string.CompareOrdinal(subNode.Name.ToString(), "Key") == 0)
          {
          subNode.Value = Guid.NewGuid().ToString();
          }
        }
      }

#endregion







    }
  }
