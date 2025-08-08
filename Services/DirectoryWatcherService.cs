namespace backuppv2.Services;


public class DirectoryWatcherService
{
  private readonly AppState _appState;

  private readonly Dictionary<string, FileSystemWatcher> _watchers = [];

  public DirectoryWatcherService(AppState appState)
  {
    Console.WriteLine("(w)-1");
    _appState = appState;
    StartWatchingAll();
  }

  private void StartWatchingAll()
  {
    Console.WriteLine($"(w)-2 {_appState.TrackedDirectories.Count}");
    foreach (var dir in _appState.TrackedDirectories.Values)
    {
      StartWatchingDirectory(dir.FullPath);
    }
  }

  private void StartWatchingDirectory(string directoryPath)
  {
    if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath) || _watchers.ContainsKey(directoryPath)) return;

    var watcher = new FileSystemWatcher(directoryPath)
    {
      IncludeSubdirectories = true,
      EnableRaisingEvents = true,
      NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
    };

    watcher.Created += OnDirectoryChanged;
    watcher.Deleted += OnDirectoryChanged;
    watcher.Changed += OnDirectoryChanged;
    watcher.Renamed += OnDirectoryChanged;

    _watchers[directoryPath] = watcher;
    Console.WriteLine($"[w] {directoryPath}");
  }

  private void OnDirectoryChanged(object sender, FileSystemEventArgs eventArgs)
  {
    Console.WriteLine($"(o) {eventArgs.ChangeType} > {eventArgs.FullPath}");

    string directoryPath = Path.GetDirectoryName(eventArgs.FullPath);
    if (directoryPath == null) return;

    var trackedDirs = _appState.TrackedDirectories.Values.ToList();
    var trackedDirectory = trackedDirs.Find(d => d.Subdirectories.Contains(directoryPath));

    if (trackedDirectory == null) return;
    if (!_appState.TrackedDirectories.TryGetValue(trackedDirectory.Name, out var backup)) return;

    switch (eventArgs.ChangeType)
    {
      case WatcherChangeTypes.Created:
      case WatcherChangeTypes.Changed:
        backup.Files[eventArgs.FullPath] = new FileBackupRecord(eventArgs.FullPath);
        break;

      case WatcherChangeTypes.Deleted:
        backup.Files.Remove(eventArgs.FullPath);
        break;

      case WatcherChangeTypes.Renamed:
        if (eventArgs is RenamedEventArgs renamedEventArgs)
        {
          backup.Files.Remove(eventArgs.FullPath);
          backup.Files[renamedEventArgs.FullPath] = new FileBackupRecord(renamedEventArgs.FullPath);
        }
        break;
    }
    Console.WriteLine($"[bu] {backup.Files.Count} {backup.FileCount}");
    _appState.NotifyStateChange();
  }
}