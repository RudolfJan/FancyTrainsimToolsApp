using Caliburn.Micro;
using FancyTrainsimTools.Desktop.Helpers;
using FancyTrainsimTools.Desktop.Models;
using Logging.Library;
using System.IO;
using System.Threading.Tasks;

namespace FancyTrainsimTools.Desktop.ViewModels
	{
	public class LoggingViewModel : Screen
		{
		public LoggingModel Logging { get; set; } = new LoggingModel();
		public bool _debugLogging = true;

		public bool DebugLogging
			{
			get { return _debugLogging; }
			set
				{
				_debugLogging = value;
				NotifyOfPropertyChange(() => DebugLogging);
				Logging.Filter.DebugChecked = DebugLogging;
				ChangeFilter();
				}
			}

		public bool _messageLogging = true;

		public bool MessageLogging
			{
			get { return _messageLogging; }
			set
				{
				_messageLogging = value;
				NotifyOfPropertyChange(() => MessageLogging);
				Logging.Filter.MessageChecked = MessageLogging;
				ChangeFilter();
				}
			}

		public bool _errorLogging = true;

		public bool ErrorLogging
			{
			get { return _errorLogging; }
			set
				{
				_errorLogging = value;
				NotifyOfPropertyChange(() => ErrorLogging);
				Logging.Filter.ErrorChecked = ErrorLogging;
				ChangeFilter();
				}
			}

		public bool _eventLogging = true;

		public bool EventLogging
			{
			get { return _eventLogging; }
			set
				{
				_eventLogging = value;
				NotifyOfPropertyChange(() => EventLogging);
				Logging.Filter.EventChecked = EventLogging;
				ChangeFilter();
				}
			}

		public LoggingViewModel()
			{
			}

		protected override void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			Logging.FilteredLogging = new BindableCollection<LogEntryClass>();
			CreateTestData();
			// Setup initial values for logging filter
			ChangeFilter();
			LogEventHandler.LogEvent += LogEventHandlerOnLogEvent;
			}

		private void LogEventHandlerOnLogEvent(object sender, LogEventArgs e)
			{
			ChangeFilter();
			}

		private void CreateTestData()
			{
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 1, "Test line Message", null,
				LogEventType.Message));
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 2, "Test line Message", null,
				LogEventType.Message));
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 2, "Test line Debug", null,
				LogEventType.Debug));
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 2, "Test line Error", null,
				LogEventType.Error));
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 2, "Test line Error", null,
				LogEventType.Error));
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 2, "Test line Event", null,
				LogEventType.Event));
			LogCollectionManager.LogEvents.Add(new LogEntryClass("", "", 2, "Test line Error", null,
				LogEventType.Error));
			}

		private void ChangeFilter()
			{
			Logging.FilteredLogging.Clear();
			foreach (var item in LogCollectionManager.LogEvents)
				{
				if (Logging.Filter.EventTypeFilter(item))
					{
					Logging.FilteredLogging.Add(item);
					}

				Logging.FilteredLogging.Refresh();
				}
			}

		public void ClearLog()
			{
			Logging.FilteredLogging.Clear();
			LogCollectionManager.LogEvents.Clear();
			Logging.FilteredLogging.Refresh();
			}

		public void SaveLog()
			{
			var fileSaveParams = new SaveFileModel
				{
				Title = "Save log file as csv",
				InitialDirectory = Settings.DataPath
				};
			var outputFile = FileIOHelper.GetSaveFileName(fileSaveParams);
			if (outputFile.Length > 0)
				{
				var allText = LogEntryClass.WriteCsvHeaderLine();
				foreach (var X in LogCollectionManager.LogEvents)
					{
					allText += X.WriteAsCsv();
					}

				File.WriteAllText(outputFile, allText);
				}
			}

		public async Task OKButton()
			{
			await TryCloseAsync();
			}
		}
	}
