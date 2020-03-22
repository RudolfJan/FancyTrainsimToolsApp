using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Mvvm.Library

  {
  public class Notifier : INotifyPropertyChanged

    {
    public event PropertyChangedEventHandler
      PropertyChanged;

    public void OnPropertyChanged(string propertyName)
      {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

    public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
      {
      if (propertyExpression == null)
        {
        return;
        }

      var handler = PropertyChanged;

      if (handler != null)
        {
        if (propertyExpression.Body is MemberExpression body)
          handler(this, new PropertyChangedEventArgs(body.Member.Name));
        }
      }
    }
  }

