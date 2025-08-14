using System.Drawing;
using System.Runtime.InteropServices;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Windows.System;

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
		var window = new Window(new MainPage()) { Title = "backuppv2" };
		window.Created += (object s, EventArgs e) =>
		{
			var nativeWindow = (Microsoft.UI.Xaml.Window)window.Handler.PlatformView;
			var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

			var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
					Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd)
			);

			appWindow.Closing += (sender, args) =>
			{
				args.Cancel = true; // stops the app from closing
				window.Hide();
			};

			InitTrayIcon();
		};

		return window;
	}



	private void OnTrayOpenClick(object sender, EventArgs e)
	{
		Console.WriteLine("Open Clicked");
	}

	private void OnTrayExitClick(object sender, EventArgs e)
	{
		Console.WriteLine("Exit Click");
	}

	private void InitTrayIcon()
	{
		// Init Icon
		var iconPath = Path.Combine("Resources", "AppIcon", "ArbysGundam1.ico");
		nint iconHandle = LoadIconHandle(iconPath);
		var clickItem = new PopupMenuItem() { Text = "Open" };
		clickItem.Click += OnTrayOpenClick;
		PopupMenu menu = new()
		{
			Items = { clickItem }
		};

		_trayIcon = new TrayIconWithContextMenu
		{
			ContextMenu = menu,
			ToolTip = "Back Up",
			Icon = iconHandle,
			Visibility = IconVisibility.Visible
		};

		_trayIcon.Show();
	}

	private static nint LoadIconHandle(string iconPath)
	{
		return LoadImage(IntPtr.Zero, iconPath, 1, 0, 0, 0x00000010);
	}

	[DllImport("user32.dll", SetLastError = true)]
	private static extern nint LoadImage(IntPtr hInst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

}
