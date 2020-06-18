CREATE TABLE IF NOT EXISTS "OperatingCompanies" (
	"Id"	INTEGER  Primary Key NOT NULL,
	"OperatingCompanyName"	TEXT NOT NULL UNIQUE,
	"OperatingCompanyDescription"	TEXT NOT NULL
);