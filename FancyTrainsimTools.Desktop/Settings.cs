using Logging.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Syroot.Windows.IO;
using System;
using System.IO;

namespace FancyTrainsimToolsDesktop
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
      SteamGamePath= (string) AppKey.GetValue(nameof(SteamGamePath), _config["Programs:SteamGamePath"]);
      TextEditor = (string) AppKey.GetValue(nameof(TextEditor), _config["Programs:TextEditor"]);
      BinEditor = (string) AppKey.GetValue(nameof(BinEditor), _config["Programs:BinEditor"]);
      SevenZip = (string) AppKey.GetValue(nameof(SevenZip), _config["Programs:SevenZip"]);
      Installer = (string) AppKey.GetValue(nameof(Installer), _config["Programs:Installer"]);
      FileCompare = (string) AppKey.GetValue(nameof(FileCompare), _config["Programs:FileCompare"]);
      TrainMapTool = (string) AppKey.GetValue(nameof(TrainMapTool), _config["Programs:TrainMapTool"]);
      WindowedScreenResX= int.Parse((string)AppKey.GetValue(nameof(WindowedScreenResX), _config["Resolutions:WindowedScreenResX"]));
      WindowedScreenResY= int.Parse((string)AppKey.GetValue(nameof(WindowedScreenResY), _config["Resolutions:WindowedScreenResY"]));
      FullScreenResX= int.Parse((string) AppKey.GetValue(nameof(FullScreenResX), _config["Resolutions:FullScreenResX"]));
      FullScreenResY= int.Parse((string)AppKey.GetValue(nameof(FullScreenResY),  _config["Resolutions:FullScreenResY"]));
      BorderlessScreenResX= int.Parse((string)AppKey.GetValue(nameof(BorderlessScreenResX), _config["Resolutions:BorderlessScreenResX"]));
      BorderlessScreenResY= int.Parse((string)AppKey.GetValue(nameof(BorderlessScreenResY), _config["Resolutions:BorderlessScreenResY"]));

      // https://github.com/Syroot/KnownFolders
      DownloadFolder = (string) AppKey.GetValue(nameof(DownloadFolder), $"{KnownFolders.Downloads.Path}\\");
      // Create Directories, if needed
      try
        {
        Directory.CreateDirectory(TempFolder);
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot create temp folder\r\n{e.Message}", e, LogEventType.Error);
        }
      
      try
        {
        Directory.CreateDirectory(DownloadFolder);
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot create download archive\r\n{e.Message}", e, LogEventType.Error);
        }
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
      AppKey.SetValue(nameof(Installer), Installer, RegistryValueKind.String);
      AppKey.SetValue(nameof(FileCompare), FileCompare, RegistryValueKind.String);
      AppKey.SetValue(nameof(TrainMapTool), TrainMapTool, RegistryValueKind.String);
      AppKey.SetValue(nameof(DownloadFolder), DownloadFolder, RegistryValueKind.String);
      AppKey.SetValue(nameof(SteamGamePath),SteamGamePath, RegistryValueKind.String);

      AppKey.SetValue(nameof(WindowedScreenResX), WindowedScreenResX, RegistryValueKind.String);
      AppKey.SetValue(nameof(WindowedScreenResY), WindowedScreenResY, RegistryValueKind.String);
      AppKey.SetValue(nameof(FullScreenResX), FullScreenResX, RegistryValueKind.String);
      AppKey.SetValue(nameof(FullScreenResY), FullScreenResY, RegistryValueKind.String);
      AppKey.SetValue(nameof(BorderlessScreenResX), BorderlessScreenResX, RegistryValueKind.String);
      AppKey.SetValue(nameof(BorderlessScreenResY), BorderlessScreenResY, RegistryValueKind.String);
      }
    #endregion


    public static string DataPath { get; set; }

    public static string TempFolder
      {
      get { return $"{DataPath}Temp\\"; }
      }
    public static string ManualFolder
      {
      get { return $"{DataPath}{_config["DataConfig:ManualPath"]}"; }
      }

    public static string BackupPath
      {
      get { return $"{DataPath}{_config["DataConfig:BackupPath"]}"; }
      }

    public static string InstallersPath
      {
      get { return $"{DataPath}{_config["DataConfig:InstallersPath"]}"; }
      }

    public static string TemplatesPath
      {
      get { return $"{DataPath}{_config["DataConfig:TemplatesPath"]}"; }
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
    public static string SteamGamePath { get; set; }

    public static string SerzPath
      {
      get
        {
        return $"{TrainSimGamePath}\\serz.exe";
        }
      }

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

 


    public static string DownloadFolder { get; set; }
    #region Applications

    #region ScreenResolutions
    public static string TextEditor { get; set; }
    public static string BinEditor { get; set; }
    public static string SevenZip { get; set; }
		public static string Installer { get; set; }
    public static string FileCompare { get; set; }
    public static string TrainMapTool { get; set; }
		public static int WindowedScreenResX { get; set; }
		public static int WindowedScreenResY { get; set; }
		public static int FullScreenResX { get; set; }
		public static int FullScreenResY { get; set; }
		public static int BorderlessScreenResX { get; set; }
		public static int BorderlessScreenResY { get; set; }

    #endregion






		#endregion

		}
  }
