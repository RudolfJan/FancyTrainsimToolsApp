using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Mvvm.Library
  {
  public interface IWindowManager
    {
    bool? ShowDialog(object rootModel);
    void ShowWindow(object rootModel);
    }

  public class WindowManager : IWindowManager
    {
    public bool? ShowDialog(object rootModel)
      {
      return CreateWindow(rootModel, true).ShowDialog();
      }

    public void ShowWindow(object rootModel)
      {
      CreateWindow(rootModel, false).Show();
      }

    protected virtual Window CreateWindow(object rootModel, bool isDialog)
      {
      Window window = LocateWindow(rootModel);
      var view = EnsureWindow(rootModel, window, isDialog);

      return view;
      }

    private Window LocateWindow(object rootModel)
      {
      Type viewModelType = rootModel.GetType();
      if (viewModelType.FullName != null)
        {
        string viewFullName = viewModelType.FullName.Replace("ViewModel", "View");
        var assembliesForSearchingIn = AssemblySource.Instance;

        var allExportedTypes = new List<Type>();
        foreach (var assembly in assembliesForSearchingIn)
          {
          allExportedTypes.AddRange(assembly.GetExportedTypes());
          }
        Type viewType = allExportedTypes.FirstOrDefault(x => x.FullName == viewFullName) == null
          ? ViewModelMappings.Mappings.First(x => x.Key == viewModelType).Value
          : null;
        object view = Activator.CreateInstance(viewType);
        var window = (Window)view;
        window.DataContext = rootModel;

        var viewAware = rootModel as IViewAware;
        if (viewAware != null)
          {
          viewAware.View = window;
          }

        return window;
        }
      return null;
      }

    protected virtual Window EnsureWindow(object model, object view, bool isDialog)
      {
      var window = view as Window;

      if (window == null)
        {
        window = new Window
          {
          Content = view,
          SizeToContent = SizeToContent.WidthAndHeight
          };
        var owner = InferOwnerOf(window);
        if (owner != null)
          {
          window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          window.Owner = owner;
          }
        else
          {
          window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
          }
        }
      else
        {
        var owner = InferOwnerOf(window);
        if (owner != null && isDialog)
          {
          window.Owner = owner;
          }
        }

      return window;
      }

    protected virtual Window InferOwnerOf(Window window)
      {
      if (Application.Current == null)
        {
        return null;
        }

      var active = Application.Current.Windows
          .OfType<Window>()
          .FirstOrDefault(x => x.IsActive);
      active = active ?? Application.Current.MainWindow;
      return active == window ? null : active;
      }
    }
  }