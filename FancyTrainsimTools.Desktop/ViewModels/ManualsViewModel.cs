using Caliburn.Micro;
using FancyTrainsimToolsDesktop.Helpers;
using Logging.Library;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Utilities.Library.TreeBuilders;

namespace FancyTrainsimToolsDesktop.ViewModels
  {
  public class ManualsViewModel: Screen
    {
    // I use the lower level ICommand directly, because in a TreeView SelectedItem is a read only property. This is not working in Caliburn.Micro
    public ICommand ShowManualCommand { get; set; }
    public ICommand OpenFolderCommand { get; set; }

    //private FileTreeModel _tree;
    //public FileTreeModel Tree
    //  {
    //  get
    //    {
    //    return _tree;
    //    }
    //  set { _tree = value; }
    //  }
 
    protected override void OnViewLoaded(object view)
      {
      base.OnViewLoaded(view);
      //Tree = TreeBuilder.BuildTree(Settings.ManualFolder);
      // TODO finish this part
      ShowManualCommand= new RelayCommand<string>(ShowManual,CanUseSelectedFile);
      OpenFolderCommand= new RelayCommand<string>(OpenFolder, CanUseSelectedFolder);
      }
 
    public static bool CanUseSelectedFile(string filePath)
      {
      if (!String.IsNullOrEmpty(filePath))
        {
        // make sure this is a file and not a folder
        var attr = File.GetAttributes(filePath);
        return !attr.HasFlag(FileAttributes.Directory);
        }
      return false;
      }
    public static bool CanUseSelectedFolder(string filePath)
      {
      if (!String.IsNullOrEmpty(filePath))
        {
        // make sure this is a folder and not a file
        var attr = File.GetAttributes(filePath);
        return attr.HasFlag(FileAttributes.Directory);
        }
      return false;
      }

    public ManualsViewModel()
      {
      //Tree = TreeBuilder.BuildTree(Settings.ManualFolder);
      }

    public void ShowManual(string filePath)
      {
      try
        {
        if (File.Exists(filePath))
          {
          using (var OpenFileProcess = new Process())
            {
            OpenFileProcess.StartInfo.FileName = "explorer.exe";
            OpenFileProcess.StartInfo.Arguments = filePath;
            OpenFileProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            OpenFileProcess.StartInfo.RedirectStandardOutput = false;
            OpenFileProcess.StartInfo.UseShellExecute = true;
            OpenFileProcess.Start();
            }
          }
        }
      catch (Exception e)
        {
        Log.Trace($"Cannot open directory {filePath} ",e, LogEventType.Error);

        }
      Log.Trace($"Cannot find file {filePath}\r\nMake sure to install it at the correct location");
      }

    // TODO move open folder to support class
    // Open folder from the application
    public void OpenFolder(string folderPath)
      {
      using (var EditScriptProcess = new Process())
        {
        if (!Directory.Exists(folderPath))
          {
          Log.Trace($"Directory does not exist {folderPath}");
          }
        try
          {
          EditScriptProcess.StartInfo.FileName = "explorer.exe";
          EditScriptProcess.StartInfo.Arguments = folderPath;
          EditScriptProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
          EditScriptProcess.StartInfo.RedirectStandardOutput = false;
          EditScriptProcess.StartInfo.UseShellExecute = false;
          EditScriptProcess.Start();
          }
        catch (Exception e)
          {
          Log.Trace($"Cannot open directory {folderPath} ",e, LogEventType.Error);
          }
        }
      }
    }
  }
