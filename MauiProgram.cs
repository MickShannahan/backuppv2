using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CommunityToolkit.Maui;
using Microsoft.Maui;
using Microsoft.Maui.LifecycleEvents;
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
		builder.Services.AddSingleton<AppState>();
		builder.Services.AddSingleton<SettingsService>();
		builder.Services.AddScoped<AzureService>();
		builder.Services.AddScoped<BackupsService>();
		builder.Services.AddSingleton<DirectoryWatcherService>();

#if WINDOWS
		builder.Services.AddSingleton<ITrayService, WinUI.TrayService>();
		builder.Services.AddSingleton<INotificationService, WinUI.NotificationService>();
#endif


#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
