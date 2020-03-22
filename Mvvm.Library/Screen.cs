using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Mvvm.Library
  {
  public class Screen : IViewAware, INotifyPropertyChanged
    {
    public Window View { get; set; }

    public void Close(bool? result)
      {
      View.DialogResult = result;
      View.Close();
      }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }