using System;

namespace Logging.Library
  {
  public class LogEventArgs : EventArgs
    {
    public LogEntryClass EntryClass { get; set; }

    public LogEventArgs(LogEntryClass entryClass)
      {
      EntryClass = entryClass;
      }
    }
  }
