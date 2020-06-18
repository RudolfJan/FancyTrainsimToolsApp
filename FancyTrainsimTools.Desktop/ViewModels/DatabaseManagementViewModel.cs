using Assets.Library.Logic;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using Logging.Library;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class DatabaseManagementViewModel: Screen
		{
				private string _backupName;

		public string BackupName
			{
			get { return _backupName; }
			set
				{
				_backupName = value;
				NotifyOfPropertyChange(()=>BackupName);
				NotifyOfPropertyChange(()=>CanCreateBackup);
				}
			}

		private BindableCollection<FileInfo> _backupList;

		public BindableCollection<FileInfo> BackupList
			{
			get { return _backupList; }
			set
				{
				_backupList = value; 
				NotifyOfPropertyChange(()=>BackupList);
				}
			}

		private FileInfo _selectedBackup;	

		public FileInfo SelectedBackup
			{
			get { return _selectedBackup; }
			set
				{
				_selectedBackup = value; 
				// NotifyOfPropertyChange(()=>SelectedBackup);
				NotifyOfPropertyChange(()=>CanDeleteBackup);
				NotifyOfPropertyChange(()=>CanRestoreBackup);
				}
			}

		protected override async void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			BuildBackupList();
			NotifyOfPropertyChange(()=>BackupName);
			NotifyOfPropertyChange(()=>CanDeleteActiveDatabase);
			NotifyOfPropertyChange(()=>CanDeleteBackup);
			NotifyOfPropertyChange(()=>CanRestoreBackup);
			}

		public void BuildBackupList()
			{
			if (!Directory.Exists(Settings.BackupPath))
				{
				Directory.CreateDirectory(Settings.BackupPath);
				}
			DirectoryInfo backupDir= new DirectoryInfo(Settings.BackupPath);
			BackupList= new BindableCollection<FileInfo>(backupDir.GetFiles("*.db",SearchOption.TopDirectoryOnly));
			NotifyOfPropertyChange(()=>BackupList);
			NotifyOfPropertyChange(()=>SelectedBackup);
			}

		public bool CanCreateBackup
			{
			get
				{
				return BackupName?.Length > 0;
				}
			}

		public bool CanRestoreBackup
			{
			get
				{
				return SelectedBackup != null;
				}
			}

		public bool CanDeleteBackup
			{
			get
				{
				return SelectedBackup != null;
				}
			}

		public bool CanDeleteActiveDatabase
			{
			get
				{
				return File.Exists(Settings.AssetDatabasePath);
				}
			}

		public bool CanCreateDatabase
			{
			get
				{
				return !File.Exists(Settings.AssetDatabasePath);
				}
			}

		public void CreateBackup()
			{
			string source = Settings.AssetDatabasePath;
			string target = $"{Settings.BackupPath}{BackupName}.db";
			if (CopyDatabase(source, target))
				{
				BackupName = "";
				FileInfo targetFile= new FileInfo(target);
				BackupList.Add(targetFile);
				NotifyOfPropertyChange(BackupName);
				}
			}

		private bool CopyDatabase(string source, string target)
			{
			try
				{
				File.Copy(source,target);
				return true;
				}
			catch (Exception ex)
				{
				Log.Trace($"Cannot copy to back {target}", ex, LogEventType.Error);
				return false;
				}
			}

		public void RestoreBackup()
			{
			// Make a safetybackup for the active database
			string source = Settings.AssetDatabasePath;
			string guid = Guid.NewGuid().ToString();
			string target = $"{Settings.BackupPath}Restore backup {guid}.db";
			if (File.Exists(source))
				{
				if (CopyDatabase(source, target))
					{
					FileInfo targetFile = new FileInfo(target);
					BackupList.Add(targetFile);
					FileIOHelper.DeleteSingleFile(source);
					CopyDatabase(SelectedBackup.FullName, source);
					}
				}
			else
				{
				// no active database for any reason ..
				CopyDatabase(SelectedBackup.FullName, source);
				NotifyOfPropertyChange(()=>CanDeleteActiveDatabase);
				}
			}

		public void DeleteBackup()
			{
			FileIOHelper.DeleteSingleFile(SelectedBackup.FullName);
			BackupList.Remove(SelectedBackup);
			SelectedBackup = null;
			NotifyOfPropertyChange(()=>BackupList);
			NotifyOfPropertyChange(()=>SelectedBackup);
			}

		public void DeleteActiveDatabase()
			{
			// Make a safety backup for the active database
			string source = Settings.AssetDatabasePath;
			string guid = Guid.NewGuid().ToString();
			string target = $"{Settings.BackupPath}Delete backup {guid}.db";
			if (CopyDatabase(source, target))
				{
				FileInfo targetFile = new FileInfo(target);
				BackupList.Add(targetFile);
				FileIOHelper.DeleteSingleFile(source);
				NotifyOfPropertyChange(() => CanDeleteActiveDatabase);
				NotifyOfPropertyChange(() => CanCreateDatabase);
				NotifyOfPropertyChange(() => CanExit);
				}
			}
		public void CreateDatabase()
			{
			AssetDatabaseAccess.InitDatabase(AssetDatabaseAccess.GetConnectionString(),Settings.AssetDatabasePath);
			NotifyOfPropertyChange(() => CanDeleteActiveDatabase);
			NotifyOfPropertyChange(() => CanCreateDatabase);
			NotifyOfPropertyChange(() => CanExit);
			}

		public bool CanExit
			{
			get
				{
				return !CanCreateDatabase; // Exit is disabled if there is no database.
				}
			}

		public async Task Exit()
			{
			await TryCloseAsync();
			}
		}
	}
