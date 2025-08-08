public class AppState
{
  public AppSettings AppSettings = new();

  public Dictionary<string, DirectoryBackup> DirectoryBackups = [];

  public string User = "Mick";
  public event Action? OnChange;


  public void NotifyStateChange()
  {
    OnChange?.Invoke();
  }
}