CREATE VIEW IF NOT EXISTS RouteAssetsView AS 
SELECT 
    Routes.RouteName
	, ProviderProduct.Provider
	, ProviderProduct.Product
	, ProviderProduct.Pack
	, Assets.BluePrintPath
 FROM RouteAssets, Routes, Assets, ProviderProduct WHERE Routes.Id= RouteAssets.RouteId AND Assets.Id=RouteAssets.AssetId
 AND Assets.ProvProdId= ProviderProduct.Id;