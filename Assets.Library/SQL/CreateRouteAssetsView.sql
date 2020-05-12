CREATE VIEW IF NOT EXISTS FullRouteAssets AS 
SELECT 
    Routes.RouteName
	, ProviderProduct.Provider
	, ProviderProduct.Product
	, ProviderProduct.Pack
	, RouteAssets.InGame
	, RouteAssets.InArchive
	, Assets.BluePrintPath
 FROM RouteAssets, Routes, Assets, ProviderProduct WHERE Routes.Id= RouteAssets.RouteId AND Assets.Id=RouteAssets.AssetId
 AND Assets.ProvProdId= ProviderProduct.Id;