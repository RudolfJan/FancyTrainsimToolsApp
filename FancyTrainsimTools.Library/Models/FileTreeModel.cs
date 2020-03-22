using FancyTrainsimTools.Library.Manuals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FancyTrainsimTools.Library.Models
  {
  public class FileTreeModel
    {
    public ObservableCollection<FileEntryModel> TreeItems { get; set; }
    public TreeItemProvider FileTree { get; set; }
    }
  }
