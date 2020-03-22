using System;
using System.Windows;
using System.Windows.Data;


// https://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum

/*
 * Binds radiobuttons to an enum
 * <Grid>
    <Grid.Resources>
      <local:CEnumBooleanConverter x:Key="EnumToBooleanConverter" />
    </Grid.Resources>
    <StackPanel >
      <RadioButton IsChecked="{Binding Path=VeryLovelyEnum, Converter={StaticResource enumBooleanConverter}, ConverterParameter=FirstSelection}">first selection</RadioButton>
      <RadioButton IsChecked="{Binding Path=VeryLovelyEnum, Converter={StaticResource enumBooleanConverter}, ConverterParameter=TheOtherSelection}">the other selection</RadioButton>
      <RadioButton IsChecked="{Binding Path=VeryLovelyEnum, Converter={StaticResource enumBooleanConverter}, ConverterParameter=YetAnotherOne}">yet another one</RadioButton>
    </StackPanel>
</Grid>
 * */



namespace Styles.Library.Converters
  {
  public class EnumToBooleanConverter : IValueConverter
    {
    public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
      {
      if (parameter != null)
        {
        if (!(parameter is String parameterString))
          return DependencyProperty.UnsetValue;

        if (value != null && Enum.IsDefined(value.GetType(), value) == false)
          return DependencyProperty.UnsetValue;

        if (value != null)
          {
          var parameterValue = Enum.Parse(value.GetType(), parameterString);
          return parameterValue.Equals(value);
          }
        }

      return null;
      }

    public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
      {
      if (!(parameter is String parameterString))
        return DependencyProperty.UnsetValue;

      return Enum.Parse(targetType, parameterString);
      }
    }
  }