using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows;

// See WPF4.5 Unleashed chapter 20

// Added a number of dependency properties so I have better control over the open file dialog
namespace Styles.Library.UserControls
  {
  public enum FileDialogTypeEnum { OpenFile, SaveFile, BrowseDir };

  [System.Windows.Markup.ContentProperty("FileName")]


  public partial class FileInputBox : UserControl
    { 
  
  #region Constructor
    public FileInputBox()
      {
      InitializeComponent();
      FileTextBox.TextChanged += new TextChangedEventHandler(OnTextChanged);
      }

    #endregion

    #region Events
    private void GetFileButtonClicked(Object Sender, RoutedEventArgs E)
      {
      switch (FileDialogType)
        {
        case FileDialogTypeEnum.OpenFile:
            {
#pragma warning disable IDE0017 // Simplify object initialization
            var Form = new OpenFileDialog();
#pragma warning restore IDE0017 // Simplify object initialization
            //set property values
            Form.Title = Title;
            Form.CheckPathExists = CheckPathExists;
            Form.CheckFileExists = CheckFileExists;
            Form.ReadOnlyChecked = ReadOnlyChecked;
            Form.InitialDirectory = InitialDirectory;
            Form.RestoreDirectory = RestoreDirectory;
            Form.Filter = Filter;


            if (Form.ShowDialog() == true) // Result could be true, false, or null
              this.FileName = Form.FileName;
            break;
            }

        case FileDialogTypeEnum.SaveFile:
            {
#pragma warning disable IDE0017 // Simplify object initialization
            var Form = new SaveFileDialog();
#pragma warning restore IDE0017 // Simplify object initialization
            Form.Title = Title;
            Form.CheckPathExists = CheckPathExists;
            Form.CheckFileExists = CheckFileExists;
            Form.InitialDirectory = InitialDirectory;
            Form.RestoreDirectory = RestoreDirectory;
            Form.Filter = Filter;
            if (Form.ShowDialog() == true) // Result could be true, false, or null
              this.FileName = Form.FileName;
            break;
            }
        case FileDialogTypeEnum.BrowseDir:
            {
#pragma warning disable IDE0017 // Simplify object initialization
            using var Form = new System.Windows.Forms.FolderBrowserDialog
              {
              Description = Title,
              RootFolder = Environment.SpecialFolder.MyComputer,
              SelectedPath = FileName
              };
            // Use of this class is exceptional, so use full lib path here
#pragma warning restore IDE0017 // Simplify object initialization
            if (Form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
              {
              FileName = Form.SelectedPath;
              }

            break;
            }
        }

      }

    public static readonly RoutedEvent FileNameChangedEvent =
           EventManager.RegisterRoutedEvent("FileNameChanged",
           RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileInputBox));
    private void OnTextChanged(Object Sender, TextChangedEventArgs E)
      {
      E.Handled = true;
      RoutedEventArgs Args = new RoutedEventArgs(FileNameChangedEvent);
      RaiseEvent(Args);
      }

    public event RoutedEventHandler FileNameChanged
      {
      add => AddHandler(FileNameChangedEvent, value);
      remove => RemoveHandler(FileNameChangedEvent, value);
      }

    #endregion

    #region Properties
    public String FileName
      {
      get => (String)GetValue(FileNameProperty);
      set => SetValue(FileNameProperty, value);
      }

    public static readonly DependencyProperty FileNameProperty =
       DependencyProperty.Register("FileName", typeof(String), typeof(FileInputBox));

    public FileDialogTypeEnum FileDialogType
      {
      get
        {
        return (FileDialogTypeEnum)GetValue(FileDialogTypeProperty);
        }

      set
        {
        SetValue(FileDialogTypeProperty, value);
        }
      }

    public static readonly DependencyProperty FileDialogTypeProperty =
    DependencyProperty.Register("FileDialogType", typeof(FileDialogTypeEnum), typeof(FileInputBox), new PropertyMetadata(FileDialogTypeEnum.OpenFile));

    public String Title
      {
      get => (String)GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
      }

    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(String), typeof(FileInputBox));

    public String InitialDirectory
      {
      get => (String)GetValue(InitialDirectoryProperty);
      set => SetValue(InitialDirectoryProperty, value);
      }

    public static readonly DependencyProperty InitialDirectoryProperty =
      DependencyProperty.Register("InitialDirectory", typeof(String), typeof(FileInputBox));

    public String Filter
      {
      get => (String)GetValue(FilterProperty);
      set => SetValue(FilterProperty, value);
      }

    public static readonly DependencyProperty FilterProperty =
      DependencyProperty.Register("Filter", typeof(String), typeof(FileInputBox));

    public Boolean RestoreDirectory
      {
      get => (Boolean)GetValue(RestoreDirectoryProperty);
      set => SetValue(RestoreDirectoryProperty, value);
      }

    public static readonly DependencyProperty RestoreDirectoryProperty =
      DependencyProperty.Register("RestoreDirectory", typeof(Boolean), typeof(FileInputBox), new PropertyMetadata(true));

    public Boolean CheckPathExists
      {
      get => (Boolean)GetValue(CheckPathExistsProperty);
      set => SetValue(CheckPathExistsProperty, value);
      }

    public static readonly DependencyProperty CheckPathExistsProperty =
      DependencyProperty.Register("CheckPathExists", typeof(Boolean), typeof(FileInputBox), new PropertyMetadata(false));

    public Boolean ReadOnlyChecked
      {
      get => (Boolean)GetValue(ReadOnlyCheckedProperty);
      set => SetValue(ReadOnlyCheckedProperty, value);
      }

    public static readonly DependencyProperty ReadOnlyCheckedProperty =
      DependencyProperty.Register("ReadOnlyChecked", typeof(Boolean), typeof(FileInputBox), new PropertyMetadata(false));
    public Boolean CheckFileExists
      {
      get => (Boolean)GetValue(CheckFileExistsProperty);
      set => SetValue(CheckFileExistsProperty, value);
      }

    public static readonly DependencyProperty CheckFileExistsProperty =
      DependencyProperty.Register("CheckFileExists", typeof(Boolean), typeof(FileInputBox), new PropertyMetadata(false));

    public Boolean MultiSelect
      {
      get => (Boolean)GetValue(MultiSelectProperty);
      set => SetValue(MultiSelectProperty, value);
      }

    public static readonly DependencyProperty MultiSelectProperty =
      DependencyProperty.Register("MultiSelect", typeof(Boolean), typeof(FileInputBox), new PropertyMetadata(false));

    public String LabelText
      {
      get => (String)GetValue(LabelTextProperty);
      set => SetValue(LabelTextProperty, value);
      }

    public static readonly DependencyProperty LabelTextProperty =
      DependencyProperty.Register("LabelText", typeof(String), typeof(FileInputBox));

    public Double LabelWidth
      {
      get { return (Double)GetValue(LabelWidthProperty); }
      set { SetValue(LabelWidthProperty, value); }
      }

    public static readonly DependencyProperty LabelWidthProperty =
      DependencyProperty.Register("LabelWidth", typeof(Double), typeof(FileInputBox));



    #endregion


    }
  }