using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class ConsistViewModel: Screen
		{
		private IWindowManager _windowManager;
		public ScenarioModel Scenario { get; set; }
		public List<ConsistModel> ConsistList { get; set; }

		private BindableCollection<InstructionModel> _instructionList;

		public BindableCollection<InstructionModel> InstructionList
			{
			get { return _instructionList; }
			set { _instructionList = value; }
			}


		private BindableCollection<ConsistModel> _filteredConsistList;	

		public BindableCollection<ConsistModel> FilteredConsistList
			{
			get { return _filteredConsistList; }
			set { _filteredConsistList = value; }
			}

		private ConsistModel _selectedConsist;

		public ConsistModel SelectedConsist
			{
			get { return _selectedConsist; }
			set
				{
				_selectedConsist = value;
				if (SelectedConsist == null)
					{
					FilteredRailVehicleList = null;
					}
				else
					{
					FilteredRailVehicleList =
						new BindableCollection<FullRailVehicleModel>(SelectedConsist.RailVehicleList);
					NotifyOfPropertyChange(()=>FilteredRailVehicleList);
					InstructionList= new BindableCollection<InstructionModel>(SelectedConsist.InstructionList);
					NotifyOfPropertyChange(()=>InstructionList);
					NotifyOfPropertyChange(()=>CanShowInstructionDetails);
					NotifyOfPropertyChange(()=>SelectedInstruction);
					}
				}
			}

		private List<FullRailVehicleModel> _railVehicleList;

		public List<FullRailVehicleModel> RailVehicleList
			{
			get { return _railVehicleList; }
			set { _railVehicleList = value; }	
			}

		private BindableCollection<FullRailVehicleModel> _filteredRailVehicleList;

		public BindableCollection<FullRailVehicleModel> FilteredRailVehicleList
			{
			get { return _filteredRailVehicleList; }
			set { _filteredRailVehicleList = value; }	
			}

		public List<FullRailVehicleModel> RequiredRailVehicles { get; set; }

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

		private InstructionModel _selectedInstruction;

		public InstructionModel SelectedInstruction
			{
			get { return _selectedInstruction; }
			set
				{
				_selectedInstruction = value;
				NotifyOfPropertyChange(()=>CanShowInstructionDetails);
				}
			}


		public ConsistViewModel(IWindowManager windowManager)
			{
			_windowManager = windowManager;
			}

		protected override async void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			if (Scenario.ScenarioProperties.ConsistList == null)
				{
				ConsistList =
					ConsistDataAccess.GetAllConsistsForScenario(Scenario.Id,
						Scenario.ScenarioProperties.BinDoc);
				await ConsistDataAccess.SaveConsistsToDatabaseMaster(ConsistList);
				Scenario.ScenarioProperties.ConsistList = ConsistList;
				}
			else
				{
				ConsistList = Scenario.ScenarioProperties.ConsistList;
				}

			if (Scenario.ScenarioProperties.RequiredRailVehicles == null)
				{
				RequiredRailVehicles = await FullRailVehicleDataAccess.GetRequiredRailVehiclesForScenario(Scenario.Id);
				Scenario.ScenarioProperties.RequiredRailVehicles = RequiredRailVehicles;
				}
			else
				{
				RequiredRailVehicles = Scenario.ScenarioProperties.RequiredRailVehicles;
				}

			FilteredConsistList= new BindableCollection<ConsistModel>(ConsistList);
			FilteredRequiredRailVehicleList =
					new BindableCollection<FullRailVehicleModel>(RequiredRailVehicles);
			}

		public bool CanShowInstructionDetails
			{
			get
				{
				return SelectedConsist!=null && SelectedConsist.IsLoose==false && SelectedConsist.IsEmpty == false && SelectedInstruction != null;
				}
			}

		public async Task  ShowInstructionDetails()
			{
			var instructionDetailVM = IoC.Get<InstructionDetailsViewModel>();
			instructionDetailVM.Instruction = SelectedInstruction;
			await _windowManager.ShowDialogAsync(instructionDetailVM);
			}

		public async Task Exit()
			{
			await TryCloseAsync();
			}

		}
	}