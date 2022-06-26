namespace Assets.Library.Models
	{

	public enum AddOnTypeEnum
		{
		Undefined,
		Other,
		Route,
		Scenario,
		Scenery,
		RailVehicle,
		Audio
		}

	public class AddOnCatalogModel
		{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ArchiveName { get; set; }
		public string Location { get; set; }
		public string Description { get; set; }
		public AddOnTypeEnum AddOnType { get; set; }
		public bool IsPayware { get; set; }

		public static string ConvertAddOnTypeToString(AddOnTypeEnum AddOnType)
			{
			return AddOnType switch
				{
					AddOnTypeEnum.Other => "Other",
					AddOnTypeEnum.Route => "Route",
					AddOnTypeEnum.Scenario => "Scenario",
					AddOnTypeEnum.RailVehicle => "RailVehicle",
					AddOnTypeEnum.Scenery => "Scenery",
					AddOnTypeEnum.Audio => "Audio",
					AddOnTypeEnum.Undefined => "Undefined",
					_ => "",
					};
			}

		public static AddOnTypeEnum ConvertStringToAddOnType(string addOnTypeString)
			{
			return addOnTypeString switch
				{
					"Undefined" => AddOnTypeEnum.Undefined,
					"Other" => AddOnTypeEnum.Other,
					"Route" => AddOnTypeEnum.Route,
					"Scenario" => AddOnTypeEnum.Scenario,
					"RailVehicle" => AddOnTypeEnum.RailVehicle,
					"Scenery" => AddOnTypeEnum.Scenery,
					"Audio" => AddOnTypeEnum.Audio,
					_ => AddOnTypeEnum.Undefined,
					};
			}
		}
	}
