using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CommunityToolkit.Maui;
using Microsoft.Maui;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Microsoft.AspNetCore.Components;
using Quartz;
using Quartz.Spi;
using System.Threading.Tasks;
using System.Text;
#if WINDOWS
using Microsoft.UI.Xaml;
using WinRT.Interop;
using PInvoke;
#endif
namespace backuppv2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

		// Keep app from running twice
		using var mutex = new Mutex(true, "MyBackupAppMutex", out bool isNewInstance);
		if (!isNewInstance)
		{
			Environment.Exit(0); // Exit the application if another instance is running
		}

		var builder = MauiApp.CreateBuilder();

		var config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory() ?? FileSystem.AppDataDirectory)
			.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
			.Build();


		builder.Configuration.AddConfiguration(config);

		builder.ConfigureLifecycleEvents(events =>
					 {
						 events.AddWindows(windowsLifecycleBuilder =>
							{
								windowsLifecycleBuilder.OnWindowCreated(window =>
								 {
									 window.SystemBackdrop = new MicaBackdrop();
								 });
							});
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
		builder.Services.AddQuartz();
		builder.Services.AddTransient<CustomWindow>();
		builder.Services.AddSingleton<AppState>();
		builder.Services.AddSingleton<SettingsService>();
		builder.Services.AddScoped<AzureService>();
		builder.Services.AddScoped<BackupsService>();
		builder.Services.AddSingleton<DirectoryWatcherService>();
		builder.Services.AddSingleton<ILogger, Logger>();
		// trying to get quartz with DI to work
		builder.Services.AddSingleton<SchedulerService>();
		var serviceProvider = builder.Services.BuildServiceProvider();


#if WINDOWS
		builder.Services.AddSingleton<ITrayService, WinUI.TrayService>();
		builder.Services.AddSingleton<INotificationService, WinUI.NotificationService>();


		// var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
		// BackupJobSchedule.SetScheduler(schedulerFactory);


#endif




#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
		Console.OutputEncoding = Encoding.UTF8;

		// 🔴 Global exception hooks here
		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			var exception = e.ExceptionObject as Exception;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"[UnhandledException] {exception?.Message}");
			Console.WriteLine($"[UnhandledException] {exception?.StackTrace}");
			Console.ResetColor();
		};

		TaskScheduler.UnobservedTaskException += (s, e) =>
		{
			System.Diagnostics.Debug.WriteLine($"[UnobservedTaskException] {e.Exception}");
			e.SetObserved();
		};
#endif

		InitializeSchedulerServiceAsync(serviceProvider);

		return builder.Build();
	}




	private static async void InitializeSchedulerServiceAsync(IServiceProvider services)
	{
		try
		{
			var schedulerService = services.GetRequiredService<SchedulerService>();
			await schedulerService.SetupAsync();
			// await schedulerService.StartSchedule();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error initializing SchedulerService: {ex.Message}");
		}
	}
}
