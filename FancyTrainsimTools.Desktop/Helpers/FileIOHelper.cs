using FancyTrainsimTools.Desktop.Models;
using Logging.Library;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FancyTrainsimTools.Desktop.Helpers
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
