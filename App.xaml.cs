using System.Drawing;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
namespace backuppv2;

public partial class App : Application
{
	public CustomWindow customWindow { get; }
	private readonly DirectoryWatcherService _w;
	private readonly AppState _a;
	private readonly ITrayService _trayService;
	private readonly INotificationService _notificationService;
	public App(DirectoryWatcherService watcher, AppState appState, ITrayService trayService, INotificationService notificationService, CustomWindow customWindow)
	{
		_a = appState;
		_w = watcher;
		_trayService = trayService;
		_notificationService = notificationService;
		this.customWindow = customWindow;
		InitializeComponent();
	}


	protected override Window CreateWindow(IActivationState? activationState)
	{
		customWindow.Page = new MainPage();
		// Window window = new Window(new MainPage()) { Title = "backuppv2" };
		InitializeTrayIcon();
		// window.X = 0;
		// window.Y = 0;
		// window.Height = 200;
		// window.Width = 600;
		// return window;
		return customWindow;
	}

	protected override void OnStart()
	{

		base.OnStart();
	}

	private void InitializeTrayIcon()
	{
		_trayService.Initialize();
		_trayService.LeftClickHandler += OnTrayOpenClick;
		_trayService.RightClickHandler += OnTrayExitClick;
	}

	private void OnTrayOpenClick()
	{
		Console.WriteLine("Clicked");
		_notificationService.ShowNotification("Sup", "Cool");
		var popupPage = new Popup();
		var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
		var secondWindow = new Window(popupPage)
		{
			Width = 300,
			Height = 600,
			X = displayInfo.Width - 300,
			Y = displayInfo.Height - 600,
		};

		// Remove title bar and border using OverlappedPresenter
		if (secondWindow.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
		{
			var appWindow = AppWindow.GetFromWindowId(
				Microsoft.UI.Win32Interop.GetWindowIdFromWindow(nativeWindow.GetWindowHandle()));
			var presenter = appWindow.Presenter as OverlappedPresenter;
			presenter?.SetBorderAndTitleBar(false, false);
		}

		App.Current?.OpenWindow(secondWindow);
		// var window = Application.Current.Windows.FirstOrDefault();
		// if (window != null)
		// {
		// 	if (window.Visible)
		// 		window.Hide();
		// 	else
		// 		window.Show();
		// }
	}

	private void OnTrayExitClick()
	{
		Current?.Quit();
	}

}
