using Dapper;
using Logging.Library;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Assets.Library.Logic
	{
	public class ConnectConsistRailVehicleDataAccess
		{
		public static async Task InsertConnectConsistRailVehicle(int consistId, int railVehicleId,int order, IDbTransaction transaction)
			{
			string sql =
				"INSERT OR IGNORE INTO ConnectConsistRailVehicles (ConsistId, RailVehicleId, [Order]) VALUES(@Consistid,@RailVehicleId, @Order);";
			try
				{
				await transaction.Connection.ExecuteAsync(sql,new{consistId, railVehicleId, order}, transaction);
				}
			catch (Exception ex)
				{
				Log.Trace("Cannot connect RailVehicle to Consist", ex, LogEventType.Error);
				}
			}
		}
	}
