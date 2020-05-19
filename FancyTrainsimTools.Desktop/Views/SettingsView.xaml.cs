namespace FancyTrainsimTools.Desktop.Views
	{
	/// <summary>
	/// Interaction logic for SettingsView.xaml
	/// </summary>
	public partial class SettingsView
		{
		public SettingsView()
			{
			InitializeComponent();
			}

		private void Hyperlink_RequestNavigate(object Sender, System.Windows.Navigation.RequestNavigateEventArgs E)
			{
			System.Diagnostics.Process.Start(E.Uri.AbsoluteUri);
			}

		}
	}
