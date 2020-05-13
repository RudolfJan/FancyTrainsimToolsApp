using FancyTrainsimTools.Library.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Screen = Caliburn.Micro.Screen;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class AboutViewModel: Screen
    {
    public AboutModel About { get; set; } = new AboutModel();

    public async Task CloseButton()
      {
      await TryCloseAsync();
      }
    }
  }
