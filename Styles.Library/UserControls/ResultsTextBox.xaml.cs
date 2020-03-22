using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Styles.Library.UserControls
  {
  /// <summary>
  /// Interaction logic for ResultsTextBox.xaml
  /// </summary>
  public partial class ResultsTextBox
    {
    #region constructor

    public ResultsTextBox()
      {
      InitializeComponent();
      // http://blog.jerrynixon.com/2013/07/solved-two-way-binding-inside-user.html
      }

    #endregion

    #region events

    private void ClearButtonClicked(Object Sender, RoutedEventArgs E)
      {
      ResultsPart.Text = String.Empty;
      }

    private void ResultsPartTextChanged(Object Sender, TextChangedEventArgs E)
      {
      ResultsPart.ScrollToEnd();
      }

    #endregion

    #region Properties

    /*
		Sets text for the TextBox part
		*/
    public String Text
      {
      get { return (String)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
      }

    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(String), typeof(ResultsTextBox));

    /*
		Allow to set a text for the header
		*/
    public String HeaderText
      {
      get { return (String)GetValue(HeaderTextProperty); }
      set { SetValue(HeaderTextProperty, value); }
      }

    public static readonly DependencyProperty HeaderTextProperty =
      DependencyProperty.Register("HeaderText", typeof(String), typeof(ResultsTextBox));

    /*
		Set the image for the Clear button
		*/
    public ImageSource Source
      {
      get { return (ImageSource)GetValue(SourceProperty); }
      set { SetValue(SourceProperty, value); }
      }

    public static readonly DependencyProperty SourceProperty =
      DependencyProperty.Register("Source", typeof(ImageSource), typeof(ResultsTextBox));

    /*
		Overrides default height for the TextBox part
		*/
    public Int32 TextHeight
      {
      get { return (Int32)GetValue(TextHeightProperty); }
      set { SetValue(TextHeightProperty, value); }
      }

    public static readonly DependencyProperty TextHeightProperty =
      DependencyProperty.Register("TextHeight", typeof(Int32), typeof(ResultsTextBox));

    #endregion
    }
  }
