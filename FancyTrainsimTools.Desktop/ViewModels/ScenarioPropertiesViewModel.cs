using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class ScenarioPropertiesViewModel : Screen
		{
		#region Properties
		private readonly IWindowManager _windowManager;
		public ScenarioModel Scenario
			{
			get => _scenario;
			set
				{
				_scenario = value;
				NotifyOfPropertyChange(()=> CanOpenPackFile);
				}
			}
		public RouteModel Route { get; set; }

		private ScenarioPropertiesModel _scenarioProperties = new ScenarioPropertiesModel();

		public ScenarioPropertiesModel ScenarioProperties
			{
			get { return _scenarioProperties; }
			set
				{
				_scenarioProperties = value;
				NotifyOfPropertyChange(() => ScenarioProperties);
				}
			}

		private BindableCollection<ConsistModel> _filteredConsistList;
		public BindableCollection<ConsistModel> FilteredConsistList
			{
			get { return _filteredConsistList; }
			set { _filteredConsistList = value; }
			}

		private BindableCollection<FullRailVehicleModel> _filteredRequiredRailVehicleList;

		public BindableCollection<FullRailVehicleModel> FilteredRequiredRailVehicleList
			{
			get { return _filteredRequiredRailVehicleList; }
			set
				{
				_filteredRequiredRailVehicleList = value; 
				NotifyOfPropertyChange(()=>FilteredRequiredRailVehicleList);	
				}
			}

		public BindableCollection<InstructionModel> PlayerInstructions { get; set; }

		public BindableCollection<CareerRule> CareerRuleList{ get; set; }



		private string ScenarioPath;
		private string ScenarioPropertiesPath;
		private string ScenarioBinPath;
		private string ScenarioXmlPath;
		private string ScenarioArchivePath;
		private ScenarioModel _scenario;
		#endregion

		#region initialization

		/// <summary>Creates an instance of the screen.</summary>
		public ScenarioPropertiesViewModel(IWindowManager windowManager)
			{
			_windowManager = windowManager; 
			}

		protected override async void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			ScenarioPath = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\Scenarios\\{Scenario.ScenarioGuid}\\";
			ScenarioArchivePath = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\{Scenario.Pack}";

			if (Scenario.IsPacked)
				{
				//var ArchivePath = RoutePath + "\\" + Package;
				//Result += CApps.CreateTempFileFromPack(ArchivePath, "Scenarios/" + ScenarioGuid + "/Scenario.bin",
				//	ScenarioXml, true);
				string tempBasePath = FileIOHelper.GetTempBasePath();
				ScenarioPropertiesPath = $"{tempBasePath}ScenarioProperties.xml";
				FileIOHelper.UnpackSingleFile(ScenarioArchivePath, $"Scenarios/{Scenario.ScenarioGuid}/ScenarioProperties.xml", ScenarioPropertiesPath);
				ScenarioBinPath = $"{tempBasePath}Scenario.bin";
				ScenarioXmlPath = $"{ScenarioPath}Scenario.xml";
				FileIOHelper.UnpackSingleFile(ScenarioArchivePath, $"Scenarios/{Scenario.ScenarioGuid}/Scenario.bin", ScenarioBinPath);
				}
			else
				{
				ScenarioPropertiesPath = $"{ScenarioPath}ScenarioProperties.xml";
				ScenarioBinPath = $"{ScenarioPath}Scenario.bin";
				ScenarioXmlPath = FileIOHelper.GetTempFilePath("Scenario.xml");
				}
			ScenarioPropertiesDataAccess.ReadScenarioProperties(ScenarioPropertiesPath, ScenarioProperties);
			FileIOHelper.DecodeSerz(ScenarioBinPath,ScenarioXmlPath,out var serzResult);
			ScenarioProperties.BinDoc= XDocument.Load(ScenarioXmlPath);
			Scenario.ScenarioProperties = ScenarioProperties;
			ScenarioProperties.ConsistList =
				ConsistDataAccess.GetAllConsistsForScenario(Scenario.Id,
					Scenario.ScenarioProperties.BinDoc);
			await ConsistDataAccess.SaveConsistsToDatabaseMaster(ScenarioProperties.ConsistList);
			ScenarioProperties.RequiredRailVehicles = await FullRailVehicleDataAccess.GetRequiredRailVehiclesForScenario(Scenario.Id);
			PlayerInstructions= new BindableCollection<InstructionModel>(ConsistDataAccess.GetPlayerInstructions(ScenarioProperties.ConsistList));
			FilteredConsistList= new BindableCollection<ConsistModel>(ScenarioProperties.ConsistList);
			NotifyOfPropertyChange(()=>FilteredConsistList);
			FilteredRequiredRailVehicleList =
				new BindableCollection<FullRailVehicleModel>(ScenarioProperties.RequiredRailVehicles);
			if (Scenario.ScenarioClass == "Career")
				{
				CareerRuleList =
					new BindableCollection<CareerRule>(
						CareerRuleDataAccess.ReadRules(Scenario.ScenarioProperties.PropertiesDoc));
				NotifyOfPropertyChange(()=> CareerRuleList);
				}

			NotifyOfPropertyChange(()=>PlayerInstructions);
			}

		#endregion

		#region scenarioProperties

		public bool CanOpenPackFile
			{
			get
				{
				return Scenario.IsPacked;
				}
			}

		public void OpenPackFile()
			{
			FileIOHelper.OpenZipFile(ScenarioArchivePath);
			}

		#endregion

		#region View

		public void ViewScenarioFolder()
			{
			FileIOHelper.OpenFolder(ScenarioPath);
			}

		public void ViewProperties()
			{

			FileIOHelper.EditTextFile(ScenarioPropertiesPath, Settings.TextEditor);
			}

		public void ViewBinFile()
			{
			FileIOHelper.EditTextFile(ScenarioBinPath, Settings.BinEditor);
			}

		#endregion

		#region Tools

		public async Task ConsistManager()
			{
			ConsistViewModel consistVM = IoC.Get<ConsistViewModel>();
			consistVM.Scenario = Scenario;
			await _windowManager.ShowWindowAsync(consistVM);
			}
		#endregion

		#region Editor


		#endregion

		#region Publish

		public async Task DocumentScenario()
			{
			var documentScenarioVM = IoC.Get<DocumentScenarioViewModel>();
			documentScenarioVM.Route = Route;
			documentScenarioVM.Scenario = Scenario;

			await _windowManager.ShowWindowAsync(documentScenarioVM);
			}
		public async Task PublishScenario()
			{
			var publishScenarioVM = IoC.Get<PublishScenarioViewModel>();
			publishScenarioVM.Scenario = Scenario;
			publishScenarioVM.Route = Route;
			await _windowManager.ShowWindowAsync(publishScenarioVM);
			}
		#endregion

		#region CleanUp

		public bool CanDeleteBak
			{
			get
				{
				return Directory.GetFiles(ScenarioPath, "*.bak?").Length > 0;
				}
			}

		public void DeleteBak()
			{
			ScenarioPropertiesDataAccess.DeleteBackupFiles(ScenarioPath);
			NotifyOfPropertyChange(()=>CanDeleteBak);
			}

		public bool CanDeleteSaved
			{
			get
				{
				return File.Exists($"{ScenarioPath}CurrentSave.bin");
				}
			}

		public void DeleteSaved()
			{
			FileIOHelper.DeleteSingleFile($"{ScenarioPath}CurrentSave.bin.MD5");
			FileIOHelper.DeleteSingleFile($"{ScenarioPath}CurrentSave.bin");
			NotifyOfPropertyChange(()=>CanDeleteSaved);
			}

		public bool CanDeleteRollingStart
			{
			get
				{
				return File.Exists($"{ScenarioPath}StartingSave.bin");
				}
			}

		public void DeleteRollingStart()
			{
			FileIOHelper.DeleteSingleFile($"{ScenarioPath}StartingSave.bin.MD5");
			FileIOHelper.DeleteSingleFile($"{ScenarioPath}StartingSave.bin");
			NotifyOfPropertyChange(()=>CanDeleteSaved);
			}

		public bool CanDeleteScripts
			{
			get
				{
				return Directory.GetFiles(ScenarioPath, "*.lua?").Length > 0 ||
				       Directory.GetFiles(ScenarioPath, "*.out").Length > 0;
				}
			}

		public void DeleteScripts()
			{
			ScenarioPropertiesDataAccess.DeleteScriptFiles(ScenarioPath);
			NotifyOfPropertyChange(()=> CanDeleteScripts);
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
