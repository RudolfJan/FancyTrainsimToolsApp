using FancyTrainsimTools.Library.Models;
using System.Threading.Tasks;
using Screen = Caliburn.Micro.Screen;

namespace FancyTrainsimToolsDesktop.ViewModels
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
