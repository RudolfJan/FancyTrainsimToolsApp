using System;

namespace Logging.Library
  {
  public class LogEventHandler
    {
    public delegate void CLogEvent(Object Sender, LogEventArgs E);
    public static event CLogEvent LogEvent;
    public static void OnLogEvent(LogEventArgs E)
      {
      LogEvent?.Invoke(null, E);
      }
    }
  }