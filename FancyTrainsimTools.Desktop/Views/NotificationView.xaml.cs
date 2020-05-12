using System;
using System.Windows;
using System.Windows.Threading;

// TODO bring this in MVVM structures

namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for NotificationView.xaml
  /// </summary>
  public partial class NotificationView
    {
    public string Message { get; set; }

    public NotificationView(string message)
      {
      InitializeComponent();
      var width = SystemParameters.PrimaryScreenWidth;
      Left = width / 2 - Left / 2;
      Message = message;
      if (string.IsNullOrEmpty(Message))
        {
        Message = "Test message";
        }

      DataContext = Message;
      StartCloseTimer();
      }

    private void StartCloseTimer()
      {
      DispatcherTimer timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromSeconds(4d);
      timer.Tick += TimerTick;
      timer.Start();
      }

    private void TimerTick(object sender, EventArgs e)
      {
      DispatcherTimer timer = (DispatcherTimer)sender;
      timer.Stop();
      timer.Tick -= TimerTick;
      Close();
      }
    }
  }
