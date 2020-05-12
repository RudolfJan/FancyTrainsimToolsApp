using System;
using System.Collections.Generic;

namespace Logging.Library
  {
  public static class LogCollectionManager
    {
    public static List<LogEntryClass> LogEvents { get; set; } = new List<LogEntryClass>();
    static LogCollectionManager()
      {
      LogEventHandler.LogEvent += OnSaveLogEvent;
      }

    public static void OnSaveLogEvent(Object Sender, LogEventArgs E)
      {
      LogEvents.Add(E.EntryClass);
      }
    }
  }
