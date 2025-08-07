namespace backuppv2.Services;

public class BackupsService(AzureService azureService)
{
  private readonly AzureService _azureService = azureService;
  private const string BackupFile = "files_cache.json";
  private readonly BackupCache _cache = BackupCache.Load(BackupFile);


  public async Task BackupAllFiles(string folderPath, string folderName)
  {

    var tracked = _cache.GetTrackedFiles(folderName);
    Console.WriteLine($"--> Backing up {folderPath}");
    foreach (string key in tracked)
    {
      Console.WriteLine($"(0)-- {key}");
    }
    // Backup files
    foreach (var filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
    {
      if (!_cache.IsFileChanged(filePath, folderName))
      {
        Console.WriteLine($"[SKP] {filePath}");
        continue;
      }
      string pathToUpload = filePath.Substring(filePath.IndexOf(folderName));

      // await _azureService.UploadFileAsync(pathToUpload);
      // bool wasRemoved = tracked.Remove(filePath);
      _cache.UpdateRecord(filePath, folderName);
      _cache.Save(BackupFile);
      // Console.WriteLine($"[UP]{pathToUpload} - {wasRemoved}");
    }

    foreach (var staleFile in tracked)
    {
      Console.WriteLine($"STALE {staleFile}");
    }
  }
}