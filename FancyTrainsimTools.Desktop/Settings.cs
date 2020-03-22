using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace FancyTrainsimTools.Desktop
  {
  public class Settings
    {
    private static readonly IConfiguration _config = CreateConfig();
 
    static IConfiguration CreateConfig()
      {
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");
      IConfiguration config = builder.Build();
      return config;
      }

    public static string DataPath
      {
      get
        {
        return _config["DataConfig:DataPath"];
        }
      }

    public static void UpdateDataPath(string newValue)
      {
      throw  new NotImplementedException();
      }

    public static string ManualFolder
      {
      get { return $"{_config["DataConfig:DataPath"] + _config["DataConfig:ManualPath"]}"; }
      }

    public static string AssetDatabasePath
      {
      get
        {
        return $"{_config["DataConfig:DataPath"] + _config["DataConfig:AssetDatabase"]}";
        }
      }

    public static string ConnectionString
      {
      get
        {
        return _config["ConnectionStrings:SqLiteDb"].Replace("path",Settings.AssetDatabasePath);
        }
      }

    public static string TrainSimGamePath
      {
      get { return _config["GamePath:TrainSimPath"]; }
      }

    public static string TrainSimArchivePath
      {
      get { return _config["GamePath:ArchivePath"]; }
      }

    public static string GameAssetFolder
      {
      get { return $"{TrainSimGamePath}Assets\\"; }
      }

    public static string ArchiveAssetFolder
      {
      get { return $"{TrainSimArchivePath}Assets\\"; }
      }

    public static string GameRoutesFolder
      {
      get { return $"{TrainSimGamePath}Content\\Routes\\"; }
      }

    public static string ArchiveRoutesFolder
      {
      get { return $"{TrainSimArchivePath}Content\\Routes\\"; }
      }



      }
    }
