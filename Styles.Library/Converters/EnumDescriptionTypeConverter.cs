using System;
using System.ComponentModel;
using System.Reflection;

// http://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/

namespace Styles.Library.Converters
  {
  public class EnumDescriptionTypeConverter : EnumConverter
    {
    public EnumDescriptionTypeConverter(Type Type)
        : base(Type)
      {
      }

    public override Object ConvertTo(ITypeDescriptorContext Context, System.Globalization.CultureInfo Culture, Object Value, Type DestinationType)
      {
      if (DestinationType == typeof(String))
        {
        if (Value != null)
          {
          FieldInfo Fi = Value.GetType().GetField(Value.ToString());
          if (Fi != null)
            {
            var Attributes = (DescriptionAttribute[])Fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((Attributes.Length > 0) && (!string.IsNullOrEmpty(Attributes[0].Description))) ? Attributes[0].Description : Value.ToString();
            }
          }

        return string.Empty;
        }

      return base.ConvertTo(Context, Culture, Value, DestinationType);
      }
    }
  }
