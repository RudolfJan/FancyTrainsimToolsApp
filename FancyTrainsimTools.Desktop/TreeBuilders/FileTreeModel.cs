using System.Collections.ObjectModel;

namespace FancyTrainsimToolsDesktop
	{
	public class FileTreeModel
    {
    public ObservableCollection<FileEntryModel> TreeItems { get; set; }
    public TreeItemProvider FileTree { get; set; }
    }
  }
