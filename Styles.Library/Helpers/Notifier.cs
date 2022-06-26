using System;
using System.ComponentModel;

namespace Styles.Library.Helpers
  {
  public class Notifier : INotifyPropertyChanged

    {
    public event PropertyChangedEventHandler
      PropertyChanged;

    protected void OnPropertyChanged(String PropertyName)
      {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
      }
    }
  }
