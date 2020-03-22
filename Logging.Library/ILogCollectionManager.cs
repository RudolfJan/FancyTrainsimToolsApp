// Created by:  Rudolf Heijink
// Copyright:  (C) 2020 Rudolf Heijink
// Created date: --
// File: ILogCollectionManager.cs
// Project: Logging.Library

using System;
using System.Collections.Generic;

namespace Logging.Library
  {
  public interface ILogCollectionManager
    {
    List<LogEntryClass> LogEvents { get; set; }
    void OnSaveLogEvent(Object Sender, LogEventArgs E);
    }
  }
