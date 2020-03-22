using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

// See https://stackoverflow.com/questions/3985876/wpf-binding-a-listbox-to-an-enum-displaying-the-description-attribute
// Need description attributes to enum values

namespace Styles.Library.Converters
  {
  public class EnumDescriptionConverter : IValueConverter
    {
    private String GetEnumDescription(Enum EnumObj)
      {
      FieldInfo MyFieldInfo = EnumObj.GetType().GetField(EnumObj.ToString());

      Object[] AttribArray = MyFieldInfo.GetCustomAttributes(false);

      if (AttribArray.Length == 0)
        {
        return EnumObj.ToString();
        }
      else
        {
        return (AttribArray[0] as DescriptionAttribute)?.Description;
        }
      }

    Object IValueConverter.Convert(Object Value, Type TargetType, Object Parameter, CultureInfo Culture)
      {
      Enum MyEnum = (Enum)Value;
      return GetEnumDescription(MyEnum);
      }

    Object IValueConverter.ConvertBack(Object Value, Type TargetType, Object Parameter, CultureInfo Culture)
      {
      return String.Empty;
      }
    }
  }
