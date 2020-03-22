using System;
using System.Windows.Data;

namespace Styles.Library.Converters
  {

  // See http://www.amazedsaint.com/2009/05/numtoboolconverter-simple.html
  /*
	 <Window.Resources >
        <local:NumberToBoolConverter x:Key="IntConverter"/>
    </Window.Resources>
	 
	 <CheckBox Name="chkBox" 
IsChecked="{Binding Path=IntProperty,Mode=TwoWay, 
Converter={StaticResource IntConverter}}"/>
	 */

  public class NumberToBoolConverter : IValueConverter
    {
    #region IValueConverter Members

    public Object Convert(Object Value, Type TargetType,
      Object Parameter, System.Globalization.CultureInfo Culture)
      {
      if (Value != null)
        {
        var Val = System.Convert.ToInt32(Value);
        return (Val != 0);
        }
      return null;
      }

    public Object ConvertBack(Object Value, Type TargetType,
      Object Parameter, System.Globalization.CultureInfo Culture)
      {
      if (Value != null)
        {
        var Val = System.Convert.ToBoolean(Value);
        return Val ? 1 : 0;
        }
      return null;
      }

    #endregion
    }


  }

