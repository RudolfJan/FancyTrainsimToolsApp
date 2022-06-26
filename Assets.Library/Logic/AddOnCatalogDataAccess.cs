using Assets.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Assets.Library.Logic
	{
	public class AddOnCatalogDataAccess
		{
		public static async Task<List<AddOnCatalogModel>> GetAllAddOns()
			{
			string sql="SELECT * FROM AddOnCatalog;";
			return await AssetDatabaseAccess.LoadDataAsync<AddOnCatalogModel,dynamic>(sql, new { },
				AssetDatabaseAccess.GetConnectionString());
			}

		public static async Task<AddOnCatalogModel> GetAddOnCatalogItem(int AddOnCatalogId)
			{
			string sql="SELECT * FROM AddOnCatalog WHERE Id=@Id;";
			var addOn = await AssetDatabaseAccess.LoadDataAsync<AddOnCatalogModel, dynamic>(sql,
				new {Id = AddOnCatalogId}, AssetDatabaseAccess.GetConnectionString());
			return addOn.FirstOrDefault();
			}


		public static void InsertAddOnCatalogItem(AddOnCatalogModel addOn)
			{
			string sql = "INSERT OR IGNORE INTO AddOnCatalog (Name, Location, ArchiveName, Description, AddOnType, IsPayware) VALUES (@Name, @Location, @ArchiveName, @Description, @AddOnType, @IsPayware);";
			AssetDatabaseAccess.SaveData(sql,new{addOn.Name, addOn.Location, addOn.ArchiveName, addOn.Description, addOn.AddOnType, addOn.IsPayware}, AssetDatabaseAccess.GetConnectionString());
			}

		public static  void UpdateAddOnCatalogItem(AddOnCatalogModel addOn)
			{
			string sql = "UPDATE OR IGNORE AddOnCatalog SET Name=@Name, Location=@Location, ArchiveName=@ArchiveName, Description=@Description, AddOnType=@AddOnType, IsPayware=@IsPayware WHERE Id=@Id;";
			AssetDatabaseAccess.SaveData(sql,new{addOn.Id, addOn.Name, addOn.Location, addOn.ArchiveName, addOn.Description, addOn.AddOnType, addOn.IsPayware}, AssetDatabaseAccess.GetConnectionString());
			}

		public static void DeleteAddOnCatalogItem(AddOnCatalogModel addOn)
			{
			string sql = "DELETE FROM AddOnCatalog WHERE Id=@Id;";
			AssetDatabaseAccess.SaveData(sql,new{addOn.Id}, AssetDatabaseAccess.GetConnectionString());
			}

		}
	}
