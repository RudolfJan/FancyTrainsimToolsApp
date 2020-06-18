using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Assets.Library.Models
	{
	public class ConsistModel
		{
		public int Id { get; set; }
		public int ScenarioId { get; set; }
		public string ConsistName { get; set; }
		public string Destination { get; set; }
		public int StartTime { get; set; }
		public int ServiceClass { get; set; }

		public string ServiceClassText
			{
			get
				{
				return GetServiceClass(ServiceClass);
				}
			}
		public bool IsPlayer { get; set; }
		public bool IsValid { get; set; }
		public bool IsLoose { get; set; }
		public bool IsEmpty { get; set; }
		public List<FullRailVehicleModel> RailVehicleList { get; set; } = new List<FullRailVehicleModel>();
		public List<InstructionModel> InstructionList { get; set; }= new List<InstructionModel>();
		public XElement ConsistNode { get; set; }

		private static string GetServiceClass(int classId)
			{
			switch (classId)
				{
					case 0:
						{
						return "Special";
						}
					case 1:
						{
						return "LightEngine";
						}
					case 2:
						{
						return "Express Passenger";
						}
					case 3:
						{
						return "Stopping Passenger";
						}
					case 4:
						{
						return "High speed Freight";
						}
					case 5:
						{
						return "Express Freight";
						}
					case 6:
						{
						return "Standard Freight";
						}
					case 7:
						{
						return "Low speed Freight";
						}
					case 8:
						{
						return "Other Freight";
						}
					case 9:
						{
						return "Empty stock";
						}
					case 10:
						{
						return "International";
						}
					default:
						{
						return "Invalid service class";
						}
				}
			}
		}
	}
