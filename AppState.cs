using CommunityToolkit.Mvvm.ComponentModel;

public class AppState : ObservableObject
{

  public AppStatus Status { get; set; } = AppStatus.IDLE;
  public AppSettings AppSettings = new();
  public Dictionary<string, DirectoryBackup> TrackedDirectories = [];
  public string User = "Mick";
  public event Action? OnChange;
  private const string BackupFile = "files_cache.json";
  private readonly BackupCache _cache;
  private readonly IDispatcher _dispatcher;

  public AppState(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
    _cache = BackupCache.Load(BackupFile);
    Load();
  }

  private void Load()
  {
    Console.WriteLine($"loading Cache {_cache.Directories.Count}");
    TrackedDirectories = _cache.Directories;
  }


  public async void NotifyStateChange()
  {
    if (_dispatcher.IsDispatchRequired)
    {
      _dispatcher.Dispatch(() => OnChange?.Invoke());
    }
    else
    {
      OnChange?.Invoke();
    }
    _cache?.Save(BackupFile);
  }
}

public enum AppStatus
{
  IDLE,
  BUSY,
  BACKINGUP,
  DOWNLOADING,
}