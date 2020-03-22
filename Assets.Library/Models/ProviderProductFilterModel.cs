namespace Assets.Library.Models
  {
  public class ProviderProductFilterModel
    {
    public bool InGameFilter { get; set; } = false;
    public bool InArchiveFilter { get; set; } = false;
    public string ProviderFilter { get; set; } = string.Empty;
    public string ProductFilter { get; set; } = string.Empty;
    }
  }
