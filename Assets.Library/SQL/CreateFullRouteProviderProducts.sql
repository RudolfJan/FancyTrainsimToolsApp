DROP VIEW IF EXISTS FullRouteProviderProducts;

CREATE VIEW IF NOT EXISTS FullRouteProviderProducts AS 
SELECT DISTINCT Providerproduct.Id AS Id
  ,  Routes.RouteName AS RouteName
	, ProviderProduct.Provider AS Provider
	, ProviderProduct.Product as Product
	, ProviderProduct.Pack AS Pack
	, Routes.Id as RouteId
 FROM RouteAssets, Routes, Assets, ProviderProduct WHERE Routes.Id= RouteAssets.RouteId AND Assets.Id=RouteAssets.AssetId
 AND Assets.ProvProdId= ProviderProduct.Id;