using System;

namespace FancyTrainsimTools.Library.Models
	{
	public class FileEntryModel
		{
    public String Name { get; set; } = String.Empty;
    public String Path { get; set; } = String.Empty;
    public Boolean IsSelected { get; set; }

    public override String ToString()
			{
			return Name;
			}
		}
  }
