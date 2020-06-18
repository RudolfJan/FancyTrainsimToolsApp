DROP VIEW IF EXISTS FullRouteProviderProducts;

CREATE VIEW IF NOT EXISTS FullRouteProviderProducts AS 
SELECT DISTINCT Providerproducts.Id AS Id
  ,  Routes.RouteName AS RouteName
	, ProviderProducts.Provider AS Provider
	, ProviderProducts.Product as Product
	, ProviderProducts.Pack AS Pack
	, ProviderProducts.InGame AS InGame
	, ProviderProducts.InArchive AS InArchive
	, Routes.Id as RouteId
 FROM RouteAssets, Routes, Assets, ProviderProducts WHERE Routes.Id= RouteAssets.RouteId AND Assets.Id=RouteAssets.AssetId
 AND Assets.ProvProdId= ProviderProducts.Id;