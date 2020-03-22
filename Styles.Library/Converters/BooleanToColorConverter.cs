using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

// https://stackoverflow.com/questions/8533546/use-of-boolean-to-color-converter-in-xaml/8533821#8533821

namespace Styles.Library.Converters
  {
  [ValueConversion(typeof(Boolean), typeof(SolidColorBrush))]
  public class BooleanToColorConverter : IValueConverter
    {
    #region Implementation of IValueConverter

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">Boolean value controlling whether to apply color change</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">A CSV string on the format [ColorNameIfTrue;ColorNameIfFalse;OpacityNumber] may be provided for customization, default is [LimeGreen;Transparent;1.0].</param>
    /// <param name="culture"></param>
    /// <returns>A SolidColorBrush in the supplied or default colors depending on the state of value.</returns>
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
      {
      SolidColorBrush color;
      // Setting default values
      var colorIfTrue = Colors.LimeGreen;
      var colorIfFalse = Colors.Transparent;
      Double opacity = 1;
      // Parsing converter parameter
      if (parameter != null)
        {
        // Parameter format: [ColorNameIfTrue;ColorNameIfFalse;OpacityNumber]
        var ParameterString = parameter.ToString();
        if (!String.IsNullOrEmpty(ParameterString))
          {
          var parameters = ParameterString.Split(';');
          var count = parameters.Length;
          if (count > 0 && !String.IsNullOrEmpty(parameters[0]))
            {
            colorIfTrue = ColorFromName(parameters[0]);
            }
          if (count > 1 && !String.IsNullOrEmpty(parameters[1]))
            {
            colorIfFalse = ColorFromName(parameters[1]);
            }
          if (count > 2 && !String.IsNullOrEmpty(parameters[2]))
            {
            if (Double.TryParse(parameters[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out var dblTemp))
              opacity = dblTemp;
            }
          }
        }
      // Creating Color Brush
      if (value != null && (Boolean)value)
        {
        color = new SolidColorBrush(colorIfTrue) { Opacity = opacity };
        }
      else
        {
        color = new SolidColorBrush(colorIfFalse) { Opacity = opacity };
        }
      return color;
      }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
      {
      throw new NotImplementedException();
      }

    #endregion

    public static Color ColorFromName(String colorName)
      {
      System.Drawing.Color systemColor = System.Drawing.Color.FromName(colorName);
      return Color.FromArgb(systemColor.A, systemColor.R, systemColor.G, systemColor.B);
      }
    }
  }
