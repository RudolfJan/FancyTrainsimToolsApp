using System;
using System.Text.RegularExpressions;

namespace Assets.Library.Helpers
  {
  public static class Converters
    {
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
    }
  }
