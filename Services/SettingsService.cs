namespace backuppv2.Services;

using System.Text.Json;
using backuppv2.Models;
using Microsoft.Extensions.Options;

public class SettingsService
{
  private const string SettingsFile = "settings.json";
  private readonly AzureSettings _azureSettings;
  private readonly AppState _appState;

  public SettingsService(AzureSettings azureSettings, AppState appState)
  {
    _azureSettings = azureSettings;
    _appState = appState;
  }

  public void Load()
  {
    if (!File.Exists(SettingsFile))
    {
      CreateSaveFile();
      return;
    }

    var json = File.ReadAllText(SettingsFile);
    AppSettings settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    _appState.AppSettings = settings;
  }

  public void Save()
  {
    AppSettings settings = _appState.AppSettings;
    var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(SettingsFile, json);
  }

  private void CreateSaveFile()
  {
    _appState.AppSettings = new AppSettings();
    Save();
  }
}