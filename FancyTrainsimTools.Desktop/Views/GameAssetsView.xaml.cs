using System;
using System.Windows;

namespace FancyTrainsimToolsDesktop.Views
  {
  /// <summary>
  /// Interaction logic for GameAssetsView.xaml
  /// </summary>
  public partial class GameAssetsView
    {
    public GameAssetsView()
      {
      InitializeComponent();
      }
    private void OKButton_Click(Object Sender, RoutedEventArgs E)
      {
      Close();
      }
    }
  }
