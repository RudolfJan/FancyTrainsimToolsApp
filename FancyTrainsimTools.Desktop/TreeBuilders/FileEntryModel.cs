namespace FancyTrainsimToolsDesktop
	{
	public class FileEntryModel
		{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsSelected { get; set; }

		public FileEntryModel()
			{
			
			}
    public override string ToString()
			{
			return Name;
			}
		}
  }
