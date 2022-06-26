using Logging.Library;
using System;
using System.IO;

namespace Assets.Library.Models
  {
  public class InstallerFileModel
    {
		#region Properties
		public String Name { get; set; } = String.Empty;
		public String Extension { get; set; } = String.Empty;
		public String FullName { get; set; } = String.Empty;
		public DateTimeOffset CreationDate { get; set; }
		public Boolean IsInstalled { get; set; } = false;
		public Boolean IsOutdated { get; set; } = false;


		#endregion

		#region Constructors

		// Create using FullName for file, and optional creation Date/Time
		public InstallerFileModel(String MyFullName, DateTimeOffset MyDate)
      {
      FullName = MyFullName;
      Name = Path.GetFileName(FullName);
      Extension = Path.GetExtension(FullName);
      CreationDate = MyDate;
      }

    public InstallerFileModel(String MyFullName, String MyName, String MyExtension, DateTimeOffset MyDate)
      {
      FullName = MyFullName;
      Name = MyName;
      Extension = MyExtension;
      CreationDate = MyDate;
      }

    public InstallerFileModel(String MyFullName, String MyName, DateTimeOffset MyDate)
      {
      FullName = MyFullName;
      Name = MyName;
      Extension = Path.GetExtension(FullName);
      CreationDate = MyDate;
      }

    public InstallerFileModel()
      {
      }

    #endregion

    #region Converters

    public void Parse7ZLine(String Line)
      {
      if (Line.Length >= 53)
        {
        try
          {
          FullName = Line.Substring(53);
          Name = Path.GetFileName(FullName);
          Extension = Path.GetExtension(FullName);
          int Year = Convert.ToInt32(Line.Substring(0, 4));
          int Month = Convert.ToInt32(Line.Substring(5, 2));
          int Day = Convert.ToInt32(Line.Substring(8, 2));
          int Hours = Convert.ToInt32(Line.Substring(11, 2));
          int Minutes = Convert.ToInt32(Line.Substring(14, 2));
          TimeSpan Offset = new TimeSpan();
          CreationDate = new DateTimeOffset(Year, Month, Day, Hours, Minutes, 0, Offset);
          if (Month > 12)
            {
            Log.Trace("Conversion error", LogEventType.Debug);
            }
          }
        catch (Exception E)
          {
          Log.Trace("Conversion error in line " + Line + " because " + E.Message, LogEventType.Debug);
          }
        }
      }

    #endregion

    #region Validators

    public Boolean CheckFile(string TrainSimPath)
      {
      var CheckPath = TrainSimPath + "\\" + FullName;
      if (CheckPath.EndsWith("//"))
        {
        return false; //Directory
        }
      if (File.Exists(CheckPath))
        {
        IsInstalled = true;
        }
      if (File.GetLastWriteTime(CheckPath) > CreationDate)
        {
        IsOutdated = true;
        }
      return true;
      }
    #endregion


    }
  }
