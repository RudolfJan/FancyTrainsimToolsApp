CREATE VIEW IF NOT EXISTS [BluePrintView] AS 
    SELECT 
    Assets.Id
    , ProviderProducts.Provider
    , ProviderProducts.Product
    , ProviderProducts.Pack
    , Assets.BluePrintPath
    , Assets.InGame
    , Assets.InArchive
 FROM ProviderProducts, Assets
 WHERE Assets.ProvProdId= ProviderProducts.Id;