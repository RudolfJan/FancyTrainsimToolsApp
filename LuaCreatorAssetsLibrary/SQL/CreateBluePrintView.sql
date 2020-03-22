CREATE VIEW IF NOT EXISTS [BluePrintView] AS 
    SELECT 
    Assets.Id
    , ProviderProduct.Provider
    , ProviderProduct.Product
    , ProviderProduct.Pack
    , Assets.BluePrintPath
 FROM ProviderProduct, Assets
 WHERE Assets.ProvProdId= ProviderProduct.Id;