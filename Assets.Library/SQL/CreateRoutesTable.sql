PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE IF NOT EXISTS [Routes] (
    [Id] INTEGER primary key
    , [RouteName] TEXT NOT NULL
    , [RouteGuid] TEXT NOT NULL
    , [Pack] TEXT NOT NULL DEFAULT ''
    , [IsPacked] INTEGER NOT NULL DEFAULT 0
    , [InGame] INTEGER NOT NULL DEFAULT 0
    , [InArchive] INTEGER NOT NULL DEFAULT 0
    , [IsValidInGame] INTEGER NOT NULL DEFAULT 0
    , [IsValidInArchive] INTEGER NOT NULL DEFAULT 0
, UNIQUE(RouteGuid));