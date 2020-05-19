using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimTools.Desktop.Helpers;
using FancyTrainsimTools.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FancyTrainsimTools.Desktop.ViewModels
	{

	public class RoutesAndScenariosViewModel : Screen
		{
		private readonly IWindowManager _windowManager;

		public List<RouteModel> RouteList { get; set; } = new List<RouteModel>();

		private RouteFilterModel _routeFilter;

		public RouteFilterModel RouteFilter
			{
			get { return _routeFilter; }
			set
				{
				_routeFilter = value;
				NotifyOfPropertyChange(() => RouteFilter);
				}
			}

		private BindableCollection<RouteModel> _filteredRouteList;

		public BindableCollection<RouteModel> FilteredRouteList
			{
			get { return _filteredRouteList; }
			set
				{
				_filteredRouteList = value;
				NotifyOfPropertyChange(() => FilteredRouteList);
				}
			}

		public ScenariosUIModel Scenarios { get; set; }

		private RouteModel _selectedRoute;

		public RouteModel SelectedRoute
			{
			get { return _selectedRoute; }
			set
				{
				if (_selectedRoute != value)
					{
					_selectedRoute = value;
					GetScenarios(_selectedRoute);
					NotifyOfPropertyChange(() => CanGetRouteAssets);
					NotifyOfPropertyChange(() => Scenarios);
					NotifyOfPropertyChange(() => CanRouteProperties);
					NotifyOfPropertyChange(()=>CanCopyToGame);
					NotifyOfPropertyChange(()=>CanCopyToArchive);
					}
				}
			}

		public bool CanGetRouteAssets
			{
			get
				{
				return SelectedRoute != null && SelectedRoute.IsValidInGame && SelectedRoute.InGame;
				}
			}

		public RoutesAndScenariosViewModel(IWindowManager windowManager)
			{
			_windowManager = windowManager;
			Scenarios = new ScenariosUIModel();
			}

		protected override void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			RoutesCollectionDataAccess.UpdateRouteTableForAllRoutes(Settings.GameRoutesFolder, true,
				false);
			RoutesCollectionDataAccess.UpdateRouteTableForAllRoutes(Settings.ArchiveRoutesFolder, false,
				true);
			RouteList = RoutesCollectionDataAccess.ReadAllRoutesFromDatabase();
			FilteredRouteList = new BindableCollection<RouteModel>(
				RoutesCollectionDataAccess.ApplyAssetsFilter(RouteList, RouteFilter).OrderBy(x => x.RouteName));
			}

		public void FilterRoutes()
			{
			FilteredRouteList = new BindableCollection<RouteModel>(
				RoutesCollectionDataAccess.ApplyAssetsFilter(RouteList, RouteFilter).OrderBy(x => x.RouteName));
			}

		public void FilterScenarios()
			{
			Scenarios.FilteredScenarioList = ScenarioCollectionDataAccess
				.ApplyAssetsFilter(Scenarios.ScenarioList, Scenarios.ScenarioFilter)
				.OrderBy(x => x.ScenarioTitle).ToList();
			NotifyOfPropertyChange(() => Scenarios);
			}

		public bool CanRouteProperties
			{
			get
				{
				return SelectedRoute != null;
				}
			}

		public async Task RouteProperties()
			{
			var routePropertiesVM = IoC.Get<RoutePropertiesViewModel>();
			routePropertiesVM.Route = SelectedRoute;
			await _windowManager.ShowWindowAsync(routePropertiesVM);
			}

		public async Task GetRouteAssets()
			{
			var routeAssetsVM = IoC.Get<RouteAssetsViewModel>();
			routeAssetsVM.Route = SelectedRoute;
			await _windowManager.ShowWindowAsync(routeAssetsVM);
			}

		private void GetScenarios(RouteModel RouteModel)
			{
			// Two step approach. In the first step we only do this for the packed scenarios, then we retrieve the unpacked scenarios, combine all in the database
			// Finally, we read the database and apply filtering and sorting.

			if (RouteModel.IsPacked)
				{
				Scenarios.ScenarioList = ScenarioCollectionDataAccess.ReadPackedListScenariosFromDisk(
					Settings.GameRoutesFolder,
					SelectedRoute.RouteGuid, true, false);
				ScenarioCollectionDataAccess.SaveScenariosBulkList(Scenarios.ScenarioList, true, false);
				}
			Scenarios.ScenarioList = ScenarioCollectionDataAccess
					.ReadScenariosFromDisk(Settings.GameRoutesFolder,
						SelectedRoute.RouteGuid, true, false);
			ScenarioCollectionDataAccess.SaveScenariosBulkList(Scenarios.ScenarioList, true, false);
			Scenarios.ScenarioList = ScenarioCollectionDataAccess
								.ReadScenariosFromDatabase(SelectedRoute.RouteGuid);
			Scenarios.FilteredScenarioList = ScenarioCollectionDataAccess
								.ApplyAssetsFilter(Scenarios.ScenarioList, Scenarios.ScenarioFilter)
								.OrderBy(x => x.ScenarioTitle).ToList();
			}

		public bool CanCopyToGame
			{
			get
				{
				return SelectedRoute != null && SelectedRoute.IsValidInArchive && SelectedRoute.InArchive;
				}
			}

		public async Task CopyToGame()
			{
			string source= $"{Settings.ArchiveRoutesFolder}\\{SelectedRoute.RouteGuid}\\";
			string destination= $"{Settings.GameRoutesFolder}\\{SelectedRoute.RouteGuid}\\";
			SelectedRoute.InGame = true;
			SelectedRoute.IsValidInGame = true;
			await CopyRoute(source, destination, false);
			NotifyOfPropertyChange(()=>SelectedRoute);
			}

		public bool CanCopyToArchive
			{
			get
				{
				return SelectedRoute != null && SelectedRoute.IsValidInGame && SelectedRoute.InGame;
				}
			}

		public async Task CopyToArchive()
			{
			string source= $"{Settings.GameRoutesFolder}\\{SelectedRoute.RouteGuid}\\";
			string destination= $"{Settings.ArchiveRoutesFolder}\\{SelectedRoute.RouteGuid}\\";
			SelectedRoute.InArchive = true;
			SelectedRoute.IsValidInArchive = true;
			await CopyRoute(source, destination, true);
			NotifyOfPropertyChange(()=>SelectedRoute);
			}


		public async Task CopyRoute(string source, string destination, bool overwrite)
			{
			await FileIOHelper.CopyDirAsync(source,destination, overwrite);
			await RoutesCollectionDataAccess.UpdateRouteStatus(SelectedRoute.Id, SelectedRoute.InGame, SelectedRoute.IsValidInGame,
				SelectedRoute.InArchive, SelectedRoute.IsValidInArchive);
			RouteList = RoutesCollectionDataAccess.ReadAllRoutesFromDatabase();
			FilteredRouteList = new BindableCollection<RouteModel>(
				RoutesCollectionDataAccess.ApplyAssetsFilter(RouteList, RouteFilter).OrderBy(x => x.RouteName));
			}
		public async Task Exit()
			{
			await TryCloseAsync();
			}
		}
	}

