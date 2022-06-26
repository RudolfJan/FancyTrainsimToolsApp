using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class SettingsViewModel : Screen
		{
		#region Properties
		private string _dataPath = Settings.DataPath;
		public string DataPath
			{
			get { return _dataPath; }
			set
				{
				_dataPath = value;
				NotifyOfPropertyChange(() => DataPath);
				}
			}

		private string _trainSimGamePath = Settings.TrainSimGamePath;

		public string TrainSimGamePath
			{
			get { return _trainSimGamePath; }
			set
				{
				_trainSimGamePath = value;
				NotifyOfPropertyChange(() => TrainSimGamePath);
				}
			}

		private string _trainSimArchivePath = Settings.TrainSimArchivePath;

		public string TrainSimArchivePath
			{
			get { return _trainSimArchivePath; }
			set
				{
				_trainSimArchivePath = value;
				NotifyOfPropertyChange(() => TrainSimArchivePath);
				}
			}

		private string _downloadFolder = Settings.DownloadFolder;

		public string DownloadFolder
			{
			get { return _downloadFolder; }
			set
				{
				_downloadFolder = value;
				NotifyOfPropertyChange(() => DownloadFolder);
				}
			}


		private string _textEditor = Settings.TextEditor;

		public string TextEditor
			{
			get { return _textEditor; }
			set
				{
				_textEditor = value;
				NotifyOfPropertyChange(() => TextEditor);
				}
			}

		private string _binEditor = Settings.BinEditor;

		public string BinEditor
			{
			get { return _binEditor; }
			set
				{
				_binEditor = value;
				NotifyOfPropertyChange(() => BinEditor);
				}
			}

		private string _sevenZip = Settings.SevenZip;

		public string SevenZip
			{
			get { return _sevenZip; }
			set
				{
				_sevenZip = value;
				NotifyOfPropertyChange(() => SevenZip);
				}
			}

		private string _installer = Settings.Installer;

		public string Installer
			{
			get { return _installer; }
			set
				{
				_installer = value;
				NotifyOfPropertyChange(() => Installer);
				}
			}

		private string _fileCompare = Settings.FileCompare;
		public string FileCompare
			{
			get { return _fileCompare; }
			set
				{
				_fileCompare = value;
				NotifyOfPropertyChange(() => FileCompare);
				}
			}

		private string _trainMapTool = Settings.TrainMapTool;
		private int windowedScreenResX;
		private int windowedScreenResY;
		private int fullScreenResX;
		private int fullScreenResY;
		private int borderlessScreenResX;
		private int borderlessScreenResY;

		public string TrainMapTool
			{
			get { return _trainMapTool; }
			set
				{
				_trainMapTool = value;
				NotifyOfPropertyChange(() => TrainMapTool);
				}
			}

		public int WindowedScreenResX
			{
			get { return windowedScreenResX; }
			set
				{
				windowedScreenResX = value;
				NotifyOfPropertyChange(() => WindowedScreenResX);
				}
			}
		public int WindowedScreenResY
			{
			get { return windowedScreenResY; }
			set
				{
				windowedScreenResY = value;
				NotifyOfPropertyChange(() => WindowedScreenResY);
				}
			}
		public int FullScreenResX
			{
			get { return fullScreenResX; }
			set
				{
				fullScreenResX = value;
				NotifyOfPropertyChange(() => FullScreenResX);
				}
			}
		public int FullScreenResY
			{
			get { return fullScreenResY; }
			set
				{
				fullScreenResY = value;
				NotifyOfPropertyChange(() => FullScreenResY);
				}
			}

		public int BorderlessScreenResX
			{
			get { return borderlessScreenResX; }
			set
				{
				borderlessScreenResX = value;
				NotifyOfPropertyChange(() => BorderlessScreenResX);
				}
			}
		public int BorderlessScreenResY
			{
			get { return borderlessScreenResY; }
			set
				{
				borderlessScreenResY = value;
				NotifyOfPropertyChange(() => borderlessScreenResY);
				}
			}

		private BindableCollection<VideoMode> _videoModesList;
		public BindableCollection<VideoMode> VideoModeList
			{
			get
				{
				return _videoModesList;
				}
			set
				{
				_videoModesList = value;
				}
			}

		private VideoMode _selectedVideoMode;

		public VideoMode SelectedVideoMode
			{
			get
				{
				return _selectedVideoMode;
				}
			set
				{
				_selectedVideoMode = value;
				NotifyOfPropertyChange(() => CanFullScreenRes);
				NotifyOfPropertyChange(() => CanWindowedScreenRes);
				NotifyOfPropertyChange(() => CanBorderlessScreenRes);
				}
			}

		private string _windowedScreenText;
		public string WindowedScreenText
			{
			get
				{
				return _windowedScreenText;
				}
			set
				{
				_windowedScreenText = value;
				NotifyOfPropertyChange(() => WindowedScreenText);
				}
			}

		private string _fullScreenText;
		public string FullScreenText
			{
			get
				{
				return _fullScreenText;
				}
			set
				{
				_fullScreenText = value;
				NotifyOfPropertyChange(() => FullScreenText);
				}
			}

		private string _borderlessScreenText;
		public string BorderlessScreenText
			{
			get
				{
				return _borderlessScreenText;
				}
			set
				{
				_borderlessScreenText = value;
				NotifyOfPropertyChange(() => BorderlessScreenText);
				}
			}

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
				NotifyOfPropertyChange(()=>CanEdit);
				NotifyOfPropertyChange(()=>CanDelete);
				}
			}

		private string _radioStationUrl;
		public string RadioStationUrl
			{
			get
				{
				return _radioStationUrl;
				}
			set
				{
				_radioStationUrl = value;
				NotifyOfPropertyChange(() => RadioStationUrl);
				NotifyOfPropertyChange(()=>CanSave);
				}
			}

		private string _radioStationName;
		public string RadioStationName
			{
			get
				{
				return _radioStationName;
				}
			set
				{
				_radioStationName = value;
				NotifyOfPropertyChange(() => RadioStationName);
				NotifyOfPropertyChange(()=>CanSave);
				}
			}

		private string _radioStationDescription;
		public string RadioStationDescription
			{
			get
				{
				return _radioStationDescription;
				}
			set
				{
				_radioStationDescription = value;
				NotifyOfPropertyChange(() => RadioStationDescription);
				}
			}

		public int RadioStationId { get; set; }
		#endregion

		protected override void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			VideoModeList = new BindableCollection<VideoMode>(VideoModes.ListVideoModes().OrderByDescending(x => x.VideoModeText));
			FullScreenResX = Settings.FullScreenResX;
			FullScreenResY = Settings.FullScreenResY;
			FullScreenText = VideoMode.GetModeText(FullScreenResX, FullScreenResY);
			WindowedScreenResX = Settings.WindowedScreenResX;
			WindowedScreenResY = Settings.WindowedScreenResY;
			WindowedScreenText = VideoMode.GetModeText(WindowedScreenResX, WindowedScreenResY);
			BorderlessScreenResX = Settings.BorderlessScreenResX;
			BorderlessScreenResY = Settings.BorderlessScreenResY;
			BorderlessScreenText = VideoMode.GetModeText(BorderlessScreenResX, BorderlessScreenResY);
			RadioStationList = new BindableCollection<RadioStationModel>(RadioStationDataAccess.GetAllRadioStations().OrderBy(x => x.RadioStationName));
			NotifyOfPropertyChange(() => VideoModeList);
			NotifyOfPropertyChange(() => RadioStationList);
			}

		public bool CanFullScreenRes
			{
			get
				{
				return SelectedVideoMode != null;
				}
			}

		public void FullScreenRes()
			{
			FullScreenText = SelectedVideoMode.VideoModeText;
			FullScreenResX = SelectedVideoMode.Width;
			FullScreenResY = SelectedVideoMode.Height;
			}

		public bool CanWindowedScreenRes
			{
			get
				{
				return SelectedVideoMode != null;
				}
			}

		public void WindowedScreenRes()
			{
			WindowedScreenText = SelectedVideoMode.VideoModeText;
			WindowedScreenResX = SelectedVideoMode.Width;
			WindowedScreenResY = SelectedVideoMode.Height;
			}

		public bool CanBorderlessScreenRes
			{
			get
				{
				return SelectedVideoMode != null;
				}
			}
		public void BorderlessScreenRes()
			{
			BorderlessScreenText = SelectedVideoMode.VideoModeText;
			BorderlessScreenResX = SelectedVideoMode.Width;
			BorderlessScreenResY = SelectedVideoMode.Height;
			}

		#region RadioStations


		public bool CanEdit
			{
			get
				{
				return SelectedRadioStation != null;
				}

			}
		public void Edit()
			{
			RadioStationName = SelectedRadioStation.RadioStationName;
			RadioStationUrl = SelectedRadioStation.RadioStationUrl;
			RadioStationDescription = SelectedRadioStation.RadioStationDescription;
			RadioStationId = SelectedRadioStation.Id;
			}


		public bool CanDelete
			{
			get
				{
				return SelectedRadioStation != null;
				}
			}

		public void Delete()
			{
			RadioStationDataAccess.DeleteRadioStation(SelectedRadioStation);
			RadioStationList.Remove(SelectedRadioStation);
			SelectedRadioStation = null;
			NotifyOfPropertyChange(()=>RadioStationList);
			NotifyOfPropertyChange(()=>SelectedRadioStation);
			}



		public bool CanSave
			{
			get
				{
				return RadioStationName?.Length>1&& RadioStationUrl?.Length>5;
				}
			}

		public void Save()
			{
			var newRadioStation= new RadioStationModel();
			newRadioStation.RadioStationName = RadioStationName;
			newRadioStation.RadioStationUrl = RadioStationUrl;
			newRadioStation.RadioStationDescription = RadioStationDescription;
			newRadioStation.Id = RadioStationId;
			if (RadioStationId == 0)
				{
				RadioStationDataAccess.CreateRadioStation(newRadioStation);
				}
			else
				{
				RadioStationDataAccess.UpdateRadioStation(newRadioStation);
				}

			Clear();
			SelectedRadioStation = null;
			RadioStationList = new BindableCollection<RadioStationModel>(RadioStationDataAccess.GetAllRadioStations().OrderBy(x => x.RadioStationName));
			NotifyOfPropertyChange(()=>RadioStationList);
			NotifyOfPropertyChange(()=>SelectedRadioStation);
			}

		public void Clear()
			{
			RadioStationName = "";
			RadioStationDescription = "";
			RadioStationUrl = "";
			RadioStationId = 0;
			NotifyOfPropertyChange(()=>CanSave);
			}
		#endregion

		public async Task Cancel() => await TryCloseAsync();

		public async Task SaveAndExit()
			{
			Settings.DataPath = AppendSlash(DataPath);
			Settings.TrainSimGamePath = AppendSlash(TrainSimGamePath);
			Settings.TrainSimArchivePath = AppendSlash(TrainSimArchivePath);
			Settings.TextEditor = TextEditor;
			Settings.BinEditor = BinEditor;
			Settings.SevenZip = SevenZip;
			Settings.Installer = Installer;
			Settings.FileCompare = FileCompare;
			Settings.TrainMapTool = TrainMapTool;
			Settings.DownloadFolder = AppendSlash(DownloadFolder);
			Settings.WindowedScreenResX = WindowedScreenResX;
			Settings.WindowedScreenResY = WindowedScreenResY;
			Settings.FullScreenResX = FullScreenResX;
			Settings.FullScreenResY = FullScreenResY;
			Settings.BorderlessScreenResX = BorderlessScreenResX;
			Settings.BorderlessScreenResY = BorderlessScreenResY;
			Settings.WriteToRegistry();
			SyncUpdates(); // update settings for other libraries
			await TryCloseAsync();
			}

		public static void SyncUpdates()
			{
			SevenZipDataAccess.SevenZipProgramLocation = Settings.SevenZip;
			}

		// Adds a backslash, for the convention that folders should end with a slash
		private string AppendSlash(string input)
			{
			if (input.EndsWith("\\"))
				{
				return input;
				}
			return $"{input}\\";
			}
		}
	}
