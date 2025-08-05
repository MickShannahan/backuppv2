namespace backuppv2.Services;

using System.Text.Json;
using backuppv2.Models;
using Microsoft.Extensions.Options;

public class SettingsService
{
  private const string SettingsFile = "settings.json";
  private readonly AzureSettings _azureSettings;

  public SettingsService(AzureSettings azureSettings)
  {
    _azureSettings = azureSettings;
    Console.WriteLine($"Loaded AZ with {_azureSettings.ContainerName}, {_azureSettings.ConnectionString}");
  }

  public AppSettings Load()
  {
    if (!File.Exists(SettingsFile))
    {
      CreateSaveFile();
      return new AppSettings();
    }

    var json = File.ReadAllText(SettingsFile);
    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
  }

  public void Save(AppSettings settings)
  {
    var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(SettingsFile, json);
  }

  private void CreateSaveFile()
  {
    AppSettings defaultSettings = new();
    Save(defaultSettings);
  }
}