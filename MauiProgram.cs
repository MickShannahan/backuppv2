using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CommunityToolkit.Maui;
using Microsoft.Maui;
using Microsoft.Maui.LifecycleEvents;
using WinRT.Interop;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Microsoft.AspNetCore.Components;
using Quartz;
using Quartz.Spi;
namespace backuppv2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

		// Keep app from running twice
		using var mutex = new Mutex(true, "MyBackupAppMutex", out bool isNewInstance);
		if (!isNewInstance)
		{
			return null;
		}

		var builder = MauiApp.CreateBuilder();

		var config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory() ?? FileSystem.AppDataDirectory)
			.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
			.Build();


		builder.Configuration.AddConfiguration(config);

		builder.ConfigureLifecycleEvents(events =>
					 {
#if WINDOWS
						 events.AddWindows(windowsLifecycleBuilder =>
							{
								windowsLifecycleBuilder.OnWindowCreated(window =>
								 {
									 window.SystemBackdrop = new MicaBackdrop();
								 });
							});
#endif
					 });


		builder
			 .UseMauiApp<App>()
			 .UseMauiCommunityToolkit()
			 .ConfigureFonts(fonts =>
			 {
				 fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			 });
		builder.Services.AddLogging(logging =>
		 {
			 logging.AddDebug();
		 });

		var azureSettings = new AzureSettings();
		config.GetSection("Azure").Bind(azureSettings);
		builder.Services.AddSingleton(azureSettings);

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddTransient<CustomWindow>();
		builder.Services.AddSingleton<AppState>();
		builder.Services.AddSingleton<SettingsService>();
		builder.Services.AddScoped<AzureService>();
		builder.Services.AddScoped<BackupsService>();
		builder.Services.AddSingleton<DirectoryWatcherService>();


#if WINDOWS
		builder.Services.AddSingleton<ITrayService, WinUI.TrayService>();
		builder.Services.AddSingleton<INotificationService, WinUI.NotificationService>();

		builder.Services.AddSingleton<SchedulerService>();
		builder.Services.AddQuartz();
		var serviceProvider = builder.Services.BuildServiceProvider();
		var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
		BackupJobSchedule.SetScheduler(schedulerFactory);
#endif




#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

		// 🔴 Global exception hooks here
		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			System.Diagnostics.Debug.WriteLine($"[UnhandledException] {e.ExceptionObject}");
		};

		TaskScheduler.UnobservedTaskException += (s, e) =>
		{
			System.Diagnostics.Debug.WriteLine($"[UnobservedTaskException] {e.Exception}");
			e.SetObserved();
		};
#endif



		return builder.Build();
	}
}
