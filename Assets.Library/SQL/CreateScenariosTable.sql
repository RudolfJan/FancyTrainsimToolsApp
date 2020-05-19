BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Scenarios" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"ScenarioTitle"	TEXT NOT NULL,
	"ScenarioGuid"	TEXT NOT NULL UNIQUE,
	"ScenarioClass"	TEXT NOT NULL,
	"InGame"	INTEGER NOT NULL DEFAULT 0,
	"InArchive"	INTEGER NOT NULL DEFAULT 0,
	"IsValidInGame"	INTEGER NOT NULL DEFAULT 0,
	"IsValidInArchive"	INTEGER NOT NULL DEFAULT 0,
	"Pack"	TEXT NOT NULL,
	"IsPacked"	INTEGER NOT NULL DEFAULT 0,
	"RouteId"	INTEGER NOT NULL REFERENCES Routes (Id) ON DELETE CASCADE ON UPDATE NO ACTION
);

COMMIT;
