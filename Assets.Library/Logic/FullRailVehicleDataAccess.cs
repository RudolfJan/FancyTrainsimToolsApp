using Assets.Library.Helpers;
using Assets.Library.Models;
using Dapper;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Assets.Library.Logic
	{
	public class FullRailVehicleDataAccess
		{
		public static List<FullRailVehicleModel> GetRailVehiclesForConsist(XElement consistNode)
			{
			List<FullRailVehicleModel> railVehicleList= new List<FullRailVehicleModel>();
			var railVehicleListNode = consistNode.XPathSelectElements("RailVehicles/cOwnedEntity");

			foreach (var RailVehicleNode in railVehicleListNode)
				{
				railVehicleList.Add(AddRailVehicle(RailVehicleNode));
				}
			return railVehicleList;
			}

		public static FullRailVehicleModel AddRailVehicle(XElement RailVehicleNode)
			{
			var railVehicle = new FullRailVehicleModel();
			railVehicle.RailVehicleNode = RailVehicleNode;
			try
				{
				var ProviderNode = RailVehicleNode.XPathSelectElement("BlueprintID/iBlueprintLibrary-cAbsoluteBlueprintID/BlueprintSetID/iBlueprintLibrary-cBlueprintSetID/Provider");
				railVehicle.Provider = ProviderNode?.Value;
				var ProductNode = RailVehicleNode.XPathSelectElement("BlueprintID/iBlueprintLibrary-cAbsoluteBlueprintID/BlueprintSetID/iBlueprintLibrary-cBlueprintSetID/Product");
				if (ProductNode != null)
					{
					railVehicle.Product = ProductNode?.Value;
					}
				var BluePrintNode = RailVehicleNode.XPathSelectElement("BlueprintID/iBlueprintLibrary-cAbsoluteBlueprintID/BlueprintID");
				railVehicle.BlueprintPath = XmlHelpers.NormalizeBlueprint(BluePrintNode?.Value);
				var NameNode=RailVehicleNode.Element("Name");
				railVehicle.DisplayName = NameNode?.Value;
				}
			catch (Exception ex)
				{
				Log.Trace("Failed to add rail vehicle",ex, LogEventType.Error);
				}
			return railVehicle;
			}

		public static void GetAssetIdsForRailVehicles(FullRailVehicleModel rv)
			{
			if (rv.AssetId < 1)
				{
				var assetId =
					(int) AssetCollectionDataAccess.GetAssetIdFromDatabase(rv.Provider, rv.Product,
						rv.BlueprintPath);
				if (assetId >= 1)
					{
					rv.AssetId = assetId;
					}
				else
					{
					var providerProductId =
						ProviderProductCollectionDataAccess.InsertLooseProviderProduct(rv.Provider, rv.Product,"");
					rv.AssetId = (Int32)
						AssetCollectionDataAccess.InsertLooseAsset(providerProductId, rv.BlueprintPath);
					}
				}
			}

		public static async Task InsertRailVehiclesBulk(
			List<FullRailVehicleModel> fullRailVehicleList, int consistId)
			{
			using (IDbConnection connection =
				new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
				{
				connection.Open();
				using (IDbTransaction transaction = connection.BeginTransaction())
					{
					string sqlStatement =
						@$"INSERT OR IGNORE INTO RailVehicles (AssetId, DisplayName, CountryId, VehicleTypeId,GaugeId,OperatingCompanyId)
																		VALUES(@AssetId,@DisplayName,@Countryid,@VehicleTypeId,@GaugeId,@OperatingCompanyId);";
					try
						{
						var order = 0;
						foreach (var item in fullRailVehicleList)
							{
							if (item.AssetId == 0)
								{
								throw new EvaluateException($"AssetId must be valid for RailVehicle {item.DisplayName}");
								}
							await connection.ExecuteAsync(sqlStatement,
								new {item.AssetId,item.DisplayName,item.CountryId,item.VehicleTypeId,item.GaugeId,item.OperatingCompanyId }, transaction);

							int rvId = (int)connection.ExecuteScalar<long>("SELECT Id FROM RailVehicles WHERE RailVehicles.AssetId=@AssetId;", new { item.AssetId }, transaction);
							order++;
							await ConnectConsistRailVehicleDataAccess.InsertConnectConsistRailVehicle(consistId,
								rvId, order, transaction);
							}

						transaction.Commit();
						}
					catch (Exception e)
						{
						transaction.Rollback();
						Log.Trace($"Bulk save in database for assets failed, rolled back", e,
							LogEventType.Error);
						}
					}
				}
			}

		//RailVehicles.Id AS Id
		//, RailVehicles.DisplayName AS DisplayName 
		//, Assets.InGame AS InGame
		//, Assets.InArchive AS InArchive
		//, Assets.BluePrintPath AS BluePrintPath
		//, ProviderProducts.Provider AS Provider
		//, ProviderProducts.Product AS Product
		//, VehicleTypes.VehicleType AS VehicleType


		public static async Task<List<FullRailVehicleModel>> GetRequiredRailVehiclesForScenario(int scenarioId)
			{
			string sqlStatement =
				"SELECT DISTINCT * FROM FullRequiredRailVehiclesView WHERE ScenarioId= @ScenarioId;";
			var output= await AssetDatabaseAccess.LoadDataAsync<FullRailVehicleModel, dynamic>(sqlStatement,
				new {scenarioId}, AssetDatabaseAccess.GetConnectionString());
			return output;
			}
		}
	}
