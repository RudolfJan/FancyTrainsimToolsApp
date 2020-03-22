using System;
using System.Windows.Data;

// See https://stackoverflow.com/questions/34337755/convert-enum-to-string-inside-textblock-text

namespace Styles.Library.Converters
  {
  public class EnumToStringConverter : IValueConverter
    {
    public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
      {
      String EnumString;
      try
        {
        EnumString = Enum.GetName((value.GetType()), value);
        return EnumString;
        }
      catch
        {
        return String.Empty;
        }
      }

    // No need to implement converting back on a one-way binding 
    public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
      {
      throw new NotImplementedException();
      }
    }

  }
