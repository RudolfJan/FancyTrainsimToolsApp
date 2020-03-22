using FancyTrainsimTools.Library.Manuals;
using FancyTrainsimTools.Library.Models;
using Logging.Library;
using Mvvm.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class ManualsViewModel: BindableBase
    {
    public FileTreeModel Tree { get; set; }
    public ICommand ShowManualCommand { get; }
    public ICommand OpenManualsFolderCommand { get; }

    public ManualsViewModel()
      {
      ManualsLogic logic= new ManualsLogic(Settings.ManualFolder);
      Tree = logic.Tree;
      // TODO finish this part
      ShowManualCommand= new RelayCommand<string>(ShowManual,CanUseSelectedFile);
      OpenManualsFolderCommand= new RelayCommand<string>(OpenFolder, CanUseSelectedFolder);
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

    // TODO move this to support class
    public static void ShowManual(string filePath)
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

    // TODO move this to support class
    // Open folder from the application
    public static void OpenFolder(string folderPath)
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
