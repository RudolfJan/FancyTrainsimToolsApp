using Assets.Library.Helpers;
using Assets.Library.Models;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Assets.Library.Logic
	{
	public class InstructionsDataAccess
		{
		public static List<InstructionModel> GetInstructionsForConsist(XElement consistNode,
			int startTime)
			{
			var instructionList = new List<InstructionModel>();
			var PlayerDriverNode = consistNode.XPathSelectElement("Driver/cDriver/PlayerDriver");
			if (PlayerDriverNode != null && PlayerDriverNode.Value == "1")
				{
				var DriverNode = consistNode.XPathSelectElement(
					"Driver/cDriver/DriverInstructionContainer/cDriverInstructionContainer/DriverInstruction");
				if (DriverNode != null)
					{
					var InstructionNodeList = DriverNode.Elements();
					foreach (XElement InstructionNode in InstructionNodeList)
            {
            var instruction = GetInstruction(InstructionNode, startTime);
							
            instructionList.Add(instruction);
							
						}
					}

        var finalDestination = GetFinalDestination(consistNode, startTime);

					instructionList.Add(finalDestination);

				}
			return instructionList;
			}

		   #region Instruction builder

    private static  InstructionModel CreateInstruction(int startTime)
      {
      InstructionModel instruction= new InstructionModel();
      instruction.ScenarioStartTime = startTime;
      return instruction;
      }


    public static InstructionModel GetInstruction(XElement InstructionNode, int startTime)
      {
      switch (InstructionNode.Name.ToString())
        {
        case "cTriggerInstruction":
            {
            return GetTriggerInstruction(InstructionNode,startTime);
            }
        case "cStopAtDestinations":
            {
            return GetStopAtInstruction(InstructionNode,startTime);
            }
        case "cPickUpPassengers":
            {
            return GetPickupPassengersInstruction(InstructionNode,startTime);
            }

        case "cConsistOperations":
            {
            return GetConsistOperation(InstructionNode,startTime);
            }
        case "cPickUpFuelOrFreight":
            {
            return GetPickupFuelOrFreightInstruction(InstructionNode,startTime);
            }
        default:
            {
            Log.Trace("InstructionType " + InstructionNode.Name.ToString() + " not recognized", LogEventType.Error);
            return null; // Something is wrong, instructiontype not recognized
            }
        }
      }

    #endregion Instruction builder

    //Note: it looks attractive to extract a common part, but this does not help very much, due to the more subtle differences in each instruction type.
    // So for now we will not do this, but focus on getting all possible details.

    #region TriggerInstruction

    private static InstructionModel GetTriggerInstruction(XElement InstructionNode, int startTime)
      {
      var instruction = CreateInstruction(startTime);
      // Note the Trigger instruction uses an empty DeltaTarget
      instruction.InstructionType = "Trigger instruction";
      var SuccessEventNode = InstructionNode.Element("SuccessEvent");
      if (SuccessEventNode != null)
        {
        instruction.SuccessEvent = SuccessEventNode.Value;
        }

      var EventTextNode = InstructionNode.XPathSelectElement("DisplayText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.DisplayText = XmlHelpers.GetLocalisedString(EventTextNode);
        }

      var FailureEventNode = InstructionNode.Element("FailureEvent");
      if (FailureEventNode != null)
        {
        instruction.FailureEvent = FailureEventNode.Value;
        }
      instruction.RepeatCount = 1;
      instruction.Location = "n.a.";
      instruction.SecondsDelay = InstructionNode.Element("SecondsDelay")?.Value;
      instruction.TriggerTrainStop = InstructionNode.Element("TriggerTrainStop")?.Value == "1";
      instruction.TriggerWheelSlip = InstructionNode.Element("TriggerWheelSlip")?.Value == "1";
      return instruction;
      }

    #endregion TriggerInstruction

    #region StopAtInstruction

    private static InstructionModel  GetStopAtInstruction(XElement InstructionNode, int startTime)
      {
      var instruction = CreateInstruction(startTime);
      instruction.InstructionType = "Stop at";
      instruction.RepeatCount = 0;
      var SuccessEventNode = InstructionNode.Element("SuccessEvent");
      if (SuccessEventNode != null && SuccessEventNode.Value != null)
        {
        instruction.SuccessEvent = SuccessEventNode.Value;
        }

      var FailureEventNode = InstructionNode.Element("FailureEvent");
      if (FailureEventNode != null && FailureEventNode.Value != null)
        {
        instruction.FailureEvent = FailureEventNode.Value;
        }

      var EventTextNode = InstructionNode.XPathSelectElement("DisplayText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.DisplayText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("TriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.SuccessEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("UntriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.FailureEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }

      var DeltaTargetNode = InstructionNode.XPathSelectElements("DeltaTarget/cDriverInstructionTarget");

      var First = true;
      foreach (XElement TargetNode in DeltaTargetNode)
        {
        instruction.RepeatCount++;
        if (First)
          {
          var LocationNode = TargetNode.Element("DisplayName");
          if (LocationNode != null)
            {
            instruction.Location = LocationNode.Value;
            }
          var TimetabledNode = TargetNode.Element("Timetabled");
          First = true;
          if (TimetabledNode != null)
            {
            instruction.TimeTabled = (TimetabledNode.Value == "1");
            }
          var HiddenNode = TargetNode.Element("Hidden");
          if (HiddenNode != null)
            {
            instruction.Hidden = (HiddenNode.Value == "1");
            }
          var MinSpeedNode = TargetNode.Element("MinSpeed");
          if (MinSpeedNode.Value != "0")
            {
            instruction.InstructionType = "Go via";
            }
          var WaypointNode = TargetNode.Element("Waypoint");

          if (WaypointNode.Value == "1")
            {
            instruction.InstructionType = "Waypoint";
            }

          var DueTimeNode = TargetNode.Element("DueTime");
          if (DueTimeNode != null)
            {
            int.TryParse(DueTimeNode.Value, out instruction.DueTime);
            if (instruction.TimeTabled)
              {
              instruction.DueTimeString = Converters.TimeToString(instruction.ScenarioStartTime + instruction.DueTime);
              }
            }
          var DurationNode = TargetNode.Element("Duration");
          if (DurationNode != null)
            {
            int.TryParse(DurationNode.Value, out int Tmp);
            if (WaypointNode.Value == "1" || MinSpeedNode.Value != "0")
              {
              instruction.Duration = 0;
              }
            else
              {
              instruction.Duration = Tmp;
              }

            }
          }
        }
      return instruction;
      }

    #endregion StopAtInstruction

    #region PickUppassengers

    private static InstructionModel  GetPickupPassengersInstruction(XElement InstructionNode, int startTime)
      {
      var instruction = CreateInstruction(startTime);
      instruction.InstructionType = "Pickup passengers";
      instruction.RepeatCount = 0;
      var SuccessEventNode = InstructionNode.Element("SuccessEvent");
      if (SuccessEventNode != null && SuccessEventNode.Value != null)
        {
        instruction.SuccessEvent = SuccessEventNode.Value;
        }

      var FailureEventNode = InstructionNode.Element("FailureEvent");
      if (FailureEventNode != null && FailureEventNode.Value != null)
        {
        instruction.FailureEvent = FailureEventNode.Value;
        }

      var EventTextNode = InstructionNode.XPathSelectElement("DisplayText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.DisplayText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("TriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.SuccessEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("UntriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.FailureEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }

      var DeltaTargetNode = InstructionNode.XPathSelectElements("DeltaTarget/cDriverInstructionTarget");

      Boolean First = true;
      foreach (XElement TargetNode in DeltaTargetNode)
        {
        instruction.RepeatCount++;
        if (First)
          {
          var LocationNode = TargetNode.Element("DisplayName");
          if (LocationNode != null)
            {
            instruction.Location = LocationNode.Value;
            }
          var TimetabledNode = TargetNode.Element("Timetabled");
          First = true;
          if (TimetabledNode != null)
            {
            instruction.TimeTabled = (TimetabledNode.Value == "1");
            }
          var HiddenNode = TargetNode.Element("Hidden");
          if (HiddenNode != null)
            {
            instruction.Hidden = (HiddenNode.Value == "1");
            }

          var DueTimeNode = TargetNode.Element("DueTime");
          if (DueTimeNode != null)
            {
            int.TryParse(DueTimeNode.Value, out instruction.DueTime);
            if (instruction.TimeTabled)
              {
              instruction.DueTimeString = Converters.TimeToString(instruction.ScenarioStartTime + instruction.DueTime);
              }
            }
          var DurationNode = TargetNode.Element("Duration");
          if (DurationNode != null)
            {
            int.TryParse(DurationNode.Value, out int Tmp);
            instruction.Duration = Tmp;
            }
          }
        }
      return instruction;
      }

    #endregion PickUppassengers

    #region ConsistOperation

    private static InstructionModel  GetConsistOperation(XElement InstructionNode, int startTime)
      {
      var instruction = CreateInstruction(startTime);
      instruction.InstructionType = "Consist Operations";
      instruction.RepeatCount = 0;
      var SuccessEventNode = InstructionNode.Element("SuccessEvent");
      if (SuccessEventNode != null && SuccessEventNode.Value != null)
        {
        instruction.SuccessEvent = SuccessEventNode.Value;
        }
      var FailureEventNode = InstructionNode.Element("FailureEvent");
      if (FailureEventNode != null && FailureEventNode.Value != null)
        {
        instruction.FailureEvent = FailureEventNode.Value;
        }
      var EventTextNode = InstructionNode.XPathSelectElement("DisplayText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.DisplayText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("TriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.SuccessEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("UntriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.FailureEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }

      var DeltaTargetNode = InstructionNode.XPathSelectElements("DeltaTarget/cDriverInstructionTarget");

      Boolean First = true;
      foreach (XElement TargetNode in DeltaTargetNode)
        {
        instruction.RepeatCount++;
        if (First)
          {
          var LocationNode = TargetNode.Element("DisplayName");
          if (LocationNode != null)
            {
            instruction.Location = LocationNode.Value;
            }
          var TimetabledNode = TargetNode.Element("Timetabled");
          First = true;
          if (TimetabledNode != null)
            {
            instruction.TimeTabled = (TimetabledNode.Value == "1");
            }
          var HiddenNode = TargetNode.Element("Hidden");
          if (HiddenNode != null)
            {
            instruction.Hidden = (HiddenNode.Value == "1");
            }

          // Get description for consist operation
          var GroupNameNode = TargetNode.XPathSelectElement("GroupName/Localisation-cUserLocalisedString");
          if (GroupNameNode != null)
            {
            instruction.GroupName = XmlHelpers.GetLocalisedString(GroupNameNode);
            }

          var OperationNode = TargetNode.Element("Operation");
          if (OperationNode != null)
            {
            if (String.CompareOrdinal(OperationNode.Value, "DropOffRailVehicle") == 0)
              {
              instruction.InstructionType = "Drop off Railvehicles";
              }
            else
              {
              instruction.InstructionType = "Couple Railvehicles";
              }
            }

          var DueTimeNode = TargetNode.Element("DueTime");
          if (DueTimeNode != null)
            {
            int.TryParse(DueTimeNode.Value, out instruction.DueTime);
            if (instruction.TimeTabled)
              {
              instruction.DueTimeString = Converters.TimeToString(instruction.ScenarioStartTime + instruction.DueTime);
              }
            }
          var DurationNode = TargetNode.Element("Duration");
          if (DurationNode != null)
            {
            int.TryParse(DurationNode.Value, out int Tmp);
            instruction.Duration = Tmp;
            }
          }
        }
      return instruction;
      }

    #endregion ConsistOperation

    #region PickUpFuelorFreight

    private static InstructionModel  GetPickupFuelOrFreightInstruction(XElement InstructionNode, int startTime)
      {
      var instruction = CreateInstruction(startTime);
      instruction.InstructionType = "Pickup fuel or freight";
      instruction.RepeatCount = 0;
      var SuccessEventNode = InstructionNode.Element("SuccessEvent");
      if (SuccessEventNode != null && SuccessEventNode.Value != null)
        {
        instruction.SuccessEvent = SuccessEventNode.Value;
        }
      var FailureEventNode = InstructionNode.Element("FailureEvent");
      if (FailureEventNode != null && FailureEventNode.Value != null)
        {
        instruction.FailureEvent = FailureEventNode.Value;
        }

      var EventTextNode = InstructionNode.XPathSelectElement("DisplayText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.DisplayText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("TriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.SuccessEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }
      EventTextNode = InstructionNode.XPathSelectElement("UntriggeredText/Localisation-cUserLocalisedString");
      if (EventTextNode != null)
        {
        instruction.FailureEventText = XmlHelpers.GetLocalisedString(EventTextNode);
        }

      var DeltaTargetNode = InstructionNode.XPathSelectElements("DeltaTarget/cDriverInstructionTarget");

      Boolean First = true;
      foreach (XElement TargetNode in DeltaTargetNode)
        {
        instruction.RepeatCount++;
        if (First)
          {
          var LocationNode = TargetNode.Element("DisplayName");
          if (LocationNode != null)
            {
            instruction.Location = LocationNode.Value;
            }
          var TimetabledNode = TargetNode.Element("Timetabled");
          First = true;
          if (TimetabledNode != null)
            {
            instruction.TimeTabled = (TimetabledNode.Value == "1");
            }
          var HiddenNode = TargetNode.Element("Hidden");
          if (HiddenNode != null)
            {
            instruction.Hidden = (HiddenNode.Value == "1");
            }

          var DueTimeNode = TargetNode.Element("DueTime");
          if (DueTimeNode != null)
            {
            int.TryParse(DueTimeNode.Value, out instruction.DueTime);
            if (instruction.TimeTabled)
              {
              instruction.DueTimeString = Converters.TimeToString(instruction.ScenarioStartTime + instruction.DueTime);
              }
            }
          var DurationNode = TargetNode.Element("Duration");
          if (DurationNode != null)
            {
            int.TryParse(DurationNode.Value, out int Tmp);
            instruction.Duration = Tmp;
            }
          }
        }
      return instruction;
      }

    #endregion PickUpFuelorFreight

    #region FinalDestination

    public static InstructionModel  GetFinalDestination(XElement ConsistNode, int startTime)
      {
      var instruction = CreateInstruction(startTime);
      var FinalDestinationMarkerNode = ConsistNode.XPathSelectElement("Driver/cDriver/FinalDestination/cDriverInstructionTarget/DisplayName");
      if (FinalDestinationMarkerNode != null)
        {
        instruction.InstructionType = "Final Destination";
        instruction.Location = FinalDestinationMarkerNode.Value;
        instruction.Hidden = false;
        instruction.RepeatCount = 1;
        instruction.TimeTabled = false;
        return instruction;
        }

      return instruction;
      }

    #endregion FinalDestination

    // Generate Lua code for an instruction
    // index is an index, you should have it converted to string already
    public static string BuildLuaInstruction(String Index, InstructionModel instruction)
      {
      var X = String.Empty;
      X += "\t\tPlayerInstructionsList[" + Index + "][\"InstructionType\"]=\"" + instruction.InstructionType + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"SuccessEvent\"]=\"" + instruction.SuccessEvent + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"FailureEvent\"]=\"" + instruction.FailureEvent + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"Location\"]=\"" + instruction.Location + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"Hidden\"]=\"" + instruction.Hidden.ToString() + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"TimeTabled\"]=\"" + instruction.TimeTabled.ToString() + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"DueTime\"]=\"" + instruction.DueTime.ToString() + "\"\r\n";
      X += "\t\tPlayerInstructionsList[" + Index + "][\"DepartureTime\"]=\"" + instruction.EarliestDepartureTime.ToString() + "\"\r\n";
      return X;
      }

    private static string BuildLuaInstructionsList(List<InstructionModel> instructionList)
      {
      string LuaInstructionsList = "-- Instructions for the player consist\r\n";
      LuaInstructionsList += "PlayerInstructionsList={}\r\n";
      int I = 0;
      foreach (var Instr in instructionList)
        {
        I++;
        LuaInstructionsList += "\tPlayerInstructionsList[" + I + "]={}\r\n";
        LuaInstructionsList += BuildLuaInstruction(I.ToString(), Instr);
        }
      LuaInstructionsList += "\r\n";
      return LuaInstructionsList;
      }

    // Create a searchable list of all events.
    public static string WriteEventsToLuaTable(List<InstructionModel> instructionList)
      {
      var Output = "TestStub_EventList={}\n";
      foreach (var Instr in instructionList)
        {
        if (Instr != null)
          {
          if (Instr.SuccessEvent.Length > 0)
            {
            Output += "TestStub_EventList[\"" + Instr.SuccessEvent + "\"]=true;\n";
            }
          if (Instr.FailureEvent.Length > 0)
            {
            Output += "TestStub_EventList[\"" + Instr.FailureEvent + "\"]=true;\n";
            }
          }
        }
      return Output;
      }

    public static void CreateLuaInstructionsListFile(string directory, string luaInstructionsList)
      {
			try
				{
				Directory.CreateDirectory(directory);
				File.WriteAllText($"{directory}InstructionsList.Lua", luaInstructionsList);
				}
			catch (Exception ex)
        {
        Log.Trace($"Cannot create Lua Instructions file", ex, LogEventType.Error);

        }
      }
    }
  }









	
	
