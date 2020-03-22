using FancyTrainsimTools.Library.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace FancyTrainsimTools.Library.Manuals
  {
  public class DirectoryItem : FileEntryModel
		{
		public ObservableCollection<FileEntryModel> DirectoryItems { get; set; } =
			new ObservableCollection<FileEntryModel>();

		public DirectoryInfo DirectoryDetails { get; set; }
		}
  }
