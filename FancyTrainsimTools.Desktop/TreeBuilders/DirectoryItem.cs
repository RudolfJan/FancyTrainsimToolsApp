using System.Collections.ObjectModel;
using System.IO;

namespace FancyTrainsimToolsDesktop
	{
	public class DirectoryItem : FileEntryModel
		{
		public ObservableCollection<FileEntryModel> DirectoryItems { get; set; } =
			new ObservableCollection<FileEntryModel>();

		public DirectoryInfo DirectoryDetails { get; set; }

		public DirectoryItem()
			{
			
			}
		}
  }
