#region UsingStatements
using System;
using System.IO;
using System.Linq;
using System.Xml.Xsl;

#endregion

namespace Assets.Library.Helpers
  {
  #region AboutThisFile
  /// <summary>
  /// Purpose:
  /// Created by: rudol
  /// Created on: 1/16/2020 10:08:24 PM 
  /// </summary>
  #endregion
  public static class FilesAndDirectories
    {


    #region Properties

    #endregion

    #region Events

    public static EventHandler<string> MessageEvent;
    

    #endregion

 

    #region Methods
    // Safe way to delete a single file
    public static void DeleteSingleFile(string InputFilePath)
      {
      if (File.Exists(InputFilePath))
        {
        // Use a try block to catch IOExceptions, to
        // handle the case of the file already being
        // opened by another process.
        try
          {
          File.Delete(InputFilePath);
          }
        catch (IOException E)
          {
          MessageEvent?.Invoke(null, $"Cannot delete {InputFilePath} because {E.Message}");
          }
        }
      }

    // https://stackoverflow.com/questions/18996330/copying-files-and-subdirectories-to-another-directory-with-existing-files
    public static void CopyDir(string FromFolder, string ToFolder, bool Overwrite = false)
      {
      Directory
        .EnumerateFiles(FromFolder, "*.*", SearchOption.AllDirectories)
        .AsParallel()
        .ForAll(From =>
          {
          var To = From.Replace(FromFolder, ToFolder);
          // Create directories if needed
          var ToSubFolder = Path.GetDirectoryName(To);
          if (!string.IsNullOrWhiteSpace(ToSubFolder))
            {
            Directory.CreateDirectory(ToSubFolder);
            }

          try
            {
            File.Copy(From, To, Overwrite);
            }
          catch (IOException)
            {
            // Should be ignored here, do not copy if destination exists
            }
          });
      }





    #endregion

    #region Helpers
 


    #endregion


    }
  }
