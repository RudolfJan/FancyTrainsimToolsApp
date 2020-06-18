using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Assets.Library.Models
	{
	public class FullRailVehicleModel
		{
		public int Id { get; set; }
		public int AssetId { get; set; }
		public bool IsMisssing { get; set; }
		public bool InGame { get; set; }
		public bool InArchive { get; set; }
		public string DisplayName { get; set; }
		public string Provider { get; set; }
		public string Product { get; set; }
		public string BlueprintPath { get; set; } // Normalized version!
		public int VehicleTypeId { get; set; } = 1;
		public string VehicleType { get; set; }
		public string VehicleTypeDescription { get; set; }
		public int CountryId { get; set; } = 1;
		public string CountryName { get; set; }
		public string CountryAbbrev { get; set; }
		public int GaugeId { get; set; } = 1;
		public string Gauge { get; set; }
		public string GaugeDescription { get; set; }
		public int OperatingCompanyId { get; set; } = 1;
		public string OperatingCompanyName { get; set; }
		public string OperatingCompanyDescription { get; set; }
		public XElement RailVehicleNode { get; set; }
		}
	}
