using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Styles.Library.Converters
  {
  /// <summary>
  /// This converter does nothing except breaking the
  /// debugger into the convert method
  /// </summary>
  /// 

  /*
	 * 
	To use add following lines to the XAML code of the window you want to debug:

	<Window.Resources>
		<local:CDebugConverter x:Key="DebugConverter"/>
	</Window.Resources>

	On the object where you need a breakpoint, add the converter to the binding you need to check:

	<TextBlock Text="{Binding Title, ElementName=wnd, Converter={StaticResource DebugConverter}}" />

	
	 * */
  public class DebugConverter : IValueConverter
    {
    public Object Convert(Object Value, Type TargetType,
        Object Parameter, CultureInfo Culture)
      {
      Debugger.Break();
      return Value;
      }

    public Object ConvertBack(Object Value, Type TargetType,
        Object Parameter, CultureInfo Culture)
      {
      Debugger.Break();
      return Value;
      }
    }
  }
