using System;
using System.Diagnostics;
using System.Windows;

namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for AboutView.xaml
  /// </summary>
  public partial class AboutView
    {
    public AboutView()
      {
      InitializeComponent();
      }
    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
      {
      // You need a workaround here for .Net Core:
     //  https://github.com/dotnet/runtime/issues/28005
      var psi = new ProcessStartInfo
        {
        FileName = e.Uri.AbsoluteUri,
        UseShellExecute = true
        };
      Process.Start (psi);
      }
    }
  }
