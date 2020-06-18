using FancyTrainsimToolsDesktop.Models;
using Logging.Library;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.Helpers
	{
	public class FileIOHelper
		{
		public static string GetSaveFileName(SaveFileModel saveFileParams)
			{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.CheckFileExists = saveFileParams.CheckFileExists;
			dialog.CheckPathExists = saveFileParams.CheckPathExists;
			dialog.CreatePrompt = saveFileParams.CreatePrompt;
			dialog.CustomPlaces = saveFileParams.CustomPlaces;
			dialog.DefaultExt = saveFileParams.DefaultExt;
			dialog.DereferenceLinks = saveFileParams.DereferenceLinks;
			dialog.FileName = saveFileParams.FileName;
			// FileNames not supported!
			dialog.Filter = saveFileParams.Filter;
			dialog.FilterIndex = saveFileParams.FilterIndex;
			dialog.InitialDirectory = saveFileParams.InitialDirectory;
			dialog.OverwritePrompt = saveFileParams.OverWriteprompt;
			dialog.Title = saveFileParams.Title;
			if (dialog.ShowDialog() == true)
				{
				saveFileParams.FileName = dialog.FileName;
				saveFileParams.SafeFileName = dialog.SafeFileName;
				saveFileParams.SafeFileNames = dialog.SafeFileNames;
				return dialog.FileName;
				}
			return "";
			}

		public static string GetOpenFileName(OpenFileModel openFileParams)
			{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = openFileParams.CheckFileExists;
			dialog.CheckPathExists = openFileParams.CheckPathExists;
			dialog.CustomPlaces = openFileParams.CustomPlaces;
			dialog.DefaultExt =openFileParams.DefaultExt;
			dialog.DereferenceLinks = openFileParams.DereferenceLinks;
			dialog.FileName = openFileParams.FileName;
			// FileNames not supported!
			dialog.Filter = openFileParams.Filter;
			dialog.FilterIndex = openFileParams.FilterIndex;
			dialog.InitialDirectory = openFileParams.InitialDirectory;
			dialog.Title = openFileParams.Title;
			if (dialog.ShowDialog() == true)
				{
				openFileParams.FileName = dialog.FileName;
				openFileParams.SafeFileName = dialog.SafeFileName;
				openFileParams.SafeFileNames = dialog.SafeFileNames;
				return dialog.FileName;
				}
			return "";
			}



		// Safe way to delete a single file
		public static void DeleteSingleFile(string FilePath)
			{
			if (File.Exists(FilePath))
				{
				// Use a try block to catch IOExceptions, to
				// handle the case of the file already being
				// opened by another process.
				try
					{
					File.Delete(FilePath);
					}
				catch (Exception ex)
					{
					Log.Trace($"Cannot delete {FilePath} because ",ex, LogEventType.Message);
					}
				}
			}

		public static void CreateEmptyFile(string Filename)
			{
			File.Create(Filename).Dispose();
			}

		public static void OpenFileWithShell(string FilePath)
			{
			try
				{
				if (File.Exists(FilePath))
					{
					using (var OpenFileProcess = new Process())
						{
						OpenFileProcess.StartInfo.FileName = "explorer.exe";
						OpenFileProcess.StartInfo.Arguments = QuoteFilename(FilePath);
						OpenFileProcess.StartInfo.UseShellExecute = true;
						OpenFileProcess.StartInfo.RedirectStandardOutput = false;
						OpenFileProcess.Start();
						}
					}
				}
			catch (Exception E)
				{
				Log.Trace("Cannot open file " + FilePath + " reason: " + E.Message,
					LogEventType.Error);
				}

			Log.Trace("Cannot find file " + FilePath +
												" \r\nMake sure to install it at the correct location");
			}

		public static void OpenFolder(string FilePath)
			{
			try
				{
				if (Directory.Exists(FilePath))
					{
					using (var OpenFileProcess = new Process())
						{
						OpenFileProcess.StartInfo.FileName = "explorer.exe";
						OpenFileProcess.StartInfo.Arguments = QuoteFilename(FilePath);
						OpenFileProcess.StartInfo.UseShellExecute = false;
						OpenFileProcess.StartInfo.RedirectStandardOutput = false;
						OpenFileProcess.Start();
						}
					}
				}
			catch (Exception E)
				{
				Log.Trace("Cannot open folder " + FilePath + " reason: " + E.Message,
					LogEventType.Error);
				}
			}

		public static void EditTextFile(string Filepath, string Editor)
			{
			if (!File.Exists(Filepath))
				{
				CreateEmptyFile(Filepath);
				}

			using (Process EditProcess = new Process())
				{
				try
					{
					EditProcess.StartInfo.FileName = Editor;
					EditProcess.StartInfo.Arguments = QuoteFilename(Filepath);
					EditProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
					EditProcess.StartInfo.RedirectStandardOutput = false;
					EditProcess.Start();
					}
				catch (Exception ex)
					{
					Log.Trace($"Error editing file { Filepath }",ex, LogEventType.Error);
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

		public static async Task CopyDirAsync(string FromFolder, string ToFolder, bool Overwrite = false)
			{
			await Task.Run(() => Directory
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
					}));
			}

		public static void UnpackSingleFile(string ArchivePath, string FilePath, string tempFilePath)

			{
			if (!File.Exists(ArchivePath))
				{
				Log.Trace($"Archive {ArchivePath} not found",LogEventType.Error);
				 // Cannot open archive
				return;
				}
			try
				{
				using var Archive = ZipFile.OpenRead(ArchivePath);
				var Entry = Archive.Entries
					.Select(x => x)
					.Where(x => FilePath.Equals(x.FullName, StringComparison.OrdinalIgnoreCase));
				var Entry2 = Entry.First();
				Entry2?.ExtractToFile(tempFilePath, true);
				}
			catch (Exception ex)
				{
				Log.Trace($"Problem extracting file {FilePath}",ex, LogEventType.Error);
				return;
				}

			// Postcondition
			if (!File.Exists(tempFilePath))
				{
				Log.Trace($"Extracted file not found for file {FilePath} from {ArchivePath}",LogEventType.Error);
				}
			}
		public static void OpenZipFile(String filePath)
			{
			if (File.Exists(filePath))
				{
				using (var process = new Process())
					{
					try
						{
						process.StartInfo.FileName = Settings.SevenZip;
						process.StartInfo.Arguments = QuoteFilename(filePath);
						process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
						process.StartInfo.RedirectStandardOutput = false;
						process.Start();
						}
					catch (Exception ex)
						{
						Log.Trace($"Cannot open zip file {filePath}",ex, LogEventType.Error);
						}
					}
				}
			}

  // Run Serz on an existing file and store the result into a temporary file.
    public static void DecodeSerz(string inputFile, string tempFile, out bool success)
      {
      success = false;
      if (!File.Exists(Settings.SerzPath))
        {
        Log.Trace($"Serz application not found, looking for {Settings.SerzPath}",LogEventType.Error);
        return;
        }

      if (!File.Exists(inputFile))
        {
        Log.Trace($"DecodeSerz, input file not found {inputFile}",LogEventType.Error);
        return;
        }

      try
	      {
	      Process Serz;
	      var Args = $"\"{inputFile}\" /:\"{tempFile}\"";
        var StartSerz =
          new ProcessStartInfo(Settings.SerzPath, Args)
            {
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true
            };
        Serz = Process.Start(StartSerz);
        if (Serz != null)
          {
          Serz.WaitForExit(1000 * 20 /* ms */);
          if (!Serz.HasExited)
            {
            Serz.Kill();
            success = false;
            Log.Trace($"Serz issue, Serz killed for {inputFile}", LogEventType.Error);
            return;
            }

          success = true;
          Serz.Close();
          }
        }
      catch (Win32Exception ex)
        {
        if (ex.NativeErrorCode == SystemErrorCodes.ErrorFileNotFound)
          {
          success = false;
          Log.Trace("Check the path. Executing Serz.exe ",ex, LogEventType.Error);
          }
        else if (ex.NativeErrorCode == SystemErrorCodes.ErrorAccessDenied)
          {
          // Note that if your word processor might generate exceptions
          // such as this, which are handled first.
          success = false;
          Log.Trace($"You do not have permission to execute Serz for file {inputFile}", LogEventType.Error);
          }
        }
      }








		// Gets a base part for a temporary file, you need to append the original filename
		public static string GetTempBasePath()
			{
			return $"{Settings.TempFolder}{Converters.GetUuidString()}-";
			}

		public static string GetTempFilePath(string inputFileName)
			{
			return $"{Settings.TempFolder}{Converters.GetUuidString()}-{inputFileName}";
			}
		// Add quotes to a filename in case it contains spaces. If the filepath is already quoted, don't do it again 
		public static string QuoteFilename(string s)
			{
			if (s.StartsWith("\"") && s.EndsWith("\""))
				{
				return s; // already quoted
				}
			else
				return $"\"{s}\"";
			}
		}
	}
