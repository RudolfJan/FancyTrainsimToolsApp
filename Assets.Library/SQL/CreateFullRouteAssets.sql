DROP VIEW IF EXISTS FullRouteAssets;

CREATE VIEW IF NOT EXISTS FullRouteAssets AS 
SELECT
		Routes.Id AS RouteId
  , Routes.RouteName AS RouteName
	, ProviderProducts.Provider AS Provider
	, ProviderProducts.Product AS product
	, ProviderProducts.Pack AS Pack
	, Assets.InGame AS InGame
	, Assets.InArchive AS InArchive
	, Assets.BluePrintPath As BluePrintPath
 FROM RouteAssets, Routes, Assets, ProviderProducts WHERE Routes.Id= RouteAssets.RouteId AND Assets.Id=RouteAssets.AssetId
 AND Assets.ProvProdId= ProviderProducts.Id;