﻿using System;
using System.Windows;

namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for ManualsView.xaml
  /// </summary>
  public partial class ManualsView
    {
    public ManualsView()
      {
      InitializeComponent();
      }

   private void OKButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Close();
      }
    }
  }
