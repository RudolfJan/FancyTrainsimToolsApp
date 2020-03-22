using Logging.Library;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for ShellView.xaml
  /// </summary>
  public partial class ShellView
    {
    private readonly IServiceProvider _serviceProvider;
    public ShellView()
      {
      InitializeComponent();
      _serviceProvider = App.serviceProvider;
      string p = Settings.DataPath;
      }
 
    private void AboutButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Log.Trace("Called aboutView");
      var aboutView = (AboutView) _serviceProvider.GetService(typeof(AboutView));
      aboutView.Show();
      }

    private void LogViewerButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Log.Trace("Called loggingView");
      var loggingView = (LoggingView) _serviceProvider.GetService(typeof(LoggingView));
      loggingView.Show();
      }

    private void OKButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Close();
      }

    private void TrainSimManualsButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Log.Trace("Called Manuals tool");
      var manualsView = (ManualsView) _serviceProvider.GetService(typeof(ManualsView));
      manualsView.Show();
      }
    }
  }
