using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace FancyTrainsimTools.Desktop.Models
	{
	public class OpenFileModel
		{
		public bool CheckFileExists { get; set; } = false;
		public bool CheckPathExists { get; set; } = true;
		// https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.filedialogcustomplace?view=netcore-3.1
		public IList<FileDialogCustomPlace> CustomPlaces { get; set; }
		public string DefaultExt { get; set; } = "*.csv";
		public bool DereferenceLinks { get; set; } = false;
		public string FileName { get; set; }
		public string[] FileNames { get; set; }
		public string Filter { get; set; } ="Csv files|*.csv|All Files|*.*";
		public int FilterIndex { get; set; } =1;
		public string InitialDirectory { get; set; } = "";
		public string SafeFileName { get;set; }
		public string[] SafeFileNames { get; set;}
		public string Title { get; set; } = "Open file";
		}
	}
