BEGIN TRANSACTION;
CREATE VIEW IF NOT EXISTS ScenariosView AS 
SELECT 
    Routes.RouteName
	, Routes.RouteGuid
	, Scenarios.ScenarioTitle
	, Scenarios.ScenarioGuid
	, Scenarios.ScenarioClass
	, Scenarios.Pack
	, Scenarios.InGame
	, Scenarios.InArchive
	, Scenarios.IsvalidIngame
	, Scenarios.IsValidInArchive
 FROM Routes, Scenarios  WHERE Routes.Id= Scenarios.RouteId;
 COMMIT;