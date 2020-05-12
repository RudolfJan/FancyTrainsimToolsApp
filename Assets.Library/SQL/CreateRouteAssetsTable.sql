PRAGMA foreign_keys=ON;
PRAGMA recursive_triggers=ON;
CREATE TABLE IF NOT EXISTS [RouteAssets] (
    [Id] INTEGER primary key
    , [InGame] INTEGER DEFAULT 0
    , [InArchive] INTEGER DEFAULT 0
    , [RouteId]	INTEGER  REFERENCES Routes (Id) ON DELETE SET NULL ON UPDATE NO ACTION
    , [AssetId] INTEGER  REFERENCES Assets (Id) ON DELETE SET NULL ON UPDATE NO ACTION
, UNIQUE(RouteId,AssetId));