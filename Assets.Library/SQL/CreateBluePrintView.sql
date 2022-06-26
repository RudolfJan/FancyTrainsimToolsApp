CREATE VIEW IF NOT EXISTS [BluePrintView] AS 
    SELECT 
    Assets.Id
    , ProviderProducts.Provider
    , ProviderProducts.Product
    , ProviderProducts.Pack
    , Assets.BluePrintPath
    , AddOnCatalog.Location
    , Assets.InGame
    , Assets.InArchive
 FROM ProviderProducts, Assets, AddOnCatalog
 WHERE Assets.ProvProdId= ProviderProducts.Id AND Assets.AddOnCatalogId=AddOnCatalog.Id;