using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
	{
	public class InstructionModel
		{
		public string InstructionType { get; set; } = string.Empty;
		public string DisplayText { get; set; } = string.Empty;
		public string SuccessEvent { get; set; } = string.Empty;
		public string FailureEvent { get; set; } = string.Empty;
		public string SecondsDelay { get; set; } = string.Empty;
		public int DueTime = 0; // latest time before penalty
		public string DueTimeString { get; set; } = string.Empty;
		public int Duration { get; set; } = 0; // duration of stop
		public int EarliestDepartureTime = 0;
		public string Location { get; set; } = string.Empty;
		public bool Hidden { get; set; } = false;
		public bool TimeTabled { get; set; } = false;
		public int RepeatCount { get; set; } = 0;
		public string SuccessEventText { get; set; } = string.Empty;
		public string FailureEventText { get; set; } = string.Empty;
		public bool TriggerTrainStop { get; set; } = false;
		public bool TriggerWheelSlip { get; set; } = false;
		public int ScenarioStartTime { get; set; } = 0;

		// Consist specific items
		public string GroupName { get; set; } = string.Empty;
		}
	}
