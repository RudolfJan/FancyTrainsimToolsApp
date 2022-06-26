using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// https://stackoverflow.com/questions/744541/how-to-list-available-video-modes-using-c

namespace FancyTrainsimToolsDesktop.Helpers
	{
	public class VideoMode
    {
    public int Width { get; set; }
    public int Height { get; set; }
    public int Freq { get; set; }
    public int Color { get; set; }

    public string VideoModeText
      {
      get
        {
        return $"{Width:d4}x{Height:d4} {Freq}Hz";
        }
      }

    public override string ToString()
      {
      return $"{Width:d4}x{Height:d4} {Freq}Hz";
      }

    public static string GetModeText(int width, int height)
      {
      return $"{width:d4}x{height:d4}";
      }
    }

 

  public class VideoModes
    {
    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(
                        string DeviceName, int ModeNum, ref Devmode DevMode);

    private const int EnumCurrentSettings = -1;

    private const int EnumRegistrySettings = -2;

    [StructLayout(LayoutKind.Sequential)]
    public struct Devmode
      {
      private const int Cchdevicename = 0x20;
      private const int Cchformname = 0x20;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
      public string dmDeviceName;

      public short dmSpecVersion;
      public short dmDriverVersion;
      public short dmSize;
      public short dmDriverExtra;
      public int dmFields;
      public int dmPositionX;
      public int dmPositionY;
      public ScreenOrientation dmDisplayOrientation;
      public int dmDisplayFixedOutput;
      public short dmColor;
      public short dmDuplex;
      public short dmYResolution;
      public short dmTTOption;
      public short dmCollate;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
      public string dmFormName;

      public short dmLogPixels;
      public int dmBitsPerPel;
      public int dmPelsWidth;
      public int dmPelsHeight;
      public int dmDisplayFlags;
      public int dmDisplayFrequency;
      public int dmICMMethod;
      public int dmICMIntent;
      public int dmMediaType;
      public int dmDitherType;
      public int dmReserved1;
      public int dmReserved2;
      public int dmPanningWidth;
      public int dmPanningHeight;
      }

    public static List<VideoMode> ListVideoModes()
      {
      var VDevMode = new Devmode();
      int I = 0;
      var VideoModesList = new List<VideoMode>();

      while (EnumDisplaySettings(null, I, ref VDevMode))
        {
        var VideoModeVar = new VideoMode
          {
          Width = VDevMode.dmPelsWidth,
          Height = VDevMode.dmPelsHeight,
          Color = 1 << VDevMode.dmBitsPerPel,
          Freq = VDevMode.dmDisplayFrequency
          };
        VideoModesList.Add(VideoModeVar);
        I++;
        }
      return VideoModesList;
      }
    }
  }
