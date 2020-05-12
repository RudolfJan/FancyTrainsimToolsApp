using FancyTrainsimTools.Desktop.Models;
using FancyTrainsimTools.Desktop.Views;
using Logging.Library;
using Mvvm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class LoggingViewModel: BindableBase
    {
    private readonly IServiceProvider _serviceProvider;

    public LoggingModel Logging { get; set; }= new LoggingModel();
    public ICommand ClearCommand { get; }
    public ICommand ChangeFilterCommand { get; }

    public LoggingViewModel()
      {
      ClearCommand= new RelayCommand(Clear);
      ChangeFilterCommand= new RelayCommand(ChangeFilter);
      _serviceProvider = App.serviceProvider;
      // CreateTestData();
      }

    private void CreateTestData()
      {
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",1,"Test line Message",null,LogEventType.Message));
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",2,"Test line Message",null,LogEventType.Message));
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",2,"Test line Debug",null,LogEventType.Debug));
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",2,"Test line Error",null,LogEventType.Error));
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",2,"Test line Error",null,LogEventType.Error));
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",2,"Test line Event",null,LogEventType.Event));
      Logging.Logging.LogEvents.Add(new LogEntryClass("","",2,"Test line Error",null,LogEventType.Error));
      }

    private void ChangeFilter()
      {
      Logging.FilteredLogging.LogEvents = GetFilteredLogging(Logging.Filter, Logging.Logging);
      }

    private void Clear()
      {
      Logging.Logging.LogEvents.Clear();
      Logging.FilteredLogging.LogEvents.Clear();
      }

    static List<LogEntryClass> GetFilteredLogging(LogFilter filter,ILogCollectionManager logging)
      {
      return logging.LogEvents.Where(filter.EventTypeFilter).ToList();
      }
 
    }
  }
