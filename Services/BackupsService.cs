
using Microsoft.Extensions.Logging;

namespace backuppv2.Services;

public class BackupsService
{
  private readonly AzureService _azureService;
  private readonly AppState _appState;
  private readonly ILogger _logger;
  public BackupsService(AzureService azureService, AppState appState, ILogger logger)
  {
    _azureService = azureService;
    _appState = appState;
    _logger = logger;
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
    List<DirectoryBackup> dirs = [.. _appState.TrackedDirectories.Values];
    foreach (DirectoryBackup directory in dirs)
    {
      await BackupDirectory(directory);
      // _appState.NotifyStateChange();
    }
  }


  public async Task BackupDirectory(DirectoryBackup directory)
  {
    try
    {
      string folderPath = directory.FullPath;
      string folderName = directory.Name;
      if (folderName == null || folderPath == null) return;
      Console.WriteLine($"--> Backing up {folderPath}");


      List<string> newFiles = [.. directory.NewFiles];
      List<string> changedFiles = [.. directory.ChangedFiles];
      List<string> missingFiles = [.. directory.MissingFiles];
      int jobs = newFiles.Count + changedFiles.Count + missingFiles.Count;
      directory.jobCount = jobs;
      directory.currentJobCount = 0;
      _appState.NotifyStateChange();

      // new files 
      foreach (var filePath in newFiles)
      {
        Console.WriteLine($"[+]{filePath}");
        string pathToUpload = filePath.Substring(filePath.IndexOf(folderName));
        await _azureService.UploadFileAsync(filePath, pathToUpload);
        directory.Files.Add(filePath, new FileBackupRecord(filePath));
        directory.currentJobCount++;
        _appState.NotifyStateChange();
      }

      // update files
      foreach (var filePath in changedFiles)
      {
        Console.WriteLine($"[*]{filePath}");
        string pathToUpload = filePath.Substring(filePath.IndexOf(folderName));
        await _azureService.UploadFileAsync(filePath, pathToUpload);
        directory.Files[filePath] = new FileBackupRecord(filePath);
        directory.currentJobCount++;
        _appState.NotifyStateChange();
      }

      // removed files
      foreach (var filePath in missingFiles)
      {
        Console.WriteLine($"[-]{filePath}");
        string pathToDelete = filePath.Substring(filePath.IndexOf(folderName));
        await _azureService.DeleteFileAsync(pathToDelete);
        directory.Files.Remove(filePath);
        directory.currentJobCount++;
        _appState.NotifyStateChange();
      }

      Console.WriteLine("✅-- Backup Complete");
    }
    catch (Exception e)
    {
      _logger.Log(LogLevel.Error, "🫘 " + e.Message);
    }
    finally
    {
      directory.jobCount = 0;
      directory.currentJobCount = 0;
      _appState.NotifyStateChange();
    }
  }

  internal void NotifyStateChange()
  {
    _appState.NotifyStateChange();
  }
}