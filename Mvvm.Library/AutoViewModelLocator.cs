using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Mvvm.Library {
    public static class AutoViewModelLocator {
        public static bool GetAutoAttachViewModel(DependencyObject obj) {
            return (bool) obj.GetValue(AutoAttachViewModelProperty);
        }

        public static void SetAutoAttachViewModel(DependencyObject obj, bool value) {
            obj.SetValue(AutoAttachViewModelProperty, value);
        }

        public static readonly DependencyProperty AutoAttachViewModelProperty =
            DependencyProperty.RegisterAttached("AutoAttachViewModel",
                typeof (bool), typeof (AutoViewModelLocator),
                new PropertyMetadata(false, AutoAttachViewModelChanged));

        private static void AutoAttachViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(d)) {
                return;
            }
            var viewType = d.GetType();
            if (viewType.FullName != null)
                {
                var viewModelTypeName = viewType.FullName.Replace("View", "ViewModel");
                var assembliesForSearchingIn = AssemblySource.Instance;

                var allExportedTypes = new List<Type>();
                foreach (var assembly in assembliesForSearchingIn) {
                    //CAN BE CACHED
                    allExportedTypes.AddRange(assembly.GetExportedTypes());
                    }
                var viewModelType = allExportedTypes.Single(x => x.FullName == viewModelTypeName);
                var viewModel = IoC.GetInstance(viewModelType, null);
                ((FrameworkElement) d).DataContext = viewModel;
                }
            }
    }
}