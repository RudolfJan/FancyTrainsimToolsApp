CREATE TABLE IF NOT EXISTS "VehicleTypes" (
	"Id" INTEGER  Primary Key NOT NULL,
	"VehicleType"	TEXT NOT NULL UNIQUE,
	"VehicleTypeDescription"	TEXT NOT NULL
);