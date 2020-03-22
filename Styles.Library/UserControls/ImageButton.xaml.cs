using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Styles.Library.UserControls
  {
  /// <summary>
  /// Interaction logic for ImageButton.xaml
  /// </summary>

  // https://blogs.msdn.microsoft.com/knom/2007/10/31/wpf-control-development-3-ways-to-build-an-imagebutton/


  public partial class ImageButton
    {
    public ImageButton()
      {
      InitializeComponent();
      }

    public ImageSource Image
      {
      get { return (ImageSource)GetValue(ImageProperty); }
      set { SetValue(ImageProperty, value); }
      }

    // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata(null));

    public Double ImageWidth
      {
      get { return (Double)GetValue(ImageWidthProperty); }
      set { SetValue(ImageWidthProperty, value); }
      }

    // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageWidthProperty =
        DependencyProperty.Register("ImageWidth", typeof(Double), typeof(ImageButton), new UIPropertyMetadata(16d));

    public Double ImageHeight
      {
      get { return (Double)GetValue(ImageHeightProperty); }
      set { SetValue(ImageHeightProperty, value); }
      }

    // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageHeightProperty =
        DependencyProperty.Register("ImageHeight", typeof(Double), typeof(ImageButton), new UIPropertyMetadata(16d));

    public String Text
      {
      get { return (String)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
      }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(String), typeof(ImageButton), new UIPropertyMetadata(""));
    }
  }
