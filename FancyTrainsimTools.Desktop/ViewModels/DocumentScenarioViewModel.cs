using Assets.Library.Helpers;
using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using Logging.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class DocumentScenarioViewModel : Screen
		{
		#region Properties
		/*
 Scenario to be documented
 */
		public ScenarioModel Scenario { get; set; }
		public RouteModel Route { get; set; }

		public string ScenarioPath { get; set; }
		/*
    Path to the HTML template
    */
		private FileInfo _TemplatePath = null;
		public FileInfo TemplatePath
			{
			get { return _TemplatePath; }
			set
				{
				_TemplatePath = value;
				NotifyOfPropertyChange(() => CanUpdate);
				NotifyOfPropertyChange(() => CanViewInBrowser);
				NotifyOfPropertyChange(() => CanDelete);
				NotifyOfPropertyChange(() => CanCreate);
				NotifyOfPropertyChange(() => CanEditTemplate);
				}
			}

		/*
    List of all available HTML templates
    */
		private BindableCollection<FileInfo> _TemplateList = null;
		public BindableCollection<FileInfo> TemplateList
			{
			get { return _TemplateList; }
			set
				{
				_TemplateList = value;
				}
			}

		/*
    Path the the documentation file to be generated
    */
		private String _DocumentationPath = String.Empty;
		public string DocumentationPath
			{
			get { return _DocumentationPath; }
			set
				{
				_DocumentationPath = value;
				}
			}

		/*
    Unformatted and unchecked output for debug purposes
    */
		private String _RawOutput = String.Empty;
		public string RawOutput
			{
			get { return _RawOutput; }
			set
				{
				_RawOutput = value;
				}
			}

		// Dictionary holding macro names and values
		private Dictionary<String, String> Replacements = new Dictionary<String, String>();

		#endregion

		#region Initialization

		protected override void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			ScenarioPath = $"{Settings.TrainSimGamePath}Content\\Routes\\{Route.RouteGuid}\\Scenarios\\{Scenario.ScenarioGuid}\\";
			DocumentationPath = ScenarioPath + "Documentation.html";
			TemplateList = BuildTemplateList();
			}

		private BindableCollection<FileInfo> BuildTemplateList()
			{
			DirectoryInfo dir = new DirectoryInfo($"{Settings.TemplatesPath}DocumentTemplates");
			FileInfo[] Files = dir.GetFiles();
			return new BindableCollection<FileInfo>(dir.GetFiles().ToList());
			}
		#endregion

		#region BuildLogic

		public bool CanCreate
			{
			get
				{
				return TemplatePath != null && !File.Exists(DocumentationPath);
				}
			}

		public void Create()
			{
			CreateDocumentation();
			NotifyOfPropertyChange(() => CanUpdate);
			NotifyOfPropertyChange(() => CanViewInBrowser);
			NotifyOfPropertyChange(() => CanDelete);
			NotifyOfPropertyChange(() => CanCreate);
			NotifyOfPropertyChange(() => RawOutput);
			}

		public bool CanUpdate
			{
			get
				{
				return TemplatePath != null && File.Exists(DocumentationPath);
				}
			}

		public void Update()
			{
			FileIOHelper.DeleteSingleFile(DocumentationPath);
			CreateDocumentation();
			NotifyOfPropertyChange(() => RawOutput);
			}

		public bool CanDelete
			{
			get
				{
				return File.Exists(DocumentationPath);
				}
			}

		public void Delete()
			{
			FileIOHelper.DeleteSingleFile(DocumentationPath);
			NotifyOfPropertyChange(() => CanUpdate);
			NotifyOfPropertyChange(() => CanViewInBrowser);
			NotifyOfPropertyChange(() => CanDelete);
			NotifyOfPropertyChange(() => RawOutput);
			}

		public bool CanEditTemplate
			{
			get
				{
				return TemplatePath != null;
				}
			}

		public void EditTemplate()
			{
			FileIOHelper.EditTextFile(TemplatePath.FullName, Settings.TextEditor);
			}

		public bool CanViewInBrowser
			{
			get
				{
				return File.Exists(DocumentationPath);
				}
			}

		public void ViewInBrowser()
			{
			FileIOHelper.OpenFileWithShell(DocumentationPath);
			}
		#endregion

		#region HouseKeeping

		public async Task Exit()
			{
			await TryCloseAsync();
			}

		#endregion

		#region BuildDocumentation

		public void CreateDocumentation()
			{

			// try to open the template
			try
				{
				RawOutput = String.Empty;
				var AllText = String.Empty;
				using (var input = new StreamReader(TemplatePath.FullName))
					{
					AllText = input.ReadToEnd();
					input.Close();
					}
				RawOutput = ReplaceAll(AllText);
				var Doc = XDocument.Parse(RawOutput);
				XmlHelpers.Save(Doc, DocumentationPath);
				}
			catch (Exception ex)
				{
				Log.Trace("Failed to create Documentation for Scenario. ", ex, LogEventType.Error);
				}
			}

		private void BuildDictionary()
			{
			Replacements.Add("{ScenarioTitle}", Scenario.ScenarioTitle);
			Replacements.Add("{RouteName}", Route.RouteName);
			Replacements.Add("{Description}", Scenario.ScenarioProperties.Description);
			Replacements.Add("{Author}", Scenario.ScenarioProperties.Author);
			Replacements.Add("{Rating}", Scenario.ScenarioProperties.Rating);
			Replacements.Add("{PlayerEngine}", Scenario.ScenarioProperties.PlayerEngine);
			Replacements.Add("{ScenarioType}", Scenario.ScenarioClass);
			Replacements.Add("{Season}", Scenario.ScenarioProperties.Season);
			Replacements.Add("{Duration}", Scenario.ScenarioProperties.Duration);
			Replacements.Add("{RollingStockList}", GetRollingStockTable());
			Replacements.Add("{PlayerEngineInstructions}", GetInstructions());
			}

		private string ReplaceAll(string line)
			{
			// Define replacement strings
			BuildDictionary();
			foreach (var x in Replacements)
				{
				line = line.Replace(x.Key, x.Value);
				}
			return line;
			}

		private string GetRollingStockTable()
			{
			string table = GetRollingStockTableHeading();
			foreach (var item in Scenario.ScenarioProperties.RequiredRailVehicles)
				{
				table += AddRailvehicle(item);
				}
			table += "</table>\n";
			return table;
			}

		private string GetRollingStockTableHeading()
			{
			string Heading = String.Empty;

			Heading += "<table>\n";
			Heading += "<tr class=\"first\">\n<td>Name</td><td>Provider</td><td>Product</td><td>Blueprint</td>\n</tr>\n";

			return Heading;
			}


		private string AddRailvehicle(FullRailVehicleModel rv)
			{
			string RvLine;
			// TODO add downloadlocations to assets as option
			RvLine = "<tr>\n<td>" + rv.DisplayName + "</td>\n";
			RvLine += "<td>" + rv.Provider + "</td>\n";
			RvLine += "<td>" + rv.Product + "</td>\n";
			RvLine += "<td>" + rv.BlueprintPath + "</td>\n</tr>\n";

			return RvLine;
			}

		private string GetInstructionsHeader()
			{
			String Heading = String.Empty;
			Heading += "<table>\n";
			Heading += "<tr class=\"first\">\n<td>Instruction</td><td>Location</td><td>Timetabled</td><td>Due time</td>\n</tr>\n";

			return Heading;
			}

		private String AddInstruction(InstructionModel instruction)
			{
			String Output = String.Empty;

			Output += "<tr>\n<td>" + instruction.InstructionType + "</td>\n";
			Output += "<td>" + instruction.Location + "</td>\n";
			Output += "<td>" + instruction.TimeTabled + "</td>\n";
			Output += "<td>" + instruction.DueTimeString + "</td>\n";
			Output += "</tr>\n";
			return Output;
			}

		private string GetInstructions()
			{
			var instructionList = ConsistDataAccess.GetPlayerInstructions(Scenario.ScenarioProperties.ConsistList);
			string instructions = string.Empty;
			instructions += GetInstructionsHeader();
			foreach (var item in instructionList)
				{
				instructions += AddInstruction(item);
				}
			instructions += "</table>\n";
			return instructions;
			}
		#endregion
		}
	}
