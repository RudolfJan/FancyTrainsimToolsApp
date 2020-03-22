using System;
using System.Windows;

// https://stackoverflow.com/questions/7174315/understanding-wpf-deriving-window-class

// Not used here but interesting:
//  https://www.source-weave.com/blog/custom-wpf-window

namespace UISupport.Library.WindowSizing
  {

  // Usage: need to handle the Loaded event and eventually the ChangedWindowState event
  public class SizableWindow : Window
    {
    public static void InitWindow(Window MyWindow, UInt32 WindowMargin = 0)
      {
      // Do not try to change this in these two states, but you need to handle WindStateChanged event
      if (MyWindow.WindowState == WindowState.Minimized ||
          MyWindow.WindowState == WindowState.Maximized)
        {
        return;
        }

      var MaxWinHeight = SystemParameters.MaximizedPrimaryScreenHeight;
      var MaxWinWidth = SystemParameters.MaximizedPrimaryScreenWidth;
      var ActualWindowHeight = MyWindow.DesiredSize.Height;
      var ActualWindowWidth = MyWindow.DesiredSize.Width;
      if (ActualWindowHeight + MyWindow.Top > MaxWinHeight)
        {
        MyWindow.SizeToContent = SizeToContent.Manual;
        MyWindow.Top = WindowMargin;
        MyWindow.MaxHeight = MaxWinHeight - WindowMargin * 2; // leave a little bit space
        }

      if (ActualWindowWidth - MyWindow.Left > MaxWinWidth)
        {
        MyWindow.SizeToContent = SizeToContent.Manual;
        MyWindow.Left = WindowMargin;
        MyWindow.MaxWidth = MaxWinWidth - WindowMargin * 2;
        }
      }
    }
  }
