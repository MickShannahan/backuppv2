using System.Drawing;
using H.NotifyIcon.Core;
namespace backuppv2;

public partial class App : Application
{
	private readonly DirectoryWatcherService _w;
	private readonly AppState _a;
	private TrayIconWithContextMenu _trayIcon;
	public App(DirectoryWatcherService watcher, AppState appState)
	{
		_a = appState;
		_w = watcher;
		InitializeComponent();
	}


	protected override Window CreateWindow(IActivationState? activationState)
	{
		Window window = new Window(new MainPage()) { Title = "backuppv2" };

		// window.Created += (object s, EventArgs e) =>
		// {
		// 	var nativeWindow = (Microsoft.UI.Xaml.Window)window.Handler.PlatformView;
		// 	var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

		// 	var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
		// 			Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd)
		// 	);

		// 	appWindow.Closing += (sender, args) =>
		// 	{
		// 		args.Cancel = true; // stops the app from closing
		// 		window.Hide();
		// 	};
		return window;
	}

	protected override void OnStart()
	{
		// Load the icon from the resources folder
		string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "AppIcon", "ArbysGundam1.ico");
		using Icon icon = new Icon(iconPath);

		TrayIconWithContextMenu trayIcon = new() { Icon = icon.Handle };

		PopupMenu menu = new()
		{
			Items = {
				new PopupMenuItem("🤠", (s,e)=> OnMenuTest(trayIcon))
			}
		};
		trayIcon.ContextMenu = menu;
		trayIcon.Create();
		trayIcon.UpdateIcon(icon.Handle);

		base.OnStart();
	}

	private void OnMenuTest(TrayIconWithContextMenu trayIcon)
	{
		// var toast = ToastNotificationManager();
		Console.WriteLine("Holy JoJo Batman, It's Rorschach");
		trayIcon.ShowNotification(title: "hello", message: "🦧", NotificationIcon.None, respectQuietTime: false);
	}

	private void OnTrayExitClicked()
	{
		// CloseWindow();
	}



	private void OnTrayOpenClick(object sender, EventArgs e)
	{
		Console.WriteLine("Open Clicked");
	}

	private void OnTrayExitClick(object sender, EventArgs e)
	{
		Console.WriteLine("Exit Click");
	}

}
