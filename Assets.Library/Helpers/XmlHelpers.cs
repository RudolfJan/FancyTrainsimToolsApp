using System;
using System.Text;
using System.Xml;
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

    public static string NormalizeBlueprint(string bluePrint)
      {
      return bluePrint?.Replace('\\', '/').Replace(".bin", "").Replace(".xml", "");
      }

    // Saves an XDocument while preserving indentation and removing the Byte Order marker (BOM)
    // you need to use this because the DTG files do not support a byte order marker
    // but at the same time you like to preserve indentation and new lines.
    public static void Save(XDocument Doc, String Targetfile)
      {
      var Settings = new XmlWriterSettings
        {
        Encoding = new UTF8Encoding(false), // The false means, do not emit the BOM.
        Indent = true
        };
      using (XmlWriter W = XmlWriter.Create(Targetfile, Settings))
        {
        Doc.Save(W);
        }
      }
    }
  }
