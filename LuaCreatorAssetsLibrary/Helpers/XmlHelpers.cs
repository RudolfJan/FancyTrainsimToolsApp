using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Assets.Library.Helpers
  {
  public class XmlHelpers
    {
    public static String GetLocalisedString(XElement Element, String Default = "")
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
      return String.Empty;
      }
    }
  }
