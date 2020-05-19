DROP VIEW IF EXISTS FullRouteAssets;

CREATE VIEW IF NOT EXISTS FullRouteAssets AS 
SELECT
		Routes.Id AS RouteId
  , Routes.RouteName AS RouteName
	, ProviderProduct.Provider AS Provider
	, ProviderProduct.Product AS product
	, ProviderProduct.Pack AS Pack
	, RouteAssets.InGame AS InGame
	, RouteAssets.InArchive AS InArchive
	, Assets.BluePrintPath As BluePrintPath
 FROM RouteAssets, Routes, Assets, ProviderProduct WHERE Routes.Id= RouteAssets.RouteId AND Assets.Id=RouteAssets.AssetId
 AND Assets.ProvProdId= ProviderProduct.Id;