using WinRT;

namespace backuppv2.Services;

public class BackupsService
{
  private readonly AzureService _azureService;
  private readonly AppState _appState;
  public BackupsService(AzureService azureService, AppState appState)
  {
    _azureService = azureService;
    _appState = appState;
  }



  public void AddDirectoryToBackup(DirectoryBackup directory)
  {
    _appState.TrackedDirectories.Add(directory.Name, directory);
    _appState.NotifyStateChange();
  }

  public async Task RemoveDirectoryFromBackup(DirectoryBackup directoryBackup)
  {
    foreach (var filePath in directoryBackup.Files.Keys)
    {
      string fileName = filePath.Substring(filePath.IndexOf(directoryBackup.Name));
      await _azureService.DeleteFileAsync(fileName);
    }
    _appState.TrackedDirectories.Remove(directoryBackup.Name);
    _appState.NotifyStateChange();
  }

  public async Task BackUpAllDirectories()
  {
    foreach (DirectoryBackup directory in _appState.TrackedDirectories.Values)
    {
      await BackupDirectory(directory);
      _appState.NotifyStateChange();
    }
  }


  public async Task BackupDirectory(DirectoryBackup directory)
  {
    string folderPath = directory.FullPath;
    string folderName = directory.Name;
    if (folderName == null || folderPath == null) return;
    Console.WriteLine($"--> Backing up {folderPath}");

    // new files 
    foreach (var filePath in directory.NewFiles)
    {
      Console.WriteLine($"[+]{filePath}");
      string pathToUpload = filePath.Substring(filePath.IndexOf(folderName));
      await _azureService.UploadFileAsync(filePath, pathToUpload);
      directory.Files.Add(filePath, new FileBackupRecord(filePath));
    }

    // update files
    foreach (var filePath in directory.ChangedFiles)
    {
      Console.WriteLine($"[*]{filePath}");
      string pathToUpload = filePath.Substring(filePath.IndexOf(folderName));
      await _azureService.UploadFileAsync(filePath, pathToUpload);
      directory.Files[filePath] = new FileBackupRecord(filePath);
    }

    // removed files
    foreach (var filePath in directory.MissingFiles)
    {
      Console.WriteLine($"[-]{filePath}");
      string pathToDelete = filePath.Substring(filePath.IndexOf(folderName));
      await _azureService.DeleteFileAsync(pathToDelete);
      directory.Files.Remove(filePath);
    }
  }
}