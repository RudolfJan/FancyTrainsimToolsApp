using System.Collections.ObjectModel;
using System.IO;

namespace FancyTrainsimToolsDesktop
	{
	public class TreeItemProvider
		{
		public ObservableCollection<FileEntryModel> GetItems(string Path, bool Always = false)
			{
			var Items = new ObservableCollection<FileEntryModel>();

			var DirInfo = new DirectoryInfo(Path);
      foreach (var Directory in DirInfo.GetDirectories())
        {
        var DirItem = new DirectoryItem
          {
          Name = Directory.Name,
          Path = Directory.FullName,
          DirectoryItems = GetItems(Directory.FullName, Always)
          };
        Items.Add(DirItem);
        }

			foreach (var File in DirInfo.GetFiles())
				{
				var Item = new FileItem
					{
					Name = File.Name,
					Path = File.FullName,
					};
				Items.Add(Item);
				}
			return Items;
			}

		// Will only return directories 
		public ObservableCollection<FileEntryModel> GetDirItems(string Path)
			{
			var Items = new ObservableCollection<FileEntryModel>();

			var DirInfo = new DirectoryInfo(Path);
      foreach (var Directory in DirInfo.GetDirectories())
				{
				var DirItem = new DirectoryItem
          {
          Name = Directory.Name,
          Path = Directory.FullName
          };
				DirItem.DirectoryItems = GetDirItems(Directory.FullName);
				Items.Add(DirItem);
				}
			return Items;
			}
		}
  }
