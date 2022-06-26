PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE  IF NOT EXISTS [Assets] (
    [Id] INTEGER NOT NULL Primary Key
    , [ProvProdId] INTEGER REFERENCES ProviderProducts (Id) ON DELETE SET NULL ON UPDATE NO ACTION
    , [BluePrintPath] TEXT NOT NULL
    , [InGame] INTEGER DEFAULT 0
    , [InArchive] INTEGER DEFAULT 0
    , [AddOnCatalogId] INTEGER REFERENCES AddOnCatalog (Id) ON DELETE SET NULL
    , [Image] TEXT NOT NULL DEFAULT ''
, UNIQUE(BluePrintPath,ProvProdId)
);

