namespace backuppv2.Services;

using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class AzureService
{
  private readonly AzureSettings _settings;
  private readonly BlobContainerClient _containerClient;
  private readonly ILogger logger;

  public AzureService(AzureSettings options, ILogger logger)
  {
    _settings = options;
    this.logger = logger;
    _containerClient = new BlobContainerClient(_settings.ConnectionString, _settings.ContainerName);
  }

  public async Task UploadFileAsync(string filePath, string uploadPath)
  {
    try
    {
      var blobClient = _containerClient.GetBlobClient($"{uploadPath}");

      await using var fileStream = File.OpenRead(filePath);
      var res = await blobClient.UploadAsync(fileStream, overwrite: true);
    }
    catch (Exception e)
    {
      logger.Log(LogLevel.Error, $"[AZ ERROR] {e.Message}");
      logger.Log(LogLevel.Error, e.StackTrace);
    }
  }

  // public async Task UploadIfFileChangedAsync(string filePath, string folderPath)
  // {
  //   if (!_cache.IsFileChanged(filePath))
  //   {
  //     Console.WriteLine($"⏭️ in cache {filePath}");
  //     return;
  //   }

  //   await UploadFileAsync(filePath, folderPath);

  //   _cache.UpdateRecord(filePath);
  //   _cache.Save(BackupFile);

  // }

  public async Task DeleteFileAsync(string fileName)
  {
    var blobClient = _containerClient.GetBlobClient(fileName);
    await blobClient.DeleteIfExistsAsync();

  }
}