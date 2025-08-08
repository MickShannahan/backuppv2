namespace backuppv2.Services;

public class BackupsService
{
  private readonly AzureService _azureService;
  private readonly AppState _appState;
  private const string BackupFile = "files_cache.json";
  private readonly BackupCache _cache;
  public BackupsService(AzureService azureService, AppState appState)
  {
    _azureService = azureService;
    _appState = appState;
    _cache = BackupCache.Load(BackupFile);
    LoadBackups();
  }

  public void LoadBackups()
  {
    Console.WriteLine($"loading Cache {_cache.Directories.Count}");
    _appState.DirectoryBackups = _cache.Directories;
  }

  public void AddDirectoryToBackup(DirectoryBackup directory)
  {
    _appState.DirectoryBackups.Add(directory.Name, directory);
    _appState.NotifyStateChange();
    _cache.Save(BackupFile);
  }

  public void RemoveDirectoryFromBackup(DirectoryBackup directoryBackup)
  {
    _appState.DirectoryBackups.Remove(directoryBackup.Name);
    _appState.NotifyStateChange();
    _cache.Save(BackupFile);
  }

  public async Task BackUpAllDirectories()
  {
    foreach (DirectoryBackup directory in _appState.DirectoryBackups.Values)
    {
      await BackupDirectory(directory.FullPath, directory.Name);
    }
  }


  public async Task BackupDirectory(string folderPath, string folderName)
  {

    var tracked = _cache.GetTrackedFiles(folderName).ToList();
    Console.WriteLine($"--> Backing up {folderPath}");
    // Backup files

    foreach (var filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
    {
      if (!_cache.IsFileChanged(filePath, folderName))
      {
        Console.WriteLine($"[SKP] {filePath}");
      }
      else
      {
        string pathToUpload = filePath.Substring(filePath.IndexOf(folderName));
        Console.WriteLine($"[^] {pathToUpload} - {filePath}");
        await _azureService.UploadFileAsync(filePath, pathToUpload);
        _cache.UpdateRecord(filePath, folderName);
        _cache.Save(BackupFile);
      }
      tracked.Remove(filePath);
    }

    foreach (var staleFile in tracked)
    {
      Console.WriteLine($"STALE {staleFile}");
      string fileName = staleFile.Substring(staleFile.IndexOf(folderName));
      await _azureService.DeleteFileAsync(fileName);
    }
  }
}