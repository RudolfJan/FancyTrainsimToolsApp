PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE IF NOT EXISTS [Routes] (
    [Id] INTEGER primary key
    , [RouteName] TEXT NOT NULL
    , [RouteGuid] TEXT NOT NULL
    , [Pack] TEXT DEFAULT ''
    , [InGame] INTEGER DEFAULT 0
    , [InArchive] INTEGER DEFAULT 0
, UNIQUE(RouteGuid));