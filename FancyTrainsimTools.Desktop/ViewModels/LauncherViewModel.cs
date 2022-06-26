using Assets.Library.Helpers;
using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class LauncherViewModel:Screen
		{
		    #region Properties

    private bool _LogMate = false;
    private bool _DebugScripting = false;
    private bool _ShowControlStates = false;
    private bool _Asynckeys = false;
    private bool _ShowDriverList = false;
    private bool _FollowAiTrain = false;
    private bool _SetFov = false;
    private int _Fov = 70;
    private bool _FullScreen = true;
    private bool _BorderlessScreen = false;
    private bool _WindowedScreen = false;
    private bool _StartTrainMapsTool = false;
    private bool _StartRadioTool = false;
    private string _AdditionalCommands = string.Empty;
    private string _Result = string.Empty;
    private BindableCollection<RadioStationModel> _radioStationList;

    public BindableCollection<RadioStationModel> RadioStationList
      {
      get
        {
        return _radioStationList;
        }
      set
        {
        _radioStationList = value;
        }
      }

    private RadioStationModel _selectedRadioStation;

    public RadioStationModel SelectedRadioStation
      {
      get
        {
        return _selectedRadioStation;
        }
      set
        {
        _selectedRadioStation = value;
        }
      }
    public bool LogMate
      {
      get => _LogMate;
      set
        {
        _LogMate = value;
        NotifyOfPropertyChange(()=>LogMate);
        }
      }

    public bool DebugScripting
      {
      get => _DebugScripting;
      set 
        {
        _DebugScripting = value;
       NotifyOfPropertyChange(()=>DebugScripting);
        }
      }

    public bool Asynckeys
      {
      get => _Asynckeys;
      set
        {
        _Asynckeys = value; 
        NotifyOfPropertyChange(()=>Asynckeys);
        }
      }

    public bool ShowControlStates
      {
      get => _ShowControlStates;
      set 
        { _ShowControlStates = value; 
        NotifyOfPropertyChange(()=>ShowControlStates);
        }
      }

    public bool ShowDriverList
      {
      get => _ShowDriverList;
      set 
        { _ShowDriverList = value; 
        NotifyOfPropertyChange(()=> ShowDriverList);
        }
      }

    public bool FollowAiTrain
      {
      get => _FollowAiTrain;
      set 
        { _FollowAiTrain = value; 
        NotifyOfPropertyChange(()=>FollowAiTrain);
        }
      }

    public bool SetFov
      {
      get => _SetFov;
      set 
        { _SetFov = value; 
        NotifyOfPropertyChange(()=>SetFov);
        }
      }

    public int Fov
      {
      get => _Fov;
      set 
        { _Fov = value; 
        NotifyOfPropertyChange(()=>Fov);
        }
      }

    public bool FullScreen
      {
      get => _FullScreen;
      set 
        { _FullScreen = value; 
        NotifyOfPropertyChange(()=>FullScreen);
        }
      }

    public bool BorderlessScreen
      {
      get => _BorderlessScreen;
      set
        {
        _BorderlessScreen = value; 
        NotifyOfPropertyChange(()=>BorderlessScreen);
        }
      }

    public bool WindowedScreen
      {
      get => _WindowedScreen;
      set 
        {
        _WindowedScreen = value; 
        NotifyOfPropertyChange(()=>WindowedScreen);
        }
      }

    public bool StartTrainMapsTool
      {
      get => _StartTrainMapsTool;
      set 
        { _StartTrainMapsTool = value; 
        NotifyOfPropertyChange(()=>StartTrainMapsTool);
        }
      }

    public bool StartRadioTool
      {
      get => _StartRadioTool;
      set 
        { _StartRadioTool = value; 
        NotifyOfPropertyChange(()=>StartRadioTool);
        }
      }

    public string AdditionalCommands
      {
      get => _AdditionalCommands;
      set 
        { _AdditionalCommands = value; 
        NotifyOfPropertyChange(()=>AdditionalCommands);
        }
      }

    public string Result
      {
      get => _Result;
      set
        {
        _Result = value;
        NotifyOfPropertyChange(()=>Result);
        }
      }

    #endregion Properties

#region Initialization

    protected override void OnViewLoaded(object view)
      {
      base.OnViewLoaded(view);
      RadioStationList = new BindableCollection<RadioStationModel>(RadioStationDataAccess.GetAllRadioStations().OrderBy(x => x.RadioStationName));
      NotifyOfPropertyChange(() => RadioStationList);
      }

    #endregion

#region Logic






    public String RunTrainMapsLive()
      {
      using var TrainMapsLive = new Process();
      try
        {
        TrainMapsLive.StartInfo.FileName = Settings.TrainMapTool;
        TrainMapsLive.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        TrainMapsLive.StartInfo.Verb = "runas"; // need to run as admin
        TrainMapsLive.StartInfo.Arguments = "";
        TrainMapsLive.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        TrainMapsLive.StartInfo.CreateNoWindow = false;
        TrainMapsLive.StartInfo.UseShellExecute = true;
        TrainMapsLive.Start();
        }
      catch (Exception E)
        {
        Result += Log.Trace("Error starting TrainmapsLive.exe ", E, LogEventType.Error);
        return String.Empty;
        }
      return String.Empty;
      }

    public String SetScreenMode()
      {
      var ScreenResX = 1280;
      var ScreenResY = 1024;
      var ScreenMode = 1;
      var ScreenModeText = "Full screen";

      if (WindowedScreen)
        {
        ScreenResX = Settings.WindowedScreenResX;
        ScreenResY = Settings.WindowedScreenResY;
        ScreenMode = 0; // borderless uses 2, windowed uses value 0
        ScreenModeText = "Windowed screen";
        }
      if (FullScreen)
        {
        ScreenResX = Settings.FullScreenResX;
        ScreenResY = Settings.FullScreenResY;
        ScreenMode = 1;
        ScreenModeText = "Full screen";
        }
      if (BorderlessScreen)
        {
        ScreenResX = Settings.BorderlessScreenResX;
        ScreenResY = Settings.BorderlessScreenResY;
        ScreenMode = 2;
        ScreenModeText = "Borderless screen";
        }

      var FilePath = Settings.TrainSimGamePath + "Content\\PlayerProfiles.bin";
      var TempPath1 = Settings.TempFolder + "PlayerProfiles.xml";
      var TempPath2 = Settings.TempFolder + "PlayerProfiles2.xml";
      try
        {


        // need to decompress first
        FileIOHelper.DecodeSerz(FilePath, TempPath1, out bool Success);
        if (Success)
          {
          var Doc = XDocument.Load(TempPath1);

          var ScreenResXNode = Doc.XPathSelectElement("cRecordSet/Record/cPlayerProfile/ScreenResX");
          var ScreenResYNode = Doc.XPathSelectElement("cRecordSet/Record/cPlayerProfile/ScreenResY");
          var FullScreenNode = Doc.XPathSelectElement("cRecordSet/Record/cPlayerProfile/FullScreen");
          ScreenResXNode.SetValue(ScreenResX);
          ScreenResYNode.SetValue(ScreenResY);
          FullScreenNode.SetValue(ScreenMode);
          XmlHelpers.Save(Doc, TempPath2);

          FileIOHelper.PackToBinSerz(FilePath, TempPath2, out Success);
          _Result += Log.Trace("Screen mode set to " + ScreenModeText + " " + ScreenResX.ToString() + "x" + ScreenResY.ToString());
          }

        }
      catch (Exception E)
        {
        return _Result += Log.Trace("Cannot set screen resolution exception", E,
          LogEventType.Error);
        }
      return _Result;
      }

    public string BuildCommandLine()
      {
      // Build argument list
      var Arguments = String.Empty;
      string Command;
      if (LogMate)
        {
        Arguments += "-LogMate ";
        if (DebugScripting)
          {
          Command = "-SetLogFilters=\"Script Manager\" -lua-debug-messages ";
          Arguments += Command;
          }
        else
          {
          Command = "-SetLogFilters=\"All\" ";
          Arguments += Command;
          }
        }
      if (ShowControlStates)
        {
        Command = "-ShowControlStateDialog ";
        Arguments += Command;
        }
      if (Asynckeys)
        {
        Command = "-EnableAsyncKeys ";
        Arguments += Command;
        }
      if (ShowDriverList)
        {
        Command = "-ShowDriverList ";
        Arguments += Command;
        }
      if (FollowAiTrain)
        {
        Command = "-followaitrain ";
        Arguments += Command;
        }
      if (SetFov)
        {
        Command = "-SetFOV=" + Fov + " ";
        Arguments += Command;
        }

      if (AdditionalCommands.Length > 0)
        {
        Command = AdditionalCommands;
        Arguments += Command;
        }
      return Arguments;
      }


    public void Launch()
      {
      if (StartRadioTool && SelectedRadioStation != null)
        {
        FileIOHelper.OpenFileWithShell(SelectedRadioStation.RadioStationUrl);
        }
      if (StartTrainMapsTool && Settings.TrainMapTool.Length > 5)
        {
        RunTrainMapTool();
        }
      RunTrainSimulator();
      }


    public String RunTrainMapTool()
      {
      using var TrainMapsLive = new Process();
      try
        {
        TrainMapsLive.StartInfo.FileName = Settings.TrainMapTool;
        TrainMapsLive.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        TrainMapsLive.StartInfo.Verb = "runas"; // need to run as admin
        TrainMapsLive.StartInfo.Arguments = "";
        TrainMapsLive.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        TrainMapsLive.StartInfo.CreateNoWindow = false;
        TrainMapsLive.StartInfo.UseShellExecute = true;
        TrainMapsLive.Start();
        }
      catch (Exception E)
        {
        Result += Log.Trace("Error starting Trainmap Tool.exe ", E, LogEventType.Error);
        return String.Empty;
        }
      return String.Empty;
      }

    public void RunTrainSimulator()
      {
      Result += SetScreenMode();
      var Arguments = BuildCommandLine();
      Result = Arguments;
 

      using var Railworks = new Process();
      try
        {
				Railworks.StartInfo.FileName = Settings.SteamGamePath;
				Railworks.StartInfo.Arguments = $"steam://rungameid/24010 {Arguments}";
				Railworks.StartInfo.CreateNoWindow = false;
				Railworks.StartInfo.UseShellExecute = false;
				Railworks.StartInfo.RedirectStandardOutput = false;
				Railworks.StartInfo.RedirectStandardError = false;
				Railworks.Start();
	      }
			catch (Exception E)
        {
        Result += Log.Trace("Error starting Railworks.exe ", E, LogEventType.Error);
        return;
        }
      }
    #endregion

		public async Task Exit()
			{
			await TryCloseAsync();
			}
		}
	}
