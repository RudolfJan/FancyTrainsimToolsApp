using Assets.Library.Models;
using System;
using System.Windows;

namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for RouteAssetsView.xaml
  /// </summary>
  public partial class RouteAssetsView
    {
    public RouteModel Route;
    public RouteAssetsView()
      {
      InitializeComponent();
      }

    private void OKButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Close();
      }
    }
  }
