using FancyTrainsimTools.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Mvvm.Library;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class AboutViewModel: BindableBase
    {
    public AboutModel About { get; set; } = new AboutModel();
    }
  }
