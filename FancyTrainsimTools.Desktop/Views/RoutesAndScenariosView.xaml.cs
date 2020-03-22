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
  /// Interaction logic for RoutesAndScenariosView.xaml
  /// </summary>
  public partial class RoutesAndScenariosView
    {
    public RoutesAndScenariosView()
      {
      InitializeComponent();
      }

    private void OKButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Close();
      }
    }
  }
