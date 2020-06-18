using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
	{
	// Model for SQL view FullRouteProviderProducts
	public class FullRouteProviderProductsModel
		{
		public int Id { get; set; }
		public string RouteName { get; set; }
		public string Provider { get; set; }
		public string Product { get; set; }
		public string Pack { get; set; }
		public bool InGame { get; set; }
		public bool InArchive { get; set; }
		public int RouteId { get; set; }
		}
	}
