using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
	{
	public class CountryModel
		{
		public int Id {get; set; }
		public string CountryName { get; set; }
		public string CountryAbbrev { get; set; }
		public bool Favorite { get; set; }

		}
	}
