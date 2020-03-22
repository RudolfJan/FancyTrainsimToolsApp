using System;
using System.Windows;

namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for CLogViewer.xaml
  /// </summary>
  public partial class LoggingView
    {

    private static readonly Object Lock1 = new Object();

    public LoggingView()
      {
      InitializeComponent();
      //Filter = new CLogFilter(DebugCheckBox.IsChecked != null && (Boolean) DebugCheckBox.IsChecked,
      //  ErrorCheckBox.IsChecked != null && (Boolean) ErrorCheckBox.IsChecked,
      //  MessageCheckBox.IsChecked != null && (Boolean) MessageCheckBox.IsChecked,
      //  EventCheckBox.IsChecked != null && (Boolean) EventCheckBox.IsChecked);
      //BindingOperations.EnableCollectionSynchronization(Log.LogManager, Lock1);
      //if (CollectionViewSource.GetDefaultView(LogView.ItemsSource) is ListCollectionView View)
      //  {
      //  View.Filter = Filter.EventTypeFilter;
      //  View.Refresh();
      //  }
      }

    private void OnLogViewDataContextChanged(object Sender, DependencyPropertyChangedEventArgs E)
      {
      //LogView.Items.MoveCurrentToLast();
      //LogView.ScrollIntoView(LogView.Items.CurrentItem); // scroll to last item
      }

    private void OnSaveLogButtonClicked(object Sender, RoutedEventArgs E)
      {
      //var FileDialog = new SaveFileDialog
      //  {
        //InitialDirectory = CLuaCreatorOptions.LuaCreatorDirectory + "Temp",
        //  RestoreDirectory = true,
        //  Title = "Save log file",
        //  Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*"
        //  };

        //if (FileDialog.ShowDialog() == true)
        //  {
        //  var AllText = String.Empty;
        //  foreach (var X in Log.LogManager)
        //    {
        //    AllText += X + "\r\n";
        //    }

        //  File.WriteAllText(FileDialog.FileName, AllText);
        //  }
        }

      private void OnFilterChanged(object Sender, RoutedEventArgs E)
        {
        //if (DebugCheckBox != null && ErrorCheckBox != null && MessageCheckBox != null &&
        //    EventCheckBox != null)
        //  {
        //  if (Filter == null)
        //    {
        //    Filter = new CLogFilter(
        //      DebugCheckBox.IsChecked != null && (Boolean)DebugCheckBox.IsChecked,
        //      ErrorCheckBox.IsChecked != null && (Boolean)ErrorCheckBox.IsChecked,
        //      MessageCheckBox.IsChecked != null && (Boolean)MessageCheckBox.IsChecked,
        //      EventCheckBox.IsChecked != null && (Boolean)EventCheckBox.IsChecked);
        //    }
        //  else
        //    {
        //    Filter.UpdateFilterSettings(
        //      DebugCheckBox.IsChecked != null && (Boolean)DebugCheckBox.IsChecked,
        //      ErrorCheckBox.IsChecked != null && (Boolean)ErrorCheckBox.IsChecked,
        //      MessageCheckBox.IsChecked != null && (Boolean)MessageCheckBox.IsChecked,
        //      EventCheckBox.IsChecked != null && (Boolean)EventCheckBox.IsChecked);
        //    }

        //  if (CollectionViewSource.GetDefaultView(LogView.ItemsSource) is ListCollectionView View)
        //    {
        //    View.Filter = Filter.EventTypeFilter;
        //    View.Refresh();
        //    }
        //  }
        }

    private void OKButton_OnClick(Object sender, RoutedEventArgs e)
      {
      Close();
      }
    }
    }
  

