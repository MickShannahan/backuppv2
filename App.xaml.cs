namespace backuppv2;

public partial class App : Application
{
	public CustomWindow CustomWindow { get; }
	private readonly ITrayService _trayService;
	private readonly INotificationService _notificationService;

	private readonly SchedulerService _schedulerService;
	public bool IsQuitRequested { get; set; } = false;

	public App(ITrayService trayService, INotificationService notificationService, CustomWindow customWindow, SchedulerService schedulerService)
	{
		CustomWindow = customWindow;
		_trayService = trayService;
		_notificationService = notificationService;
		_schedulerService = schedulerService;
		_ = _schedulerService.StartSchedule(); // Fire-and-forget the async call
		InitializeComponent();
	}


	protected override Window CreateWindow(IActivationState? activationState)
	{
		CustomWindow.Page = new MainPage();

		// Intercept close event
#if WINDOWS
		CustomWindow.HandlerChanged += (sender, args) =>
		{
			if (CustomWindow.Handler?.PlatformView is Microsoft.UI.Xaml.Window window)
			{
				window.Closed += (s, e) =>
				{
					Console.WriteLine($"❌ {IsQuitRequested}");
					if (!IsQuitRequested)
					{
						e.Handled = true; // Prevent the window from closing
						var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
						PInvoke.User32.ShowWindow(hwnd, PInvoke.User32.WindowShowStyle.SW_HIDE); // Hide the window
						WindowExtensions.MinimizeToTray();
					}
				};
			}
		};
#endif

		InitializeTrayIcon();
		return CustomWindow;
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
		// _notificationService.ShowNotification("Sup", "Cool");

		if (Current?.Windows != null)
		{
			var mainWindow = Current.Windows.FirstOrDefault();
			if (mainWindow != null)
			{
				// WindowExtensions.BringToFront();
			}
			else
			{
				CustomWindow.Page = new MainPage();
				Current.OpenWindow(CustomWindow);
			}
		}
	}

	private void OnTrayExitClick()
	{
		QuitApplication();
	}

	public void QuitApplication()
	{
		IsQuitRequested = true; // Set the flag to allow quitting
		Current?.Quit();
	}

}
