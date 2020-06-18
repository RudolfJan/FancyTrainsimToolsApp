﻿CREATE TABLE IF NOT EXISTS "RailVehicles" (
	"Id"	INTEGER  Primary Key NOT NULL,
	"AssetId"	INTEGER,
	"DisplayName"	TEXT NOT NULL,
	"CountryId"	INTEGER DEFAULT 1,
	"VehicleTypeId"	INTEGER DEFAULT 1,
	"GaugeId"	INTEGER DEFAULT 1,
	"OperatingCompanyId"	INTEGER DEFAULT 1,
	FOREIGN KEY("VehicleTypeId") REFERENCES "VehicleTypes"("Id") ON DELETE SET DEFAULT,
	FOREIGN KEY("CountryId") REFERENCES "Countries"("Id") ON DELETE SET DEFAULT,
	FOREIGN KEY("GaugeId") REFERENCES "Gauges"("Id") ON DELETE SET DEFAULT,
	UNIQUE (AssetId)
)