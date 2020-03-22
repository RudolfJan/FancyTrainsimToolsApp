using System;

namespace Logging.Library
  {
  public class LogFilter 
    {
    private Boolean _DebugChecked = true;
    public Boolean DebugChecked
      {
      get => _DebugChecked;
      set
        {
        _DebugChecked = value;
        //OnPropertyChanged("DebugChecked");
        }
      }

    private Boolean _ErrorChecked = true;
    public Boolean ErrorChecked
      {
      get => _ErrorChecked;
      set
        {
        _ErrorChecked = value;
        //OnPropertyChanged("ErrorChecked");
        }
      }

    private Boolean _MessageChecked = true;
    public Boolean MessageChecked
      {
      get => _MessageChecked;
      set
        {
        _MessageChecked = value;
        //OnPropertyChanged("MessageChecked");
        }
      }

    private Boolean _EventChecked = true;
    public Boolean EventChecked
      {
      get => _EventChecked;
      set
        {
        _EventChecked = value;
        //OnPropertyChanged("EventChecked");
        }
      }

    public LogFilter(Boolean debugChecked, Boolean errorChecked, Boolean messageChecked, Boolean eventChecked)
      {
      UpdateFilterSettings(debugChecked, errorChecked, messageChecked, eventChecked);
      }

    public void UpdateFilterSettings(Boolean debugChecked, Boolean errorChecked, Boolean messageChecked, Boolean eventChecked)
      {
      DebugChecked = debugChecked;
      ErrorChecked = errorChecked;
      MessageChecked = messageChecked;
      EventChecked = eventChecked;
      }


    public Boolean EventTypeFilter(object Item)
      {
      var MyItem = (LogEntryClass)Item;
      if (MyItem != null)
        {
        switch (MyItem.EventType)
          {
          case LogEventType.Error:
              {
              return ErrorChecked;
              }
          case LogEventType.Debug:
              {
              return DebugChecked;
              }
          case LogEventType.Message:
              {
              return MessageChecked;
              }
          case LogEventType.Event:
              {
              return EventChecked;
              }
          default:
              {
              return false;
              }
          }
        }
      return false;
      }
    }
  }
