using System;
using System.ComponentModel;
using System.Windows;

// https://www.tutorialspoint.com/mvvm/mvvm_hooking_up_viewmodel.htm
// Note this requires .Net Core and cannot be in a .Net Standard2.1 library

namespace FancyTrainsimTools.Desktop.VML
  {

  // retrieve the ViewModelLocator from the Application resources

  public class ViewModelLocator
    {
    public ViewModelLocator Instance
      {
      get
        {
        return Application.Current.Resources["Locator"] as ViewModelLocator;
        }
      }

    public static bool GetAutoHookedUpViewModel(DependencyObject obj)
      {
      return (bool) obj.GetValue(AutoHookedUpViewModelProperty);
      }

    public static void SetAutoHookedUpViewModel(DependencyObject obj, bool value)
      {
      obj.SetValue(AutoHookedUpViewModelProperty, value);
      }

    // Using a DependencyProperty as the backing store for AutoHookedUpViewModel. 
    //This enables animation, styling, binding, etc...

    public static readonly DependencyProperty AutoHookedUpViewModelProperty =
      DependencyProperty.RegisterAttached("AutoHookedUpViewModel",
        typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false,
          AutoHookedUpViewModelChanged));


    private static void AutoHookedUpViewModelChanged(DependencyObject d,
      DependencyPropertyChangedEventArgs e)
      {
      if (DesignerProperties.GetIsInDesignMode(d)) return;
      var viewType = d.GetType();
      string str = viewType.FullName;
      str = str.Replace(".Views.", ".ViewModels.");

      var viewTypeName = str;
      var viewModelTypeName = viewTypeName + "Model";
      var viewModelType = Type.GetType(viewModelTypeName);
      var viewModel = Activator.CreateInstance(viewModelType ?? throw new InvalidOperationException());
      ((FrameworkElement) d).DataContext = viewModel;
      }
    }
  }