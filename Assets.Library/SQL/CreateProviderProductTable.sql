PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE IF NOT EXISTS [ProviderProducts] (
    [Id] INTEGER  Primary Key NOT NULL
    , [Provider] TEXT NOT NULL
    , [Product] TEXT NOT NULL
    , [Pack] TEXT DEFAULT ''
    , [InGame] INTEGER DEFAULT 0
    , [InArchive] INTEGER DEFAULT 0
, UNIQUE(Provider, Product)
);
