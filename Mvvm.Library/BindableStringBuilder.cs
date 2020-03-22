using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;


// https://stackoverflow.com/questions/413675/howto-bind-textbox-control-to-a-stringbuilder-instance

/*
 Use in viewmodel:
 public  BindableStringBuilder ErrorMessages { get; set; }
ErrorMessages.AppendLine("Missing Image: " + imagePath);

    In XAML:
<TextBox Text="{Binding ErrorMessages.Text, Mode=OneWay}"/>

Note that BindableStringBuilder must be initialized: public BindableStringBuilder ErrorMessages { get ; } = new BindableStringBuilder(); 
 
 */

namespace Mvvm.Library
  {
  public class BindableStringBuilder : Notifier
    {
    private readonly StringBuilder _builder = new StringBuilder();

    private EventHandler<EventArgs> TextChanged;

    public string Text
      {
      get { return _builder.ToString(); }
      }

    public int Count
      {
      get { return _builder.Length; }
      }

    public void Append(String text)
      {
      _builder.Append(text);
      TextChanged?.Invoke(this, null);
      RaisePropertyChanged(() => Text);
      }

    public void AppendLine(String text)
      {
      _builder.AppendLine(text);
      TextChanged?.Invoke(this, null);
      RaisePropertyChanged(() => Text);
      }

    public void Clear()
      {
      _builder.Clear();
      TextChanged?.Invoke(this, null);
      RaisePropertyChanged(() => Text);
      }
    }
  }