CREATE TABLE IF NOT EXISTS "Gauges" (
	"Id"	INTEGER  Primary Key NOT NULL,
	"GaugeName"	TEXT NOT NULL UNIQUE,
	"GaugeDescription"	TEXT NOT NULL
);