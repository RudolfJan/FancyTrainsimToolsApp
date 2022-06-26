using Assets.Library.Models;
using Caliburn.Micro;
using Logging.Library;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using Assets.Library.Helpers;
using Assets.Library.Logic;
using FancyTrainsimToolsDesktop.Helpers;
using Syroot.Windows.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class InstallerViewModel : Screen
		{
		#region Properties

		private BindableCollection<FileInfo> _SourceFileList = null;

		public BindableCollection<FileInfo> SourceFileList
			{
			get
				{
				return _SourceFileList;
				}
			set
				{
				_SourceFileList = value;
				}
			}

		private FileInfo _selectedSourceFile;

		public FileInfo SelectedSourceFile
			{
			get
				{
				return _selectedSourceFile;
				}
			set
				{
				_selectedSourceFile = value;
				FilesInArchiveList = new BindableCollection<InstallerFileModel>(GetArchivedFiles(SelectedSourceFile));
				NotifyOfPropertyChange(() => FilesInArchiveList);
				}
			}

		private BindableCollection<AddOnCatalogModel> _addOnCatalogList;
		public BindableCollection<AddOnCatalogModel> AddOnCatalogList
			{
			get
				{
				return _addOnCatalogList;
				}
			set
				{
				_addOnCatalogList = value;
				}
			}

		private AddOnCatalogModel selectedAddOn;

		public AddOnCatalogModel SelectedAddOn
			{
			get
				{
				return selectedAddOn;
				}
			set
				{
				selectedAddOn = value;
				NotifyOfPropertyChange(()=>CanRegisterAddOn);
				}
			}

		private BindableCollection<InstallerFileModel> _filesInArchiveList;
		public BindableCollection<InstallerFileModel> FilesInArchiveList
			{
			get => _filesInArchiveList;
			set
				{
				_filesInArchiveList = value;
				}
			}


		private InstallerFileModel _selectedFileInArchive;
		public InstallerFileModel SelectedFileInArchive
			{
			get
				{
				return _selectedFileInArchive;
				}
			set
				{
				_selectedFileInArchive = value;
				NotifyOfPropertyChange(() => CanViewFile);
				NotifyOfPropertyChange(() => CanViewEmbeddedRwp);
				NotifyOfPropertyChange(()=> CanCheckStatus);
				NotifyOfPropertyChange(()=>CanInstallAllFiles);
				NotifyOfPropertyChange(()=>CanRegisterAddOn);
				NotifyOfPropertyChange(()=>CanInstallNewFiles);
				}
			}

		private DirectoryInfo _InstallSourceDirectory = null;
		public DirectoryInfo InstallSourceDirectory
			{
			get => _InstallSourceDirectory;
			set
				{
				_InstallSourceDirectory = value;
				}
			}

		private String _InstallTargetDirectory = String.Empty;
		public String InstallTargetDirectory
			{
			get => _InstallTargetDirectory;
			set
				{
				_InstallTargetDirectory = value;
				}
			}

		private String _TempDirectory = String.Empty;
		public String TempDirectory
			{
			get => _TempDirectory;
			set
				{
				_TempDirectory = value;
				}
			}

		private UInt32 _Installed = 0;
		public UInt32 Installed
			{
			get => _Installed;
			set
				{
				_Installed = value;
				}
			}

		private UInt32 _Total = 0;
		public UInt32 Total
			{
			get => _Total;
			set
				{
				_Total = value;
				}
			}

		private UInt32 _Outdated = 0;
		public UInt32 Outdated
			{
			get => _Outdated;
			set
				{
				_Outdated = value;
				}
			}

		private String _CheckReportText = String.Empty;
		public String CheckReportText
			{
			get => _CheckReportText;
			set
				{
				_CheckReportText = value;
				NotifyOfPropertyChange(() => CheckReportText);
				}
			}

		private InstallerFileModel _EmbeddedCompressedFile = null;
		public InstallerFileModel EmbeddedCompressedFile
			{
			get => _EmbeddedCompressedFile;
			set
				{
				_EmbeddedCompressedFile = value;
				}
			}

		#endregion

		#region Initialization

		protected override async void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			TempDirectory = Settings.TempFolder;
			InstallTargetDirectory = Settings.TrainSimGamePath;

			InstallSourceDirectory = new DirectoryInfo(Settings.DownloadFolder);
			SourceFileList = new BindableCollection<FileInfo>(SevenZipDataAccess.GetInstallerFiles(new DirectoryInfo(Settings.DownloadFolder)));
			NotifyOfPropertyChange(()=>SourceFileList);
			AddOnCatalogList= new BindableCollection<AddOnCatalogModel>(await AddOnCatalogDataAccess.GetAllAddOns());
			NotifyOfPropertyChange(()=>AddOnCatalogList);
			}

		#region Commands

		public bool CanViewFile
			{
			get
				{
				return SelectedSourceFile != null && SelectedFileInArchive != null;
				}

			}

		public void ViewFile()
			{
			ViewFileContents(SelectedSourceFile, SelectedFileInArchive);
			}

		public bool CanViewEmbeddedRwp
			{
			get
				{
				return SelectedSourceFile != null && SelectedFileInArchive != null && SelectedFileInArchive.Extension == ".rwp";
				}
			}

		public async Task ViewEmbeddedRwp()
			{
			FilesInArchiveList = new BindableCollection<InstallerFileModel>(await ViewEmbeddedCompressedFileAsync(SelectedSourceFile, SelectedFileInArchive));
			NotifyOfPropertyChange(() => FilesInArchiveList);
			NotifyOfPropertyChange(() => EmbeddedCompressedFile);
			}

		public bool CanCheckStatus
			{
			get
				{
				return SelectedSourceFile != null;
				}
			}


		public void CheckStatus()
			{
			CheckFiles();
			}

		public bool CanInstallAllFiles
			{
			get
				{
				return SelectedSourceFile != null;
				}
			}


		public void InstallAllFiles()
			{
			InstallAllFiles(SelectedSourceFile);
			}

		public bool CanInstallNewFiles
			{
			get
				{
				return SelectedSourceFile != null;
				}
			}

		public void InstallNewFiles()
			{
			InstallNewFiles(SelectedSourceFile);
			}

		public bool CanRegisterAddOn
			{
			get
				{
				return SelectedAddOn!=null && SelectedSourceFile != null;
				}
			}

		public async Task RegisterAddOn()
			{
			List<InstallerFileModel> BinFiles =
				FilesInArchiveList.Where(x => x.Extension == ".bin").ToList();
			foreach (var bin in BinFiles)
				{
				var flatAsset = AssetCollectionDataAccess.GetAssetFromPath(bin.FullName);
				if (flatAsset != null)
					{
					flatAsset.Pack = "";
					var asset = AssetCollectionDataAccess.ConvertFlatAssetToAsset(flatAsset);
					asset.AddOnCatalogId = SelectedAddOn.Id;
					await AssetCollectionDataAccess.UpsertAssetAsync(asset);
					}
				}
			}

		#endregion

		#endregion

		#region getarchives
		public List<InstallerFileModel> GetArchivedFiles(FileInfo ArchiveFile)
			{
			var Extension = ArchiveFile.Extension.ToLower();

			switch (Extension)
				{
				case ".zip":
				case ".ap":
						{
						return GetZipArchivedFiles(ArchiveFile);
						}
				case ".rwp":
				case ".rpk":
				case ".rar":
				case ".7z":
						{
						return GetRwpArchivedFiles(ArchiveFile);
						}
				case ".exe":
						{
						return new List<InstallerFileModel>();
						}
				default:
						{
						Log.Trace("Archive type " + Extension + " is not (yet) supported");
						return new List<InstallerFileModel>();
						}
				}
			}

		// Show contents of a .zip file
		public List<InstallerFileModel> GetZipArchivedFiles(FileInfo ArchiveFile)
			{
			var output = new List<InstallerFileModel>();
			try
				{
				using var Archive = ZipFile.OpenRead(ArchiveFile.FullName);
				foreach (var Entry in Archive.Entries)
					{
					InstallerFileModel FilePresenter = new InstallerFileModel(Entry.FullName, Entry.Name, Entry.LastWriteTime);
					output.Add(FilePresenter);
					}
				return output;
				}
			catch (Exception)
				{
				Log.Trace("Failed to show file entries for archive " + ArchiveFile.FullName, LogEventType.Error);
				}
			return output;
			}

		public List<InstallerFileModel> GetRwpArchivedFiles(FileInfo ArchiveFile)
			{
			return GetRwpArchivedFiles(ArchiveFile.FullName);
			}

		public List<InstallerFileModel> GetRwpArchivedFiles(String ArchiveFile)
			{
			SevenZipDataAccess.ListZipFiles(ArchiveFile, out var FileReport);
			var Skip = 16;
			if (String.Compare(Path.GetExtension(ArchiveFile), ".rar", StringComparison.Ordinal) == 0 || String.Compare(Path.GetExtension(ArchiveFile), ".7z", StringComparison.Ordinal) == 0)
				{
				Skip = 19;
				}
			var output = new List<InstallerFileModel>();
			using var Reader = new StringReader(FileReport);
			String MetaData;
			for (int I = 0; I < Skip; I++)
				{
				MetaData = Reader.ReadLine() + "\r\n";
				}
			var Done = false;
			while (!Done)
				{
				var Temp = Reader.ReadLine();
				if (Temp == null || Temp.StartsWith("-----"))
					{
					Done = true;
					}
				else
					{
					if (Temp.Length > 0)
						{
						var FilePresenter = new InstallerFileModel();
						FilePresenter.Parse7ZLine(Temp);
						output.Add(FilePresenter);
						}
					}
				}
			return output;
			}

		public async Task<List<InstallerFileModel>> GetRwpArchivedFilesAsync(String ArchiveFile)
			{				
			var output = new List<InstallerFileModel>();
			await Task.Run(() =>
				{
				SevenZipDataAccess.ListZipFiles(ArchiveFile, out var FileReport);
				var Skip = 16;
				if (String.Compare(Path.GetExtension(ArchiveFile), ".rar", StringComparison.Ordinal) == 0 ||
				    String.Compare(Path.GetExtension(ArchiveFile), ".7z", StringComparison.Ordinal) == 0)
					{
					Skip = 19;
					}


				using var Reader = new StringReader(FileReport);
				String MetaData;
				for (int I = 0; I < Skip; I++)
					{
					MetaData = Reader.ReadLine() + "\r\n";
					}

				var Done = false;
				while (!Done)
					{
					var Temp = Reader.ReadLine();
					if (Temp == null || Temp.StartsWith("-----"))
						{
						Done = true;
						}
					else
						{
						if (Temp.Length > 0)
							{
							var FilePresenter = new InstallerFileModel();
							FilePresenter.Parse7ZLine(Temp);
							output.Add(FilePresenter);
							}
						}
					}
				});
			return output;
			}




		#endregion

		#region ContentViewers

		public void ViewFileContents(FileInfo ArchiveFile, InstallerFileModel ArchiveEntry)
			{
			switch (ArchiveFile.Extension)
				{
				case ".zip":
				case ".ap":
						{
						ViewZippedContent(ArchiveFile, ArchiveEntry);
						break;
						}
				case ".rwp":
				case ".rpk":
				case ".rar":
				case ".7z":
						{
						ViewRwpContent(ArchiveFile, ArchiveEntry);
						break;
						}
				default:
						{
						Log.Trace("Archive type " + ArchiveFile.Extension + " is not (yet) supported");
						break;
						}
				}
			}

		public void ViewZippedContent(FileInfo ArchiveFile, InstallerFileModel ArchiveEntry)
			{
			try
				{
				using (var Archive = ZipFile.OpenRead(ArchiveFile.FullName))
					{
					var Entry = Archive.GetEntry(ArchiveEntry.FullName);
					var DestinationFile = TempDirectory + ArchiveEntry.Name;
					FileIOHelper.DeleteSingleFile(DestinationFile);
					Entry.ExtractToFile(DestinationFile);
					FileIOHelper.OpenFileWithShell(DestinationFile);
					}
				}
			catch (Exception)
				{
				Log.Trace("Failed to show file entries for archive " + ArchiveFile.FullName, LogEventType.Error);
				}
			}

		public void ViewRwpContent(FileInfo ArchiveFile, InstallerFileModel ArchiveEntry)
			{
			try
				{
				var DestinationFile = TempDirectory + ArchiveEntry.Name;
				FileIOHelper.DeleteSingleFile(DestinationFile);
				SevenZipDataAccess.ExtractZipFile(ArchiveFile.FullName, ArchiveEntry.FullName, TempDirectory);
				FileIOHelper.OpenFileWithShell(DestinationFile);
				}
			catch (Exception E)
				{
				Log.Trace("Failed to show file entries for archive " + ArchiveFile.FullName + " because " + E.Message, LogEventType.Error);
				}
			}

		public List<InstallerFileModel> ViewEmbeddedCompressedFile(FileInfo ArchiveFile, InstallerFileModel ArchiveEntry)
			{
			try
				{
				EmbeddedCompressedFile = ArchiveEntry;
				// extract the embedded file first to a temporary location
				var DestinationFile = TempDirectory + ArchiveEntry.Name;
				SevenZipDataAccess.ExtractZipFile(ArchiveFile.FullName, ArchiveEntry.FullName, TempDirectory);
				return GetRwpArchivedFiles(DestinationFile);
				}
			catch (Exception E)
				{
				Log.Trace("Failed to show file entries for archive " + ArchiveFile.FullName + " because " + E.Message, LogEventType.Error);
				}
			return null;
			}

		public async Task<List<InstallerFileModel>> ViewEmbeddedCompressedFileAsync(FileInfo ArchiveFile, InstallerFileModel ArchiveEntry)
			{
			try
				{
				EmbeddedCompressedFile = ArchiveEntry;
				// extract the embedded file first to a temporary location
				var DestinationFile = TempDirectory + ArchiveEntry.Name;
				SevenZipDataAccess.ExtractZipFile(ArchiveFile.FullName, ArchiveEntry.FullName, TempDirectory);
				return await GetRwpArchivedFilesAsync(DestinationFile);
				}
			catch (Exception E)
				{
				Log.Trace("Failed to show file entries for archive " + ArchiveFile.FullName + " because " + E.Message, LogEventType.Error);
				}
			return null;
			}


		#endregion ContentViewers

		#region Validators

		public void CheckFiles()
			{
			Total = 0;
			Installed = 0;
			Outdated = 0;
			foreach (var Entry in FilesInArchiveList)
				{
				if (Entry.CheckFile(Settings.TrainSimGamePath)) // Skip directory entries here
					{
					Total++;
					if (Entry.IsInstalled)
						{
						Installed++;
						}
					if (Entry.IsOutdated)
						{
						Outdated++;
						}
					}
				}
			CheckReportText = CheckReport();
			}

		public String CheckReport()
			{
			return "Entries " + Total.ToString() + ", Installed " + Installed.ToString() + ", Outdated " + Outdated.ToString();
			}

		#endregion Validators

		#region Installers

		public void InstallAllFiles(FileInfo ArchiveFile, InstallerFileModel FileEntry = null)
			{

			// try embedded .rwp/rpk file
			if (EmbeddedCompressedFile != null)
				{
				var SourceFile = TempDirectory + EmbeddedCompressedFile.Name;
				InstallEmbeddedCompressedFile(SourceFile);
				return;
				}

			// Otherwise, check if there is an installable SelectedFIle
			if (FileEntry != null)
				{
				switch (FileEntry.Extension)
					{
					case ".zip":
					case ".ap":
					case ".rwp":
					case ".rpk":
					case ".rar":
					case ".7z":
							{
							InstallAllZippedFiles(ArchiveFile);
							return;
							}
					case ".exe:":
							{
							FilesAndDirectories.ExecuteFile(ArchiveFile);
							return;
							}
					default:
							{
							break;
							}
					}
				}

			// If none of this, we install all files
			switch (ArchiveFile.Extension)
				{
				case ".zip":
				case ".ap":
				case ".rwp":
				case ".rpk":
				case ".rar":
				case ".7z":
						{
						InstallAllZippedFiles(ArchiveFile);
						break;
						}
				case ".exe:":
						{
						FilesAndDirectories.ExecuteFile(ArchiveFile);
						break;
						}
				default:
						{
						Log.Trace("Archive type " + ArchiveFile.Extension + " is not (yet) supported");
						break;
						}
				}
			}

		public void InstallEmbeddedCompressedFile(String FileName)
			{
			SevenZipDataAccess.SevenZipExtractAll(Settings.SevenZip, FileName, InstallTargetDirectory);
			}

		// Uses 7zip and supports all zipped files
		public void InstallAllZippedFiles(FileInfo ArchiveFile)
			{
			SevenZipDataAccess.SevenZipExtractAll(Settings.SevenZip, ArchiveFile.FullName, InstallTargetDirectory);
			}

		public void InstallNewFiles(FileInfo ArchiveFile)
			{
			if (EmbeddedCompressedFile != null)
				{
				var SourceFile = TempDirectory + EmbeddedCompressedFile.Name;
				InstallNewEmbeddedCompressedFile(SourceFile);
				return;
				}
			switch (ArchiveFile.Extension)
				{
				case ".zip":
				case ".ap":
				case ".rwp":
				case ".rpk":
				case ".rar":
				case ".7z":
						{
						InstallNewZippedFiles(ArchiveFile);
						break;
						}
				default:
						{
						Log.Trace("Archive type " + ArchiveFile.Extension + " is not (yet) supported");
						break;
						}
				}
			}

		public void InstallNewZippedFiles(FileInfo ArchiveFile)
			{
			foreach (var Entry in FilesInArchiveList)
				{
				if (!Entry.IsOutdated)
					{
					SevenZipDataAccess.SevenZipExtractSingle(ArchiveFile.FullName, InstallTargetDirectory, Entry.FullName);
					}
				}
			}

		public void InstallNewEmbeddedCompressedFile(String FileName)
			{
			foreach (var Entry in FilesInArchiveList)
				{
				if (!Entry.IsOutdated)
					{
					SevenZipDataAccess.SevenZipExtractSingle(FileName, InstallTargetDirectory, Entry.FullName);
					}
				}
			}

		#endregion Installers

		#region Support
		public async Task Exit()
			{
			await TryCloseAsync();
			}

		#endregion


		}
	}
