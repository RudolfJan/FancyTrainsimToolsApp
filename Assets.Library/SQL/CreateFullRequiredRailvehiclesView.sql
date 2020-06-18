DROP VIEW IF EXISTS FullRequiredRailVehiclesView;

CREATE VIEW IF NOT EXISTS FullRequiredRailVehiclesView AS 
SELECT DISTINCT RailVehicles.Id AS Id
				, RailVehicles.DisplayName AS DisplayName 
				, Assets.InGame AS InGame
				, Assets.InArchive AS InArchive
				, Assets.BluePrintPath AS BluePrintPath
				, ProviderProducts.Provider AS Provider
				, ProviderProducts.Product AS Product
				, VehicleTypes.VehicleType AS VehicleType
				, Consists.ScenarioId AS ScenarioId
		FROM RailVehicles, Consists, ConnectConsistRailVehicles, Assets, ProviderProducts, VehicleTypes
		WHERE ConnectConsistRailVehicles.ConsistId= Consists.Id 
			AND ConnectConsistRailVehicles.RailVehicleId= RailVehicles.Id
			AND RailVehicles.AssetId= Assets.Id 
			AND Assets.ProvProdId=ProviderProducts.Id
			AND RailVehicles.VehicleTypeId = VehicleTypes.Id;
