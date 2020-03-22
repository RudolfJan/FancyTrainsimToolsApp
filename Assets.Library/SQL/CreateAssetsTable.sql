PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE  IF NOT EXISTS [Assets] (
    [Id] INTEGER primary key
    , [ProvProdId] INTEGER REFERENCES ProviderProduct (Id) ON DELETE SET NULL ON UPDATE NO ACTION
    , [BluePrintPath] TEXT NOT NULL
    , [InGame] INTEGER DEFAULT 0
    , [InArchive] INTEGER DEFAULT 0
, UNIQUE(BluePrintPath));

