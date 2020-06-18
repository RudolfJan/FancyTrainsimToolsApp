using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using Logging.Library;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
  {
  public class PublishScenarioViewModel : Screen
		{
    #region Properties
		public ScenarioModel Scenario { get; set; }
    public RouteModel Route { get; set; }

    private String _InstallScriptDirectory = String.Empty;
    public String InstallScriptDirectory
      {
      get => _InstallScriptDirectory;
      set
        {
        _InstallScriptDirectory = value;
        }
      }

    /*
    Directory where all template files are kept
    */
    private String _InstallTemplates = String.Empty;
    public String InstallTemplates
      {
      get { return _InstallTemplates; }
      set
        {
        _InstallTemplates = value;
        }
      }

    /*
    Directory for output files for this specific installer
    */
    private String _InstallerDirectory = String.Empty;
    public String InstallerDirectory
      {
      get { return _InstallerDirectory; }
      set
        {
        _InstallerDirectory = value;
        }
      }

    /*
    Base directory for all generated installers
    */
    private String _InstallersOutputDirectory = String.Empty;
    public String InstallersOutputDirectory
      {
      get { return _InstallersOutputDirectory; }
      set
        {
        _InstallersOutputDirectory = value;
        }
      }

    private FileInfo _InstallerExecutableFile;
    public FileInfo InstallerExecutableFile
      {
      get => _InstallerExecutableFile;
      set
        {
        _InstallerExecutableFile = value;
        }
      }

    private String _InstallerOptions = String.Empty;
    public String InstallerOptions
      {
      get => _InstallerOptions;
      set
        {
        _InstallerOptions = value;
        }
      }

    private UInt32 _InstallerVersion = 1;
    public UInt32 InstallerVersion
      {
      get => _InstallerVersion;
      set
        {
        _InstallerVersion = value;
        }
      }

    private FileInfo _LicenseFile;
    public FileInfo LicenseFile
      {
      get => _LicenseFile;
      set
        {
        _LicenseFile = value;
        }
      }
    private FileInfo _ReadMeFile;
    public FileInfo ReadMeFile
      {
      get => _ReadMeFile;
      set
        {
        _ReadMeFile = value;
        }
      }

    private FileInfo _SetupImageFile;
    public FileInfo SetupImageFile
      {
      get => _SetupImageFile;
      set
        {
        _SetupImageFile = value;
        }
      }

    private FileInfo _ImageFile;
    public FileInfo ImageFile
      {
      get => _ImageFile;
      set
        {
        _ImageFile = value;
        }
      }

    private FileInfo _ManualFile;
    public FileInfo ManualFile
      {
      get => _ManualFile;
      set
        {
        _ManualFile = value;
        }
      }

    private Boolean _CreateZip;
    public Boolean CreateZip
      {
      get => _CreateZip;
      set
        {
        _CreateZip = value;
        }
      }

    private Boolean _CreateSimpleReadme;
    public Boolean CreateSimpleReadme
      {
      get => _CreateSimpleReadme;
      set
        {
        _CreateSimpleReadme = value;
        }
      }
    private Boolean _IncludeManual;
    public Boolean IncludeManual
      {
      get => _IncludeManual;
      set
        {
        _IncludeManual = value;
        }
      }

    private String _AuthorContactInfo = String.Empty;
    public String AuthorContactInfo
      {
      get => _AuthorContactInfo;
      set
        {
        _AuthorContactInfo = value;
        }
      }

    private FileTreeModel _tree;
    public FileTreeModel Tree
      {
      get
        {
        return _tree;
        }
      set { _tree = value; }
      }
 
    private string ScenarioPath; 

    #endregion

    #region Initialization
    public PublishScenarioViewModel()
      {
 
      }

    protected override void OnViewLoaded(object view)
      {
      base.OnViewLoaded(view);
      InstallScriptDirectory = Settings.InstallersPath;
      InstallTemplates = Settings.TemplatesPath;
      InstallerDirectory = InstallScriptDirectory + "Installers\\";
      InstallersOutputDirectory = InstallerDirectory + Route.RouteGuid + "\\" + Scenario.ScenarioGuid + "\\";
      ScenarioPath= $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\Scenarios\\{Scenario.ScenarioGuid}\\";
      Directory.CreateDirectory(InstallersOutputDirectory);
      if (!Directory.Exists(InstallersOutputDirectory))
        {
        Log.Trace("Failed to create installer output directory", LogEventType.Error);
        }
      if (Scenario.ScenarioProperties.Author!=null && Scenario.ScenarioProperties.Author.Trim().Length > 0)
        {
        InstallerExecutableFile = new FileInfo(InstallersOutputDirectory + (Scenario.ScenarioProperties.Author.Trim() + " " + Scenario.ScenarioTitle.Trim()) + ".exe");
        }
      else
        {
        InstallerExecutableFile = new FileInfo(InstallersOutputDirectory + Scenario.ScenarioTitle.Trim() + ".exe");
        }

      LicenseFile = new FileInfo(InstallTemplates + "License.txt");
      ReadMeFile = new FileInfo(InstallTemplates + "Readme.txt");
      SetupImageFile = new FileInfo(InstallTemplates + "Setup.bmp");
      if (File.Exists($"{ScenarioPath}Documentation.html"))
        {
        ManualFile = new FileInfo($"{ScenarioPath}Documentation.html");
        IncludeManual = true;
        }
      Tree = TreeBuilder.BuildTree(ScenarioPath);
      foreach (var item in Tree.TreeItems)
        {
        item.IsSelected = InitCheck(item.Name, false);
        }
      NotifyOfPropertyChange(()=>Tree.TreeItems);
      }

    #endregion

    #region CreateOutput

    public void MakeInstallScript()
      {
      if (CreateSimpleReadme)
        {
        BuildSimpleReadme();
        }
      BuildInstallScript();
      }

    public void CompileInstallScript()
      {
      CompileInstallerScript(
        InstallerExecutableFile.DirectoryName + "\\" +
        (Path.GetFileNameWithoutExtension(InstallerExecutableFile.Name)).Trim() +
        ".iss",
        InstallerOptions);

      if (CreateZip)
        {
        BuildZipFile();
        }
      }

    public void OpenInstallerOutput()
      {
      FileIOHelper.OpenFolder(InstallersOutputDirectory);
      }

    #endregion

    #region Logic

    private static bool InitCheck(string filename, bool always)
      {

      if (always)
        {
        return true;
        }
      filename = filename.ToLower();
      switch (filename)
        {
          case "scenarioproperties.xml":
          case "scenarioproperties.xml.md5":
          case "scenario.bin":
          case "scenario.bin.md5":
          case "initialsave.bin":
          case "initialsave.bin.md5":
          case "scenarionetworkproperties.bin":
          case "startingsave.bin":
          case "startingsave.bin.md5":
          case "scenarioscript.lua":
          case "scenarioscript.luac":
          case "scenarioscript.luac.md5":
            {
            return true;
            }
        }
      if (filename.EndsWith(".jpg"))
        {
        return true;
        }
      if (filename.EndsWith(".png"))
        {
        return true;
        }
      if (filename.EndsWith(".wav"))
        {
        return true;
        }
      if (filename.EndsWith(".iss"))
        {
        return true;
        }
      return false;
      }




    public static void CompileInstallerScript(String InputFile, String InstallerOptions = "")
      {
      using (Process Installer = new Process())
        {
        var InstallerName = Settings.Installer;
        if (!File.Exists(InstallerName))
          {
          Log.Trace("Inno Setup not found, expected " + InstallerName,
            LogEventType.Error);
          }

        try
          {
          Installer.StartInfo.FileName = InstallerName;
          Installer.StartInfo.Arguments = InstallerOptions + " \"" + InputFile + "\"";
          Installer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
          Installer.StartInfo.CreateNoWindow = true;
          Installer.StartInfo.UseShellExecute = false;
          Installer.StartInfo.RedirectStandardOutput = true;
          Installer.StartInfo.RedirectStandardError = true;
          Installer.Start();
          var R = Installer.StandardOutput.ReadToEnd();
          R += Installer.StandardError.ReadToEnd();
          Installer.WaitForExit();
          Log.Trace(R + "\r\nInstaller created " + InstallerName);
          }
        catch (Exception E)
          {
          Log.Trace("Error creating installer " + E.Message, LogEventType.Error);
          }
        }
      }

    public void BuildSimpleReadme()
      {
      var Text = "This pack contains the scenario " + Scenario.ScenarioTitle + " for route " + Route.RouteName + "\r\n";
      if (Scenario.ScenarioProperties.Author.Length > 0)
        {
        Text += "This scenario is created by " + Scenario.ScenarioProperties.Author + "\r\n\r\n";
        }

      if (AuthorContactInfo.Length > 0)
        {
        Text += AuthorContactInfo + "\r\n\r\n";
        }

      Text += "Description:\r\n";
      Text += Scenario.ScenarioProperties.Description + "\r\n";

      Text += "Duration:" + Scenario.ScenarioProperties.Duration + " Rating: " + Scenario.ScenarioProperties.Rating + " Scenario type: " + Scenario.ScenarioClass + "\r\n\r\n";
			// Now try to get a rolling stock list

			// TODO get rollingstock list and instructions and print them
			if (Scenario.ScenarioProperties.RequiredRailVehicles == null)
				{
				
				}
			Text += ConsistDataAccess.PrintRollingStockList(Scenario.ScenarioProperties.RequiredRailVehicles);


			// Now write the text to the Output Readme file
			try
        {
        ReadMeFile = new FileInfo(InstallersOutputDirectory + "Readme.txt");
        using var Writer = new StreamWriter(ReadMeFile.FullName, false);
        Writer.WriteLine(Text);
        Writer.Close();
        }
      catch (Exception E)
        {
        Log.Trace("Failed to readme file " + ReadMeFile.FullName + "because " + E.Message, LogEventType.Error);
        }
      }


    #region BuildInstallSections

    private String BuildHeader()
      {
      var AppName = InstallerExecutableFile.Name.Substring(0, InstallerExecutableFile.Name.Length - 4);
      var Text = "#define MyAppName \"" + AppName + "\"\n";
      Text += "#define MyAppVersion \"" + InstallerVersion + "\"\n";
      Text += "#define MyAppPublisher \"\"\n";
      Text += "#define MyAppURL \"\"\n";
      Text += "#define MyOutputDir \"" + InstallersOutputDirectory + "\"\n";
      Text += "#define MessageFile \"" + InstallTemplates + "InstallerSetup.isl\"\n";
      Text += "#define ImageFile \"" + SetupImageFile.FullName + "\"\n";
      Text += "#define MyLicense \"" + LicenseFile.FullName + "\"\n\n";
      Text += "#define MyDefaultInstallDir = \"C:\\Program Files (x86)\\Steam\"\n\n";
      Text += "[Setup]\n";
      Text += "; NOTE: The value of AppId uniquely identifies this application.\n";
      Text += "; Do not use the same AppId value in installers for other applications.\n";
      Text += "AppId={{" + Guid.NewGuid() + "}\n";
      return Text;
      }

    private String BuildIntro()
      {
      var Text =
@"AppName ={#MyAppName}
AppVersion ={#MyAppVersion}
;AppVerName ={#MyAppName} {#MyAppVersion}
AppPublisher ={#MyAppPublisher}
AppPublisherURL ={#MyAppURL}
AppSupportURL ={#MyAppURL}
AppUpdatesURL ={#MyAppURL}
DefaultDirName = ""{code:GetInstallDir}\SteamApps\common\RailWorks""
DefaultGroupName ={#MyAppName}
LicenseFile = {#MyLicense}
InfoBeforeFile =
InfoAfterFile =
OutputDir = {#MyOutputDir}
OutputBaseFilename = {#MyAppName}
Compression = lzma
SolidCompression = yes
EnableDirDoesntExistWarning = True
DirExistsWarning = no
CloseApplications = False
RestartApplications = False
CreateUninstallRegKey = no
PrivilegesRequired=none
DisableWelcomePage=no
WizardImageFile= {#ImageFile}   
[Languages]
Name: ""english""; MessagesFile: {#MessageFile}

[Code]
function GetInstallDir(def: string): string;
var
InstallDir : string;
begin
  Result := 'C:\\Program Files (x86)\\Steam';
  if RegQueryStringValue(HKEY_CURRENT_USER, 'Software\Valve\Steam', 'SteamPath', InstallDir) then
  begin
    // Successfully read the value.
    Result := InstallDir;
  end;
end;

[Files]
";
      if (File.Exists(ReadMeFile.FullName))
        {
        Text += "Source: \"" + ReadMeFile.FullName + "\"; DestDir: \"{app}\\Readme.txt\"; Flags: ignoreversion\n";
        }
      return Text;
      }


    private String BuildFooter()
      {
      var Text = "; NOTE: Don't use Flags: ignoreversion on any shared system files\n\n";

      Text += "[Icons]\n";
      Text += "Name: \"{group}\\{cm:UninstallProgram,{#MyAppName}}\"; Filename:\"{uninstallexe}\"\n\n";
      return Text;
      }


    // Call the procedure using the TreeView.
    private String CallRecursive()
      {
      var Text = String.Empty;
      foreach (var Item in Tree.TreeItems)
        {
        if (Item.IsSelected)
          {
          if (Item.GetType().Name == "DirectoryItem")
            {
            Text += CallRecursive((DirectoryItem)Item, Text);
            }
          else
            {
            var DestDir = Item.Path;
            if (Item.Path.Length > 0)
              {
              // strip filename
              DestDir = DestDir.Substring(0, DestDir.Length - Item.Name.Length);
              // Strip off path base
              DestDir = DestDir.Substring(Settings.TrainSimGamePath.Length);
              }

            Text += "Source: \"" + Item.Path + "\"; DestDir: \"{app}\\" + DestDir + "\"; Flags: ignoreversion\r\n";
            }
          }
        }
      return Text;
      }

    private String CallRecursive(DirectoryItem DirItem, String Text)
      {
      foreach (var Item in DirItem.DirectoryItems)
        {
        if (Item.IsSelected)
          {
          if (Item.GetType().Name == "DirectoryItem")
            {
            Text += CallRecursive((DirectoryItem)Item, Text);
            }
          else
            {
            var DestDir = Item.Path;
            if (Item.Path.Length > 0)
              {
              // strip filename
              DestDir = DestDir.Substring(0, DestDir.Length - Item.Name.Length);
              // Strip off path base
              DestDir = DestDir.Substring(Settings.TrainSimGamePath.Length);
              }
            Text += "Source: \"" + Item.Path + "\"; DestDir: \"{app}\\" + DestDir + "\"; Flags: ignoreversion\r\n";
            }
          }
        }
      return Text;
      }

    public void BuildInstallScript()
      {
      // optional: create readme


      var Text = BuildHeader();
      Text += BuildIntro();
      Text += CallRecursive();
      Text += BuildFooter();

      // Create output stream

      var OutputFile = InstallerExecutableFile.FullName.Substring(0, InstallerExecutableFile.FullName.Length - 4) + ".iss";
      using var Sw = new StreamWriter(OutputFile, false);
      Sw.WriteLine(Text);
      Sw.Close();
      }
    #endregion

    private void AddFileToArchive(FileInfo FilePath, ZipArchive Archive)
      {
      if (FilePath != null && File.Exists(FilePath.FullName))
        {
        Archive.CreateEntryFromFile(FilePath.FullName, FilePath.Name);
        }
      else
        {
        if (FilePath != null) Log.Trace("Failed to add " + FilePath.FullName + " to Zip archive\r\n");
        }
      }

    public void BuildZipFile()
      {
      var ZipFileName = InstallersOutputDirectory + InstallerExecutableFile.Name.Substring(0, InstallerExecutableFile.Name.Length - 4) + ".zip";
      FileIOHelper.DeleteSingleFile(ZipFileName);
      try
        {
        using (ZipArchive Archive = ZipFile.Open(ZipFileName, ZipArchiveMode.Update))
          {
          AddFileToArchive(InstallerExecutableFile, Archive);
          AddFileToArchive(ReadMeFile, Archive);
          AddFileToArchive(ImageFile, Archive);
          AddFileToArchive(ManualFile, Archive);
          }
        }
      catch (Exception E)
        {
        Log.Trace("Exception when creating Zip file. " + E.Message, LogEventType.Error);
        }
      }
    #endregion

    #region Housekeeping

    public async Task Exit()
      {
      await TryCloseAsync();
      }

    #endregion
    }
	}

