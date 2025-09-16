using CommunityToolkit.Mvvm.ComponentModel;

public class AppState : ObservableObject
{
  public AppStatus Status { get; set; } = AppStatus.IDLE;
  private Dictionary<string, DirectoryBackup> _trackedDirectories = new();
  public Dictionary<string, DirectoryBackup> TrackedDirectories
  {
    get => _trackedDirectories;
    set
    {
      _trackedDirectories = value;
      NotifyStateChange();
    }
  }
  public string User = "Mick";
  public AppSettings AppSettings = new();
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
    Console.WriteLine($"loading Cache {_cache.LinkedDirectories.Count}");
    TrackedDirectories = _cache.LinkedDirectories;
  }


  public async void NotifyStateChange()
  {
    Console.WriteLine("-.>");
    if (_dispatcher != null && _dispatcher.IsDispatchRequired)
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