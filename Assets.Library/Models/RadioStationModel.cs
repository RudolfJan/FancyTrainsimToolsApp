using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
	{
	public class RadioStationModel
		{
		//"Id"	INTEGER NOT NULL,
		//"RadioStationUrl"	TEXT NOT NULL,
		//"RadioStationName"	TEXT NOT NULL,
		//"RadioStationDescription"	TEXT NOT NULL DEFAULT '',

		public int Id { get; set; }
		public string RadioStationUrl { get; set; }
		public string RadioStationName { get; set; }
		public string RadioStationDescription { get; set; }
		}
	}
