using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using FancyTrainsimToolsDesktop.Models;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Screen = Caliburn.Micro.Screen;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class AddOnCatalogViewModel : Screen
		{
		#region Properties

		private bool CopyValues=true;
		private BindableCollection<FileInfo> _windowsDownloadList;
		public BindableCollection<FileInfo> WindowsDownloadList
			{
			get
				{
				return _windowsDownloadList;
				}
			set
				{
				_windowsDownloadList = value;
				}
			}

		private FileInfo _selectedWindowsDownload;

		public FileInfo SelectedWindowsDownload
			{
			get
				{
				return _selectedWindowsDownload;
				}
			set
				{
				_selectedWindowsDownload = value;
				NotifyOfPropertyChange(()=>CanMoveToArchive);
				}
			}

		private BindableCollection<FileInfo> _downloadArchiveList;

		public BindableCollection<FileInfo> DownloadArchiveList
			{
			get
				{
				return _downloadArchiveList;
				}
			set
				{
				_downloadArchiveList = value;
				}
			}

		private FileInfo _selectedDownload;

		public FileInfo SelectedDownload
			{
			get
				{
				return _selectedDownload;
				}
			set
				{
				_selectedDownload = value;
				if (CopyValues)
					{
					ArchiveName = SelectedDownload.Name;
					}
				}
			}

		private string _archiveName;

		public string ArchiveName
			{
			get
				{
				return _archiveName;
				}
			set
				{
				_archiveName = value;
				NotifyOfPropertyChange(()=>ArchiveName);
				}
			}

		private BindableCollection<AddOnCatalogModel> _addOnCatalogList;
		private AddOnCatalogModel selectedAddOn;

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

		public AddOnCatalogModel SelectedAddOn
			{
			get
				{
				return selectedAddOn;
				}
			set
				{
				selectedAddOn = value;
				if (selectedAddOn!=null&&CopyValues)
					{
					Name = SelectedAddOn.Name;
					Location = SelectedAddOn.Location;
					ArchiveName = SelectedAddOn.ArchiveName;
					Description = SelectedAddOn.Description;
					IsPayware = SelectedAddOn.IsPayware;
					AddOnType = SelectedAddOn.AddOnType;
					}
				NotifyOfPropertyChange(()=>SelectedAddOn);
				NotifyOfPropertyChange(()=>CanSave);
				NotifyOfPropertyChange(()=>CanDelete);
				}
			}

		private string _Name= String.Empty;

		public string Name
			{
			get { return _Name; }
			set
				{
				_Name = value;
				NotifyOfPropertyChange(() => Name);
				NotifyOfPropertyChange(()=>CanSave);
				}
			}

		private string _Description=String.Empty;

		public string Description
			{
			get { return _Description; }
			set
				{
				_Description = value;
				NotifyOfPropertyChange(() => Description);
				}
			}

		private string _Location= String.Empty;

		public string Location
			{
			get { return _Location; }
			set
				{
				_Location = value;
				NotifyOfPropertyChange(() => Location);
				}
			}

		private bool _IsPayware;

		public bool IsPayware
			{
			get { return _IsPayware; }
			set
				{
				_IsPayware = value;
				NotifyOfPropertyChange(() => IsPayware);
				}
			}

		private AddOnTypeEnum _AddOnType=0;
		public AddOnTypeEnum AddOnType
			{
			get
				{
				return _AddOnType;
				}
			set 
				{ _AddOnType= value;
				NotifyOfPropertyChange(()=>AddOnType);
				NotifyOfPropertyChange(()=>CanSave);
				}
			}
		#endregion

		protected override async void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			WindowsDownloadList=new BindableCollection<FileInfo>(SevenZipDataAccess.GetInstallerFiles(new DirectoryInfo(KnownFolders.Downloads.DefaultPath)).OrderByDescending(x=>x.LastAccessTime));
			var dir= new DirectoryInfo(Settings.DownloadFolder);
			DownloadArchiveList= new BindableCollection<FileInfo>(dir.GetFiles("*").ToList());
			AddOnCatalogList= new BindableCollection<AddOnCatalogModel>(await AddOnCatalogDataAccess.GetAllAddOns());
			NotifyOfPropertyChange(()=>AddOnCatalogList);
			NotifyOfPropertyChange(()=>WindowsDownloadList);
			NotifyOfPropertyChange(()=>DownloadArchiveList);
			}

		public bool CanMoveToArchive
			{
			get
				{
				return SelectedWindowsDownload != null && string.CompareOrdinal(Settings.DownloadFolder,KnownFolders.Downloads.DefaultPath)!=0;
				}
			}

		public void MoveToArchive()
			{
			string targetFile = $"{Settings.DownloadFolder}{SelectedWindowsDownload.Name}";
			File.Move(SelectedWindowsDownload.FullName,targetFile,true);
			FileInfo target= new FileInfo(targetFile);
			DownloadArchiveList.Add(target);
			WindowsDownloadList.Remove(SelectedWindowsDownload);
			NotifyOfPropertyChange(()=>WindowsDownloadList);
			SelectedWindowsDownload = null;
			
			NotifyOfPropertyChange(()=>SelectedWindowsDownload);
			NotifyOfPropertyChange(()=>DownloadArchiveList);
			}

		public bool CanGetWithFileSelector
			{
			get
				{
				return true;
				}
			}

		public void GetWithFileSelector()
			{
			var openFileParams= new OpenFileModel();
			openFileParams.InitialDirectory = KnownFolders.Downloads.DefaultPath;
			openFileParams.Title = "Move file to TS archive";
			openFileParams.CheckPathExists = true;
			openFileParams.Filter = "rwp files|*.rwp|rpk files|*.rwp|rpk files|*.rwp|zip files|*.zip|7z files|*.7z|rar files|*.rar|exe files|*.exe|All Files|*.*";
			var source = FileIOHelper.GetOpenFileName(openFileParams);
			if (!String.IsNullOrEmpty(source))
				{
				var target =$"{Settings.DownloadFolder}{Path.GetFileName(source)}";
				File.Move(source,target,true);
				DownloadArchiveList.Add(new FileInfo(target));
				}
			NotifyOfPropertyChange(()=> DownloadArchiveList);
			}

		public bool CanSave
			{
			get
				{
				return Name?.Length > 3 && AddOnType>0;
				}
			}

		public bool CanDelete
			{
			get
				{
				return SelectedAddOn != null;
				}
			}
		public void Save()
			{
			if (SelectedAddOn == null)
				{
				CopyValues = false;
				SelectedAddOn=new AddOnCatalogModel();
				CopyValues = true;
				}
			SelectedAddOn.Name = Name;
			SelectedAddOn.ArchiveName = ArchiveName;
			SelectedAddOn.Location = Location;
			SelectedAddOn.Description = Description;
			SelectedAddOn.IsPayware = IsPayware;
			SelectedAddOn.AddOnType = AddOnType;
			if (SelectedAddOn.Id == 0)
				{
				AddOnCatalogDataAccess.InsertAddOnCatalogItem(SelectedAddOn);
				AddOnCatalogList.Add(SelectedAddOn);
				Clear();
				}
			else
				{
				AddOnCatalogDataAccess.UpdateAddOnCatalogItem(SelectedAddOn);
				}

			NotifyOfPropertyChange(()=>AddOnCatalogList);
			}

		public void Clear()
			{
			Name = null;
			Description = null;
			ArchiveName = null;
			Location = null;
			IsPayware = false;
			AddOnType = 0;
			SelectedAddOn = null;
			NotifyOfPropertyChange(()=>SelectedAddOn);
			}

		public void Delete()
			{
			AddOnCatalogDataAccess.DeleteAddOnCatalogItem(SelectedAddOn);
			AddOnCatalogList.Remove(SelectedAddOn);
			Clear();
			}

		public async Task Exit()
			{
			await TryCloseAsync();
			}
		}
	}
