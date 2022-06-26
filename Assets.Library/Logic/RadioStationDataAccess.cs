using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Assets.Library.Logic
	{
	public class RadioStationDataAccess
		{
		public static List<RadioStationModel> GetAllRadioStations()
			{
			var sql= "SELECT * FROM [RadioStations];";
			return AssetDatabaseAccess.LoadData<RadioStationModel, dynamic>(sql, new { }, AssetDatabaseAccess.GetConnectionString());
			}

		public static void CreateRadioStation(RadioStationModel radioStation)
			{
			var sql =
				"INSERT OR IGNORE INTO [RadioStations] (RadioStationUrl, RadioStationName, RadioStationDescription)" +
				"VALUES (@RadioStationUrl, @RadioStationName, @RadioStationDescription);";
			AssetDatabaseAccess.SaveData(sql,new{radioStation.RadioStationUrl, radioStation.RadioStationName, radioStation.RadioStationDescription},AssetDatabaseAccess.GetConnectionString());
			}

		public static void UpdateRadioStation(RadioStationModel radioStation)
			{
			var sql =
				"UPDATE OR IGNORE[RadioStations] SET RadioStationUrl=@RadioStationUrl, RadioStationName=@RadioStationName, RadioStationDescription=@RadioStationDescription WHERE Id=@Id";
			AssetDatabaseAccess.SaveData(sql,new{radioStation.RadioStationUrl, radioStation.RadioStationName, radioStation.RadioStationDescription,radioStation.Id},AssetDatabaseAccess.GetConnectionString());
			}

		public static void DeleteRadioStation(RadioStationModel radioStation)
			{
			var sql =
				"DELETE FROM [RadioStations] WHERE Id=@Id;";
			AssetDatabaseAccess.SaveData(sql,new{radioStation.Id},AssetDatabaseAccess.GetConnectionString());
			}
		}
	}
