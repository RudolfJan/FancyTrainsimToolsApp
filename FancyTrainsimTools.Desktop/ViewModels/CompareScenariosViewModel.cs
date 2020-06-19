using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class CompareScenariosViewModel: Screen
		{
		#region Properties

		private readonly IWindowManager _windowManager;
		public RouteModel Route { get; set; }

		private ScenarioModel _scenario1;
		public ScenarioModel Scenario1
			{
			get
				{
				return _scenario1;
				}
			set
				{
				_scenario1 = value;
				}
			}

		private BindableCollection<ScenarioModel> _scenarioList;

		public BindableCollection<ScenarioModel> ScenarioList
			{
			get
				{
				return _scenarioList;
				}
			set
				{
				_scenarioList = value;
				}
			}

		private ScenarioModel _scenario2;
		public ScenarioModel Scenario2
			{
			get
				{
				return _scenario2;
				}
			set
				{
				_scenario2 = value;
				NotifyOfPropertyChange(()=>Scenario2);
				NotifyOfPropertyChange(()=>CanCompareProperties);
				NotifyOfPropertyChange(()=>CanCompareScenarioBin);
				}
			}
		#endregion

		#region Initialization

		public CompareScenariosViewModel(IWindowManager windowManager)
			{
			_windowManager = windowManager;
			}

		/// <summary>Called when an attached view's Loaded event fires.</summary>
		/// <param name="view"></param>
		protected override void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			ScenarioList= new BindableCollection<ScenarioModel>(ScenarioCollectionDataAccess.ReadScenariosFromDatabase(Route.RouteGuid));
			}

		#endregion

		#region Commands

		public bool CanCompareProperties
			{
			get
				{
				return Scenario2 != null && Scenario2.IsValidInGame;
				}
			}
		public void CompareProperties()
			{
			var destination =$"{Settings.TempFolder}Compare\\";
	    Directory.CreateDirectory(destination);
  		var routePath = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\";
			string path1 = GetScenarioPropertiesFile(Scenario1, routePath, destination);
			string path2 = GetScenarioPropertiesFile(Scenario2, routePath, destination);
			if (path1 != null && path2 != null)
				{
				FileIOHelper.FileCompareTool(path1, path2, Scenario1.ScenarioTitle, Scenario2.ScenarioTitle, Settings.FileCompare);
				}
			}

		public bool CanCompareScenarioBin
			{
			get
				{
				return Scenario2 != null && Scenario2.IsValidInGame;
				}
			}

		public void CompareScenarioBin()
			{
			var destination =$"{Settings.TempFolder}Compare\\";
			Directory.CreateDirectory(destination);
			var routePath = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\";
			string path1 = GetScenarioBinFile(Scenario1, routePath, destination);
			string path2 = GetScenarioBinFile(Scenario2, routePath, destination);
			if (path1 != null && path2 != null)
				{
				FileIOHelper.FileCompareTool(path1, path2, Scenario1.ScenarioTitle, Scenario2.ScenarioTitle, Settings.FileCompare);
				}
			}

		#endregion

		// Copy the scenario properties to a temporary directory and get a path to the copied file
		public static string GetScenarioPropertiesFile(ScenarioModel scenario, string routePath, string destination)
			{
			string output;
			if (!scenario.IsPacked)
				{
				var input = $"{routePath}\\Scenarios\\{scenario.ScenarioGuid}\\ScenarioProperties.xml";
				output = $"{destination}{scenario.ScenarioGuid}ScenarioProperties.xml";
				File.Copy(input,output,true);
				}
			else
				{
				string archivePath=$"{routePath}{scenario.Pack}";
				var input=$"Scenarios/{scenario.ScenarioGuid}/ScenarioProperties.xml";
				output = $"{destination}{scenario.ScenarioGuid}ScenarioProperties.xml";
				FileIOHelper.UnpackSingleFile(archivePath,input,output);
				}
			return output;
			}

		public static string GetScenarioBinFile(ScenarioModel scenario, string routePath, string destination)
			{
			string temp;
			if (!scenario.IsPacked)
				{
				var input = $"{routePath}\\Scenarios\\{scenario.ScenarioGuid}\\Scenario.bin";
				temp = $"{destination}{scenario.ScenarioGuid}Scenario.bin";
				File.Copy(input,temp,true);
				}
			else
				{
				string archivePath=$"{routePath}{scenario.Pack}";
				var input=$"Scenarios/{scenario.ScenarioGuid}/Scenario.bin";
				temp = $"{destination}{scenario.ScenarioGuid}Scenario.bin";
				FileIOHelper.UnpackSingleFile(archivePath,input,temp);
				}

			var output = temp.Replace(".bin", ".xml");
			FileIOHelper.DecodeSerz(temp,output, out bool success);
			return output;
			}

		#region Housekeeping

		public async Task Exit()
			{
			await TryCloseAsync();
			}
		#endregion
		}
	}
