using System;
using System.Collections.Generic;
using Mvvm.Library;

namespace Logging.Library
  {
  public class LogCollectionManager : ILogCollectionManager
    {
    public List<LogEntryClass> LogEvents { get; set; } = new List<LogEntryClass>();

    public LogCollectionManager()
      {
      LogEventHandler.LogEvent += OnSaveLogEvent;
      }

    public void OnSaveLogEvent(Object Sender, LogEventArgs E)
      {
      LogEvents.Add(E.EntryClass);
      }
    }
  }
