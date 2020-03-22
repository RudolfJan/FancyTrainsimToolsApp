using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Styles.Library.UserControls
  {
  /// <summary>
  /// Interaction logic for LabelTextBox.xaml
  /// </summary>
  /// 
  // https://stackoverflow.com/questions/3067617/raising-an-event-on-parent-window-from-a-user-control-in-net-c-sharp

  [System.Windows.Markup.ContentProperty("TextBoxText")]
  public partial class LabelTextBox
    {
    public LabelTextBox()
      {
      InitializeComponent();
      }

    public String LabelText
      {
      get { return (String)GetValue(LabelTextProperty); }
      set { SetValue(LabelTextProperty, value); }
      }

    public static readonly DependencyProperty LabelTextProperty =
      DependencyProperty.Register("LabelText", typeof(String), typeof(LabelTextBox));

    /*
    Sets width of the label part
    */
    public Double LabelWidth
      {
      get { return (Double)GetValue(LabelWidthProperty); }
      set { SetValue(LabelWidthProperty, value); }
      }

    public static readonly DependencyProperty LabelWidthProperty =
      DependencyProperty.Register("LabelWidth", typeof(Double), typeof(LabelTextBox), new PropertyMetadata(50.0));



    public static readonly RoutedEvent TextChangedEvent =
           EventManager.RegisterRoutedEvent("TextChangedEvent", RoutingStrategy.Bubble,
           typeof(RoutedEventHandler), typeof(LabelTextBox));

    public event RoutedEventHandler TextChanged
      {
      add => AddHandler(TextChangedEvent, value);
      remove => RemoveHandler(TextChangedEvent, value);
      }

    /*
    If set, makes the TextBox part readOnly
    */
    public Boolean IsReadOnly
      {
      get { return (Boolean)GetValue(IsReadOnlyProperty); }
      set { SetValue(IsReadOnlyProperty, value); }
      }

    public static readonly DependencyProperty IsReadOnlyProperty =
      DependencyProperty.Register("IsReadOnly", typeof(Boolean), typeof(LabelTextBox), new PropertyMetadata(false));

    public Brush TextBoxBackGround
      {
      get { return (Brush)GetValue(TextBoxBackGroundProperty); }
      set { SetValue(TextBoxBackGroundProperty, value); }
      }

    public static readonly DependencyProperty TextBoxBackGroundProperty =
      DependencyProperty.Register("TextBoxBackGround", typeof(Brush), typeof(LabelTextBox));

    public String TextBoxText
      {
      get { return (String)GetValue(TextBoxTextProperty); }
      set { SetValue(TextBoxTextProperty, value); }
      }

    public static readonly DependencyProperty TextBoxTextProperty =
      DependencyProperty.Register("TextBoxText", typeof(String), typeof(LabelTextBox));

    private void OnTextChanged(Object Sender, TextChangedEventArgs E)
      {
      TextBoxText = LabeledTextBox.Text;
      RaiseEvent(new RoutedEventArgs(TextChangedEvent));
      }
    }
  }
