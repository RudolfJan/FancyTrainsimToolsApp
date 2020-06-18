CREATE TABLE IF NOT EXISTS "ConnectConsistRailVehicles" (
	"Id"	INTEGER  Primary Key NOT NULL,
	"ConsistId"	INTEGER NOT NULL,
	"RailVehicleId"	INTEGER NOT NULL,
	"Order" INTEGER NOT NULL,
	FOREIGN KEY("ConsistId") REFERENCES "Consists"("Id") ON DELETE CASCADE,
	FOREIGN KEY("RailVehicleId") REFERENCES "RailVehicles"("Id") ON DELETE CASCADE,
	UNIQUE (ConsistId, RailVehicleId)
);