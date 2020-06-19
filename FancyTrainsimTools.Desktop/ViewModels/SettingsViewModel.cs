using Caliburn.Micro;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class SettingsViewModel: Screen
		{
		#region Properties
		private string _dataPath = Settings.DataPath;
		public string DataPath
			{
			get { return _dataPath; }
			set
				{
				_dataPath = value;
				NotifyOfPropertyChange(()=>DataPath);
				}
			}

		private string _trainSimGamePath = Settings.TrainSimGamePath;	

		public string TrainSimGamePath
			{
			get { return _trainSimGamePath; }
			set
				{
				_trainSimGamePath = value; 
				NotifyOfPropertyChange(()=>TrainSimGamePath);
				}
			}

		private string _trainSimArchivePath = Settings.TrainSimArchivePath;

		public string TrainSimArchivePath
			{
			get { return _trainSimArchivePath; }
			set
				{
				_trainSimArchivePath = value; 
				NotifyOfPropertyChange(()=>TrainSimArchivePath);
				}
			}

		private string _textEditor = Settings.TextEditor;

		public string TextEditor
			{
			get { return _textEditor; }
			set
				{
				_textEditor = value; 
				NotifyOfPropertyChange(()=>TextEditor);
				}
			}

		private string _binEditor = Settings.BinEditor;

		public string BinEditor
			{
			get { return _binEditor; }
			set
				{
				_binEditor = value; 
				NotifyOfPropertyChange(()=>BinEditor);	
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
		#endregion

		public async Task Cancel()
			{
			await TryCloseAsync();
			}

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
			Settings.WriteToRegistry();
			await TryCloseAsync();
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
