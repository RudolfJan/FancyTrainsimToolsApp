using Logging.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Assets.Library.Logic
	{
	public class SevenZipDataAccess
		{
		// You need to tell this library where to find the 7Zip program
		public static string SevenZipProgramLocation;

		// Add quotes to a filename in case it contains spaces. If the filepath is already quoted, don't do it again 
		private static string QuoteFilename(string s)
			{
			if (s.StartsWith("\"") && s.EndsWith("\""))
				{
				return s; // already quoted
				}
			else
				return $"\"{s}\"";
			}
		private static string GetSevenZipProgram()
			{
			if (String.IsNullOrEmpty(SevenZipProgramLocation))
				{
				throw new NotImplementedException("You need to set the SevenZip program location properly before you can use it");
				}

			if (!File.Exists(SevenZipProgramLocation))
				{
				throw new NotImplementedException($"SevenZip program not found at {SevenZipProgramLocation} ");
				}
			return QuoteFilename(SevenZipProgramLocation);;
			}


		public static List<FileInfo> GetInstallerFiles(DirectoryInfo sourceDirectory)
			{

			var sourceFileList = new List<FileInfo>();
			foreach (var File in sourceDirectory.GetFiles("*.zip"))
				{
				sourceFileList.Add(File);
				}
			foreach (var File in sourceDirectory.GetFiles("*.rwp"))
				{
				sourceFileList.Add(File);
				}

			foreach (var File in sourceDirectory.GetFiles("*.rpk"))
				{
				sourceFileList.Add(File);
				}

			foreach (var File in sourceDirectory.GetFiles("*.rar"))
				{
				sourceFileList.Add(File);
				}
			foreach (var File in sourceDirectory.GetFiles("*.7z"))
				{
				sourceFileList.Add(File);
				}
			foreach (var File in sourceDirectory.GetFiles("*.exe"))
				{
				sourceFileList.Add(File);
				}
			return sourceFileList;
			}


			// This extraction will NOT take into account the filepath
		public static String ExtractZipFile(String Archive, String ArchiveEntry, String OutputDirectory)
			{
			if (File.Exists(Archive))
				{
				using (var MyProcess = new Process())
					{
					try
						{
						MyProcess.StartInfo.FileName = GetSevenZipProgram();
						MyProcess.StartInfo.Arguments =
							"-y e \"" + Archive + "\" -o\"" + OutputDirectory + "\" \"" + ArchiveEntry + "\"";
						MyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						MyProcess.StartInfo.CreateNoWindow = true;
						MyProcess.StartInfo.RedirectStandardOutput = true;
						MyProcess.StartInfo.RedirectStandardError = true;
						MyProcess.StartInfo.UseShellExecute = false;
						MyProcess.Start();
						// ReSharper disable once UnusedVariable
						var Stdout = MyProcess.StandardOutput.ReadToEnd();
						var Stderr = MyProcess.StandardError.ReadToEnd();
						MyProcess.WaitForExit();
						if (Stderr.Length > 0)
							{
							return Log.Trace(
								"Error extracting compressed file " + Archive + " message: " + Stderr,
								LogEventType.Message);
							}

						return String.Empty;
						}
					catch (Exception E)
						{
						return Log.Trace("Cannot extract compressed file " + Archive + " reason: " + E.Message,
							LogEventType.Error);
						}
					}
				}
			return String.Empty;
			}

		// Extract a single file, take filepath into account
		public static String SevenZipExtractSingle(String Archive, String OutputDirectory,
			String FullName)
			{
			var Stdout = String.Empty;
			if (File.Exists(Archive))
				{
				using (var MyProcess = new Process())
					{
					try
						{
						MyProcess.StartInfo.FileName = GetSevenZipProgram();
						MyProcess.StartInfo.Arguments =
							"-y x \"" + Archive + "\" -o\"" + OutputDirectory + "\" " + FullName;
						MyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						MyProcess.StartInfo.CreateNoWindow = true;
						MyProcess.StartInfo.RedirectStandardOutput = true;
						MyProcess.StartInfo.RedirectStandardError = true;
						MyProcess.StartInfo.UseShellExecute = false;
						MyProcess.Start();
						Stdout = MyProcess.StandardOutput.ReadToEnd();
						var Stderr = MyProcess.StandardError.ReadToEnd();
						MyProcess.WaitForExit();
						if (Stderr.Length > 0)
							{
							return Log.Trace(
								"Error extracting compressed files " + Archive + " message: " + Stderr,
								LogEventType.Message);
							}
						return Stdout;
						}
					catch (Exception E)
						{
						return Log.Trace("Cannot extract compressed file " + Archive + " reason: " + E.Message,
							LogEventType.Error);
						}
					}
				}
			return Stdout;
			}

		// Extract all files, take filepath into account
		public static String SevenZipExtractAll(String Archive, String OutputDirectory,
			String Filter = "*",
			Boolean Recursive = false)
			{
			var Stdout = String.Empty;
			var RecursiveOption = String.Empty;
			if (Recursive)
				{
				RecursiveOption = " -r";
				}

			if (File.Exists(Archive))
				{
				using (var MyProcess = new Process())
					{
					try
						{
						MyProcess.StartInfo.FileName = GetSevenZipProgram();
						MyProcess.StartInfo.Arguments =
							"-y x \"" + Archive + "\" -o\"" + OutputDirectory + "\" " + Filter + RecursiveOption;
						MyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						MyProcess.StartInfo.CreateNoWindow = true;
						MyProcess.StartInfo.RedirectStandardOutput = true;
						MyProcess.StartInfo.RedirectStandardError = true;
						MyProcess.StartInfo.UseShellExecute = false;
						MyProcess.Start();
						Stdout = MyProcess.StandardOutput.ReadToEnd();
						var Stderr = MyProcess.StandardError.ReadToEnd();
						MyProcess.WaitForExit();
						if (Stderr.Length > 0)
							{
							return Log.Trace(
								"Error extracting compressed files " + Archive + " message: " + Stderr,
								LogEventType.Message);
							}

						return Stdout;
						}
					catch (Exception E)
						{
						return Log.Trace(
							"Cannot extract compressed file " + Archive + " reason: " + E.Message,
							LogEventType.Error);
						}
					}
				}
			return Stdout;
			}

		//public static String ListZipFiles(String FilePath, out String Stdout)
		//	{
		//	Stdout = String.Empty;
		//	if (File.Exists(FilePath))
		//		{
		//		using (var MyProcess = new Process())
		//			{
		//			try
		//				{
		//				MyProcess.StartInfo.FileName = CLuaCreatorOptions.SevenZip;
		//				MyProcess.StartInfo.Arguments = "-r l \"" + FilePath + "\"";
		//				MyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		//				MyProcess.StartInfo.CreateNoWindow = true;
		//				MyProcess.StartInfo.RedirectStandardOutput = true;
		//				MyProcess.StartInfo.RedirectStandardError = true;
		//				MyProcess.StartInfo.UseShellExecute = false;
		//				MyProcess.Start();
		//				Stdout = MyProcess.StandardOutput.ReadToEnd();
		//				var Stderr = MyProcess.StandardError.ReadToEnd();
		//				MyProcess.WaitForExit();
		//				if (Stderr.Length > 0)
		//					{
		//					return CLog.Trace("Error list zip file " + FilePath + " message: " + Stderr,
		//						LogEventType.Message);
		//					}

		//				return String.Empty;
		//				}
		//			catch (Exception E)
		//				{
		//				return CLog.Trace("Cannot list zip file " + FilePath + " reason: " + E.Message,
		//					LogEventType.Error);
		//				}
		//			}
		//		}

		//	return String.Empty;
		//	}








		public static String ListZipFiles(String FilePath, out String Stdout)
			{
			Stdout = String.Empty;
			if (File.Exists(FilePath))
				{
				using (var MyProcess = new Process())
					{
					try
						{
						MyProcess.StartInfo.FileName = GetSevenZipProgram();
						MyProcess.StartInfo.Arguments = "-r l \"" + FilePath + "\"";
						MyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						MyProcess.StartInfo.CreateNoWindow = true;
						MyProcess.StartInfo.RedirectStandardOutput = true;
						MyProcess.StartInfo.RedirectStandardError = true;
						MyProcess.StartInfo.UseShellExecute = false;
						MyProcess.Start();
						Stdout = MyProcess.StandardOutput.ReadToEnd();
						var Stderr = MyProcess.StandardError.ReadToEnd();
						MyProcess.WaitForExit();
						if (Stderr.Length > 0)
							{
							return Log.Trace("Error list zip file " + FilePath + " message: " + Stderr,
								LogEventType.Message);
							}

						return String.Empty;
						}
					catch (Exception E)
						{
						return Log.Trace("Cannot list zip file " + FilePath + " reason: " + E.Message,
							LogEventType.Error);
						}
					}
				}

			return String.Empty;
			}


		}
	}
