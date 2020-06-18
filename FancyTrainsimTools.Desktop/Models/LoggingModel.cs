﻿using Caliburn.Micro;
using Logging.Library;

namespace FancyTrainsimToolsDesktop.Models
	{
	public class LoggingModel
		{
		// public ILogCollectionManager Logging { get; set; }
		public BindableCollection<LogEntryClass> FilteredLogging { get; set; }
		public LogFilter Filter { get; set; } = new LogFilter(true, true, true, true);
		}
	}

