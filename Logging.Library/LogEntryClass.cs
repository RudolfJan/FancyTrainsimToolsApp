using System;

namespace Logging.Library
  {
  public class LogEntryClass

    {
    private String _LogEntry = String.Empty;
    public String LogEntry
      {
      get => _LogEntry;
      set
        {
        _LogEntry = value;
        //OnPropertyChanged("LogEntryClass");
        }
      }

    private LogEventType _EventType;
    public LogEventType EventType
      {
      get => _EventType;
      set
        {
        _EventType = value;
        //OnPropertyChanged("EventType");
        }
      }

    private String _Method = String.Empty;
    public String Method
      {
      get => _Method;
      set
        {
        _Method = value;
        //OnPropertyChanged("Method");
        }
      }

    private String _FilePath = String.Empty;
    public String FilePath
      {
      get => _FilePath;
      set
        {
        _FilePath = value;
        //OnPropertyChanged("FilePath");
        }
      }

    private int _LineNumber = 0;
    public int LineNumber
      {
      get => _LineNumber;
      set
        {
        _LineNumber = value;
        //OnPropertyChanged("LineNumber");
        }
      }

    private Exception _E;
    public Exception E
      {
      get { return _E; }
      set
        {
        _E = value;
        //OnPropertyChanged("E");
        }
      }

    private bool _PlaySound;
    public bool PlaySound
      {
      get => _PlaySound;
      set
        {
        _PlaySound = value;
        //OnPropertyChanged("PlaySound");
        }
      }
    public LogEntryClass(String MyMethod, String MyFilePath, int MyLineNumber, String MyText, Exception ExceptionDetails = null, LogEventType MyEventType= LogEventType.Message, bool MyPlaySound=false)
      {
      LogEntry = MyText;
      EventType = MyEventType;
      Method = MyMethod;
      FilePath = MyFilePath;
      LineNumber = MyLineNumber;
      E = ExceptionDetails;
      PlaySound = MyPlaySound;
      }

    public override String ToString()
      {
      var Output = EventType + " " + LogEntry;
      if (E != null)
        {
        Output += " Exception details: " + E.Message;
        }
      return Output;
      }
    }
  }