using FancyTrainsimTools.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FancyTrainsimTools.Library.Manuals
  {
  public class ManualsLogic
    {
    public FileTreeModel Tree { get; set; } =new FileTreeModel();

    public ManualsLogic(string ManualsFolderPath)
      {
      FillManualsList(ManualsFolderPath);
      }

    public void FillManualsList(string ManualsFolderPath)
      {
      var Dir = new DirectoryInfo(ManualsFolderPath);
      Tree.FileTree = new TreeItemProvider();
      Tree.TreeItems = Tree.FileTree.GetItems(Dir.FullName);
      }
    }
  }
