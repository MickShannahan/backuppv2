namespace backuppv2.Services;

public class BackupsService(AzureService azureService)
{
  private readonly AzureService _azureService = azureService;


  public async Task BackupAllFiles(string folderPath)
  {
    foreach (var file in Directory.EnumerateFiles(folderPath))
    {
      await _azureService.UploadFileAsync(file);
    }
  }
}