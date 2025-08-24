using System.Drawing;
using CommunityToolkit.Mvvm.Input;
namespace backuppv2;

public partial class App : Application
{
	private readonly DirectoryWatcherService _w;
	private readonly AppState _a;
	private readonly ITrayService _trayService;
	private readonly INotificationService _notificationService;
	public App(DirectoryWatcherService watcher, AppState appState, ITrayService trayService, INotificationService notificationService)
	{
		_a = appState;
		_w = watcher;
		InitializeComponent();
		_trayService = trayService;
		_notificationService = notificationService;
	}


	protected override Window CreateWindow(IActivationState? activationState)
	{
		Window window = new Window(new MainPage()) { Title = "backuppv2" };
		InitializeTrayIcon();
		// window.X = 0;
		// window.Y = 0;
		// window.Height = 200;
		// window.Width = 600;
		return window;
	}

	protected override void OnStart()
	{

		base.OnStart();
	}

	private void InitializeTrayIcon()
	{
		_trayService.Initialize();
		_trayService.LeftClickHandler += OnTrayOpenClick;
	}

	private void OnTrayOpenClick()
	{
		Console.WriteLine("Clicked");
		_notificationService.ShowNotification("Sup", "Cool");
		// var window = Application.Current.Windows.FirstOrDefault();
		// if (window != null)
		// {
		// 	if (window.Visible)
		// 		window.Hide();
		// 	else
		// 		window.Show();
		// }
	}

	// private void OnTrayExitClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	// {
	// 	_trayIcon?.Dispose();
	// 	Current.Quit();
	// }

}
