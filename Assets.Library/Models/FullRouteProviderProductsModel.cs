using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
	{
	// Model for SQL view FullRouteProviderProducts
	public class FullRouteProviderProductsModel
		{
		//Providerproduct.Id AS Id
		//,  Routes.RouteName AS RouteName
		//, ProviderProduct.Provider AS Provider
		//, ProviderProduct.Product as Product
		//, ProviderProduct.Pack AS Pack
		//, Routes.Id as RouteId

		public int Id { get; set; }
		public string RouteName { get; set; }
		public string Provider { get; set; }
		public string Product { get; set; }
		public string Pack { get; set; }
		public int RouteId { get; set; }
		}
	}
