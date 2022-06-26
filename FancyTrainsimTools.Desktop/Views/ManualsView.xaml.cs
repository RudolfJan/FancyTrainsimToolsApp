using System;
using System.Windows;
using TreeBuilders.Library.Wpf;

namespace FancyTrainsimToolsDesktop.Views
  {
  /// <summary>
  /// Interaction logic for ManualsView.xaml
  /// </summary>
  public partial class ManualsView
    {
    public FileTreeViewModel Tree { get; set; }
    public ManualsView(string rootFolder)
      {
      InitializeComponent();
      FileTreeViewControl.FolderImage = "Images\\folder.png";
      FileTreeViewControl.FileImage = "Images\\file_extension_doc.png";
      FileTreeViewControl.SetImages();
      Tree = new FileTreeViewModel(rootFolder);
      FileTreeBuilder.RenameFilesToUnquoted(Tree.FileTree);
      FileTreeViewControl.Tree = Tree;
      FileTreeViewControl.DataContext = Tree;
      }

   private void OKButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Close();
      }
    }
  }
