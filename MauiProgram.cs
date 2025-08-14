using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using H.NotifyIcon;
using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;
using WinRT.Interop;
using Microsoft.UI;

namespace backuppv2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		var config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory() ?? FileSystem.AppDataDirectory)
			.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
			.Build();


		builder.Configuration.AddConfiguration(config);

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseNotifyIcon()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
		builder.Services.AddLogging(logging =>
		{
			logging.AddDebug();
		});


		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<AppState>();
		builder.Services.AddSingleton<SettingsService>();
		builder.Services.AddScoped<AzureService>();
		builder.Services.AddScoped<BackupsService>();
		builder.Services.AddSingleton<DirectoryWatcherService>();

		var azureSettings = new AzureSettings();
		config.GetSection("Azure").Bind(azureSettings);
		builder.Services.AddSingleton(azureSettings);

		// Intercept windows close
		// #if WINDOWS
		// 		builder.ConfigureLifecycleEvents(events =>
		// 		{
		// 			events.AddWindows(windows =>
		// 	{
		// 		windows.OnWindowCreated((window) =>
		// 								{
		// 									// Hook close event
		// 									var hwnd = WindowNative.GetWindowHandle(window);
		// 									var winId = Win32Interop.GetWindowIdFromWindow(hwnd);
		// 									var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(winId);

		// 									appWindow.Closing += (sender, args) =>
		// 													{
		// 														// Cancel close
		// 														args.Cancel = true;

		// 														// Hide instead
		// 														MainThread.BeginInvokeOnMainThread(() =>
		// 																					{
		// 																						// Application.Current.Windows[0].Hide();
		// 																						Application.Current.Quit();
		// 																					});
		// 													};
		// 								});
		// 	});
		// 		});
		// #endif


#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
