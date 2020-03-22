using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Logging.Library
  {
  public class Log 
    {
    // TODO start using LogCollectionManager, then make this stuff all static
    private ObservableCollection<LogEntryClass> _LogManager = null;
    public ObservableCollection<LogEntryClass> LogManager
      {
      get => _LogManager;
      set
        {
        _LogManager = value;
        //OnPropertyChanged("LogManager");
        }
      }

    public Log()
      {
      LogEventHandler.LogEvent += OnSaveLogEvent;
      LogManager = new ObservableCollection<LogEntryClass>();
      }

    private void OnSaveLogEvent(Object Sender, LogEventArgs E)
      {
      LogManager.Add(E.EntryClass);
      }

 

    // see https://stackoverflow.com/questions/12556767/how-do-i-get-the-current-line-number
    public static String Trace(String Text, LogEventType EventType = LogEventType.Message, Boolean PlaySound = false,
                                    [CallerMemberName] string CallingMethod = "",
                                    [CallerFilePath] string CallingFilePath = "",
                                    [CallerLineNumber] int CallingFileLineNumber = 0)
      {
      return Trace(Text, null, EventType, PlaySound, CallingMethod, CallingFilePath, CallingFileLineNumber);
      }

    public static String Trace(String Text, Exception ExceptionDetails, LogEventType EventType = LogEventType.Message, Boolean PlaySound = false,
      [CallerMemberName] string CallingMethod = "",
      [CallerFilePath] string CallingFilePath = "",
      [CallerLineNumber] int CallingFileLineNumber = 0)
      {

      var LogEvent = new LogEventArgs(new LogEntryClass(CallingMethod, Path.GetFileName(CallingFilePath), CallingFileLineNumber, Text, ExceptionDetails, EventType, PlaySound));
      LogEventHandler.OnLogEvent(LogEvent);
      return LogEvent.ToString();
      }
    }
  }
