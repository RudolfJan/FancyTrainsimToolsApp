using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class RoutePropertiesViewModel: Screen
		{
		private RouteModel _route;			
		public RouteModel Route
			{
			get { return _route; }
			set
				{
				_route = value;
				}
			}

		private BindableCollection<FileInfo> _packFileList;
		public BindableCollection<FileInfo> PackFileList
			{
			get { return _packFileList; }
			set
				{
				_packFileList = value; 
				}
			}

		private FileInfo _selectedPackFile;

		public FileInfo SelectedPackFile
			{
			get { return _selectedPackFile; }
			set
				{
				_selectedPackFile = value; 
				NotifyOfPropertyChange(()=>CanOpenPackedFile);
				}
			}

		protected override void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			string Path = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\";
			PackFileList= new BindableCollection<FileInfo>(RoutesCollectionDataAccess.GetPackFilesForRoute(Path));
			}

		public bool CanEditRouteProperties 
			{
			get
				{
				return Route.IsPacked=false;
				}
			}

		public void EditRouteproperties()
			{
			string Path = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\RouteProperties.xml";
			FileIOHelper.EditTextFile(Path,Settings.TextEditor);
			}

		public void OpenRouteFolder()
			{
			string Path = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\";
			FileIOHelper.OpenFolder(Path);
			}

		public bool CanOpenPackedFile
			{
			get
				{
				return SelectedPackFile != null;
				}
			}

		public void OpenPackedFile()
			{
			FileIOHelper.EditTextFile(SelectedPackFile.FullName,Settings.SevenZip);
			}

		public async Task Exit()
			{
			await TryCloseAsync();
			}
		}
	}
