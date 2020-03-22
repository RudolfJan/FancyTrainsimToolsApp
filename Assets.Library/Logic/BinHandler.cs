// ***********************************************************************
// Assembly         : LuaCreatorAssetsLibrary
// Author           : Rudolf  Heijink
// Created          : 01-16-2020
//
// Last Modified By : Rudolf  Heijink
// Last Modified On : 01-18-2020
// ***********************************************************************
// <summary></summary>
// ***********************************************************************

using Assets.Library.Helpers;
using Logging.Library;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace Assets.Library.Logic
  {
  /// <summary>
  /// Class BinHandler.
  /// </summary>
  public class BinHandler
    {
    /// <summary>
    /// The serz path
    /// </summary>
    private static string _SerzPath = @"C:\ConEmu\serz.exe";

    // Run Serz on an existing file and store the result into a temporary file.
    /// <summary>
    /// Serz to file.
    /// </summary>
    /// <param name="InputFilePath">The input file path.</param>
    /// <param name="OutputFilePath">The output file path.</param>
    public static void SerzToFile(string InputFilePath, string OutputFilePath)
      {
      if (!File.Exists(_SerzPath))
        {
        Log.Trace($"Serz application not found at {_SerzPath}",null, LogEventType.Error);
        return;
        }

      if (!File.Exists(InputFilePath))
        {
        Log.Trace($"DecodeSerz, input file not found {InputFilePath}",null, LogEventType.Error);
        return;
        }

      try
        {
        var Args = $"\"{InputFilePath}\" /:\"{OutputFilePath}\"";
        var StartSerz =
          new ProcessStartInfo(_SerzPath, Args)
            {
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true
            };
        using (var Serz = Process.Start(StartSerz))
          {
          if (Serz != null)
            {
            Serz.WaitForExit(1000 * 20 /* ms */);
            if (!Serz.HasExited)
              {
              Serz.Kill();
              Log.Trace($"Serz issue, Serz killed for {InputFilePath}",null, LogEventType.Error);
              return;
              }
            Serz.Close();
            }
          }
        }
      catch (Win32Exception E)
        {
        if (E.NativeErrorCode == SystemErrorCodes.ErrorFileNotFound)
          {
          // CLog.Trace(E.Message + ". Check the path. Executing Serz.exe", LogEventType.Error);
          Log.Trace($"{E.Message}. Check the path. Executing Serz.exe",null,LogEventType.Error);
          return;
          }

        if (E.NativeErrorCode == SystemErrorCodes.ErrorAccessDenied)
          {
          // Note that if your word processor might generate exceptions
          // such as this, which are handled first.
          Log.Trace($"You do not have permission to execute Serz for this file.", E,LogEventType.Error);
          }
        }
      }

    /// <summary>
    /// Files to document.
    /// </summary>
    /// <param name="InputFilePath">The input file path.</param>
    /// <returns>XDocument.</returns>
    public static XDocument FileToDoc(string InputFilePath)
      {
      XDocument output;
      if (!File.Exists(InputFilePath))
        {
        Log.Trace($"{InputFilePath}. File does not exist",null, LogEventType.Error);
        return null;
        }

      try
        {
        output = XDocument.Load(InputFilePath);
        }
      catch (Exception e)
        {
        Log.Trace(
          $"{InputFilePath} Failed to open XML document",e,LogEventType.Error);
        return null;
        }

      return output;
      }
    /// <summary>
    /// Serzs to document.
    /// </summary>
    /// <param name="InputFilePath">The input file path.</param>
    /// <returns>XDocument.</returns>
    public static XDocument SerzToDoc(string InputFilePath)
      {
      string TempFile= $"{Path.GetTempFileName()}.xml";
      SerzToFile(InputFilePath,TempFile);
      var output = FileToDoc(TempFile);
      FilesAndDirectories.DeleteSingleFile(TempFile);
      return output;
      }

    }
  }
