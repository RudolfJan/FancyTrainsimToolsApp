using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

// Converter to apply percentage to a size
// https://stackoverflow.com/questions/8121906/resize-wpf-window-and-contents-depening-on-screen-resolution?noredirect=1&lq=1

/*
Usage example:

Height="{Binding Source={x:Static SystemParameters.MaximizedPrimaryScreenHeight}, Converter={tools:RatioConverter}, ConverterParameter='0.9' }" 
Width="{Binding Source={x:Static SystemParameters.MaximizedPrimaryScreenWidth}, Converter={tools:RatioConverter}, ConverterParameter='0.9' }" 
      Title="{Binding Path=DisplayName}"

Also possible: MaximizedPrimaryScreenHeight 
 */

namespace Styles.Library.Converters
  {
  [ValueConversion(typeof(String), typeof(String))]
  public class RatioConverter : MarkupExtension, IValueConverter
    {
    private static RatioConverter _instance;

    public Object Convert(Object Value, Type TargetType, Object Parameter, CultureInfo Culture)
      {
      // do not let the culture default to local to prevent variable outcomes are decimal syntax
      var size = System.Convert.ToDouble(Value) * System.Convert.ToDouble(Parameter, CultureInfo.InvariantCulture);
      return size.ToString("G0", CultureInfo.InvariantCulture);
      }

    public Object ConvertBack(Object Value, Type TargetType, Object Parameter, CultureInfo Culture)
      {
      // read only converter...
      throw new NotImplementedException();
      }

    public override Object ProvideValue(IServiceProvider ServiceProvider) => _instance ?? (_instance = new RatioConverter());
    }
  }