PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE IF NOT EXISTS [ProviderProduct] (
    [Id] INTEGER primary key
    , [Provider] TEXT NOT NULL
    , [Product] TEXT NOT NULL
    , [Pack] TEXT DEFAULT ''
        , [InGame] INTEGER DEFAULT 0
    , [InArchive] INTEGER DEFAULT 0
, UNIQUE(Provider, Product)
);
