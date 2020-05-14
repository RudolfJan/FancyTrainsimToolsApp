using Microsoft.Win32;
using System.Collections.Generic;

namespace FancyTrainsimTools.Desktop.Models
	{
	// https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.savefiledialog?view=netcore-3.1
	public class SaveFileModel
		{
		public bool CheckFileExists { get; set; } = false;
		public bool CheckPathExists { get; set; } = true;
		public bool CreatePrompt { get; set; } = false;
		// https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.filedialogcustomplace?view=netcore-3.1
		public IList<FileDialogCustomPlace> CustomPlaces { get; set; }
		public string DefaultExt { get; set; } = "*.csv";
		public bool DereferenceLinks { get; set; } = false;
		public string FileName { get; set; }
		public string[] FileNames { get; set; }
		public string Filter { get; set; } ="Csv files|*.csv|All Files|*.*";
		public int FilterIndex { get; set; } =1;
		public string InitialDirectory { get; set; } = "";
		public bool OverWriteprompt { get; set; } =true;
		public string SafeFileName { get;set; }
		public string[] SafeFileNames { get; set;}
		public string Title { get; set; } = "Save file";

		}
	}
