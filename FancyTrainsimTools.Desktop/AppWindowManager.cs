using Mvvm.Library;
using System.Windows;

namespace FancyTrainsimTools.Desktop
  {
  public class AppWindowManager : WindowManager
  {
    protected override Window EnsureWindow(object model, object view, bool isDialog)
    {
      Window window = base.EnsureWindow(model, view, isDialog);

      window.SizeToContent = SizeToContent.Manual;
      window.Width = 300;
      window.Height = 300;

      return window;
    }
  }
}
