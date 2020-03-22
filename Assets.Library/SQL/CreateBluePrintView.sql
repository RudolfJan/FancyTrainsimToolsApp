CREATE VIEW IF NOT EXISTS [BluePrintView] AS 
    SELECT 
    Assets.Id
    , ProviderProduct.Provider
    , ProviderProduct.Product
    , ProviderProduct.Pack
    , Assets.BluePrintPath
    , Assets.InGame
    , Assets.InArchive
 FROM ProviderProduct, Assets
 WHERE Assets.ProvProdId= ProviderProduct.Id;