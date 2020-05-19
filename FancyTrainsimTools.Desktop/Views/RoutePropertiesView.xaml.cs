namespace FancyTrainsimTools.Desktop.Views
  {
  /// <summary>
  /// Interaction logic for RoutePropertiesView.xaml
  /// </summary>
  public partial class RoutePropertiesView
		{
		public RoutePropertiesView()
			{
			InitializeComponent();
			}
		}
	}

    //public CRoute Route
    //  {
    //  get;
    //  set;
    //  }

    //public RoutePropertiesForm(CRoute MyRoute)
    //  {
    //  InitializeComponent();
    //  Route = MyRoute;
    //  Contract.Assert(MyRoute != null);
    //  Route.UpdateRouteStatus();
    //  SetControlStates();
    //  DataContext = Route;
    //  }

    //private void SetControlStates()
    //  {
    //  OpenPackedFileButton.IsEnabled = Route.IsPacked && APFilesListbox.SelectedItem != null;
    //  EditRoutePropertiesButton.IsEnabled = (!Route.HasNoRouteProperties) && (Route.IsDecompressed || !Route.IsPacked);
    //  }


    //private void OnOkButtonClick(object Sender, RoutedEventArgs E)
    //  {
    //  Close();
    //  }

    //private void OnEditRoutePropertiesButtonClick(object Sender, RoutedEventArgs E)
    //  {
    //  Route.EditRouteProperties();
    //  }

    //private void OnOpenRouteFolderButtonClick(object Sender, RoutedEventArgs E)
    //  {
    //  Route.OpenRouteFolder();
    //  }

    //private void OnOpenPackedFileButton(object Sender, RoutedEventArgs E)
    //  {
    //  var FilePath = (FileInfo)APFilesListbox.SelectedItem;
    //  CApps.OpenZipFile(FilePath.FullName);
    //  }

    //private void OnApListboxSelectionChanged(object Sender, SelectionChangedEventArgs E)
    //  {
    //  SetControlStates();
    //  }

    //#region Filter
    //private void OnSetFilterButtonClicked(System.Object sender, RoutedEventArgs e)
    //  {
    //  //TODO implement filters
    //  }

    //private void FilterInGameChecked(System.Object sender, RoutedEventArgs e)
    //  {

    //  }

    //private void FilterInGameUnChecked(System.Object sender, RoutedEventArgs e)
    //  {

    //  }

    //private void FilterInArchiveChecked(System.Object sender, RoutedEventArgs e)
    //  {

    //  }
    //private void FilterInArchiveUnChecked(System.Object sender, RoutedEventArgs e)
    //  {

    //  }



    //private void ProviderFilterTextChanged(System.Object sender, TextChangedEventArgs e)
    //  {

    //  }

    //private void ProductFilterTextChanged(System.Object sender, TextChangedEventArgs e)
    //  {

    //  }
