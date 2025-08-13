using H.NotifyIcon;

namespace backuppv2;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}


	private void OnTrayOpenClick(object sender, EventArgs e)
	{
		var window = Application.Current.Windows.FirstOrDefault();
		window.Activate();
	}

	private void OnTrayExitClick(object sender, EventArgs e)
	{
		Application.Current.Quit();
	}
}
