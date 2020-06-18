using Assets.Library.Helpers;
using Assets.Library.Models;
using Dapper;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Assets.Library.Logic
	{
	public class ConsistDataAccess
		{
		public static List<ConsistModel> GetAllConsistsForScenario(int scenarioId, XDocument Doc)
			{
			List<ConsistModel> consistList = new List<ConsistModel>();
			try
				{
				var ConsistNodeList = Doc.XPathSelectElements("cRecordSet/Record/cConsist");

				foreach (var ConsistNode in ConsistNodeList)
					{
					consistList.Add(CreateConsist(scenarioId, ConsistNode));
					}

				return consistList;
				}
			catch (Exception ex)
				{
				Log.Trace("Get consists failed", ex, LogEventType.Error);
				}

			return consistList;
			}

		public static ConsistModel CreateConsist(int scenarioId, XElement consistNode)
			{
			var output = new ConsistModel();
			try
				{
				output.ScenarioId = scenarioId;
				output.ConsistNode = consistNode;
				output.IsLoose = false;
				output.IsEmpty = false;
				output.IsValid = true;

				// Try to find consist name
				var ConsistDisplayNameNode =
					consistNode.XPathSelectElement(
						"Driver/cDriver/ServiceName/Localisation-cUserLocalisedString");
				if (ConsistDisplayNameNode != null)
					{
					output.ConsistName =
						Converters.GetLocalisedString(ConsistDisplayNameNode, @"Loose Consist");
					}
				else
					{
					output.ConsistName = "Loose consist";
					output.IsLoose = true;
					}

				var PlayerConsistNode = consistNode.XPathSelectElement("Driver/cDriver/PlayerDriver");
				output.IsPlayer = (PlayerConsistNode?.Value == "1");

				var ServiceClassNode = consistNode.XPathSelectElement("Driver/cDriver/ServiceClass");
				if (ServiceClassNode != null)
					{
					var result = int.TryParse(ServiceClassNode.Value, out int value);
					if (result)
						{
						output.ServiceClass = value;
						}
					else
						{
						output.ServiceClass = -1; // Not valid
						}
					}

				var StartTimeNode = consistNode.XPathSelectElement("Driver/cDriver/StartTime");
				if (StartTimeNode != null)
					{
					var result = int.TryParse(StartTimeNode.Value, out int value);
					if (result)
						{
						output.StartTime = value;
						}
					else
						{
						output.StartTime = 0; // Not valid
						}
					}

				var DestinationNode =
					consistNode.XPathSelectElement(
						"Driver/cDriver/FinalDestination/cDriverInstructionTarget/DisplayName");
				output.Destination = DestinationNode?.Value??"";
				output.RailVehicleList = FullRailVehicleDataAccess.GetRailVehiclesForConsist(consistNode);
				output.IsEmpty = (output.RailVehicleList.Count == 0);
				if (output.IsEmpty)
					{
					output.IsLoose = false;
					output.ConsistName = "Empty Consist";
					}

				if (!(output.IsEmpty || output.IsLoose))
					{
					output.InstructionList =
						InstructionsDataAccess.GetInstructionsForConsist(consistNode, output.StartTime);
					}

				return output;
				}

			catch (Exception ex)
				{
				output.IsValid = false;
				Log.Trace($"Failed to create consist ", ex, LogEventType.Error);
				return output;
				}
			}

		public static async Task<List<ConsistModel>> GetAllConsistsForScenarioFromDb(int scenarioId)
			{
			string sql = "SELECT * FROM Consists WHERE Consists.ScenarioId=@ScenarioId";
			return await AssetDatabaseAccess.LoadDataAsync<ConsistModel, dynamic>(sql, new {scenarioId},
				AssetDatabaseAccess.GetConnectionString());
			}

		public static int GetConsistCountForScenarioFromDb(int scenarioId)
			{
			string sql = "SELECT Count() FROM Consists WHERE Consists.ScenarioId=@ScenarioId";
			return AssetDatabaseAccess.LoadData<int, dynamic>(sql, new {scenarioId},
				AssetDatabaseAccess.GetConnectionString()).FirstOrDefault();
			}

		public static void BulkSaveConsistsPerScenario(List<ConsistModel> consistList)
			{
			using (IDbConnection connection =
				new SQLiteConnection(AssetDatabaseAccess.GetConnectionString()))
				{
				connection.Open();
				using (IDbTransaction transaction = connection.BeginTransaction())
					{
					string sqlStatement =
						@$"INSERT OR IGNORE INTO Consists (ConsistName, ScenarioId, Destination, StartTime, ServiceClass, IsPlayer, IsLoose, IsEmpty, IsValid) 
																		VALUES(@ConsistName, @ScenarioId, @Destination, @StartTime, @ServiceClass, @IsPlayer, @IsLoose, @IsEmpty, @IsValid); ";
					try
						{
						foreach (var item in consistList)
							{
							connection.Execute(sqlStatement,
								new
									{
									item.ConsistName, item.ScenarioId, item.IsPlayer, item.IsLoose, item.IsEmpty,
									item.IsValid, item.Destination, item.StartTime, item.ServiceClass
									},
								transaction);
							var lastRow = connection.ExecuteScalar("SELECT last_insert_rowid();",new{},transaction);
							item.Id = (int)(long) lastRow;
							}
						transaction.Commit();
						}
					catch (Exception e)
						{
						transaction.Rollback();
						Log.Trace($"Bulk save in database for assets failed, rolled back", e, LogEventType.Error);
						}
					}
				}
			}

		// Save consists, rolling stock and eventually Instructions to database ...
		public static async Task SaveConsistsToDatabaseMaster(List<ConsistModel> consistList)
			{
			BulkSaveConsistsPerScenario(consistList);
			foreach (var item in consistList)
				{
				
				if (item.RailVehicleList.Count > 0)
					{
					
					// Get all asset id's
					foreach (var rv in item.RailVehicleList)
						{
						// Get Asset it, create asset if it is missing
						FullRailVehicleDataAccess.GetAssetIdsForRailVehicles(rv);
						}
					await FullRailVehicleDataAccess.InsertRailVehiclesBulk(item.RailVehicleList, item.Id);
					}
				}
			}
	
		public static string PrintRollingStockList(List<FullRailVehicleModel> requiredRailVehicles)
			{
			var output = string.Empty;
			foreach (var item in requiredRailVehicles.OrderBy(x=>x.Provider).ThenBy(x=>x.Product).ThenBy(x=>x.BlueprintPath))
				{
				output+= $"{item.DisplayName},{item.Provider},{item.Product},{item.BlueprintPath}\r\n";
				}
			return output;
			}

		public static  List<InstructionModel> GetPlayerInstructions(List<ConsistModel> consistList)
			{
			return consistList.First(x => x.IsPlayer == true).InstructionList;
			}
		}
	}

