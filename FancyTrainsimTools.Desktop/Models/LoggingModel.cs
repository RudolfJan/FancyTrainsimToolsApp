using Logging.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FancyTrainsimTools.Desktop.Models
  {
  public class LoggingModel
    {
    public ILogCollectionManager Logging { get; set; } = App.GetLogger();
    public ILogCollectionManager FilteredLogging { get; set; } = App.GetLogger();
    public LogFilter Filter { get; set; } = new LogFilter(false,true,true,false);
    }
  }
