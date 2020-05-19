using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace FancyTrainsimTools.Desktop
  {

  // https://stackoverflow.com/questions/51351464/user-configuration-settings-in-net-core

  // See http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx

  public class Settings
    {
    #region Initialization

    private static readonly IConfiguration _config = CreateConfig();

    static IConfiguration CreateConfig()
      {
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");
      IConfiguration config = builder.Build();
      return config;
      }


    // static holder for instance, need to use lambda to construct since constructor private
    private static readonly Lazy<Settings> MyInstance
      = new Lazy<Settings>(() => new Settings());

    // private to prevent direct instantiation.

    private Settings()
      {
      }

    // accessor for instance

    public static Settings Instance
      {
      get
        {
        // ReadFromRegistry();
        return MyInstance.Value;
        }
      }

    public static string RegkeyString
      {
      get { return "software\\Holland Hiking\\FancytrainsimTools"; }
      }

    private static RegistryKey OpenRegistry()
      {
      return Registry.CurrentUser.CreateSubKey(RegkeyString, true);
      }


    #endregion

    #region DataAccess

    // User settings, a user may change go in the user settings.
    // Other configuration stuff goes in the appsettings.json file
    public static void ReadFromRegistry()
      {
      using var AppKey = OpenRegistry();
      DataPath = (string) AppKey.GetValue(nameof(DataPath), _config["DataConfig:DataPath"]);
      TrainSimGamePath = (string) AppKey.GetValue(nameof(TrainSimGamePath), _config["GamePath:TrainSimPath"]);
      TrainSimArchivePath = (string) AppKey.GetValue(nameof(TrainSimArchivePath), _config["GamePath:ArchivePath"]);
      TextEditor = (string) AppKey.GetValue(nameof(TextEditor), _config["Programs:TextEditor"]);
      BinEditor = (string) AppKey.GetValue(nameof(BinEditor), _config["Programs:BinEditor"]);
      SevenZip = (string) AppKey.GetValue(nameof(SevenZip), _config["Programs:SevenZip"]);
      }

    public static void  WriteToRegistry()
      {
      using var AppKey = OpenRegistry();
      AppKey.SetValue(nameof(DataPath), DataPath, RegistryValueKind.String);
      AppKey.SetValue(nameof(TrainSimGamePath), TrainSimGamePath, RegistryValueKind.String);
      AppKey.SetValue(nameof(TrainSimArchivePath), TrainSimArchivePath, RegistryValueKind.String);
      AppKey.SetValue(nameof(TextEditor), TextEditor, RegistryValueKind.String);
      AppKey.SetValue(nameof(BinEditor), BinEditor, RegistryValueKind.String);
      AppKey.SetValue(nameof(SevenZip), SevenZip, RegistryValueKind.String);
      }
    #endregion


    public static string DataPath { get; set; }

    public static string ManualFolder
      {
      get { return $"{DataPath}{_config["DataConfig:ManualPath"]}"; }
      }

    public static string AssetDatabasePath
      {
      get { return $"{DataPath}{_config["DataConfig:AssetDatabase"]}"; }
      }

    public static string ConnectionString
      {
      get
        {
        return _config["ConnectionStrings:SqLiteDb"].Replace("path", Settings.AssetDatabasePath);
        }
      }

    public static string TrainSimGamePath { get; set; }

    public static string TrainSimArchivePath { get; set; }

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

    #region appilications

    public static string TextEditor { get; set; }
    public static string BinEditor { get; set; }
    public static string SevenZip { get; set; }

    #endregion

    }
  }
