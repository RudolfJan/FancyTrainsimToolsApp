using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Library.Models
  {
  // Note this class represents the database View AssetsView, so it flattens provider and product
  public class FlatAssetModel
    {
    public int Id { get; set; }
    public string Provider { get; set; }
    public string Product { get; set; }
    public string BluePrintPath { get; set; }
    public string Pack { get; set; }
    public string AddOnReference { get; set; }
    public bool InGame { get; set; }
    public bool InArchive { get; set; }
    }
  }
