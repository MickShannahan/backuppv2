namespace backuppv2.Services;

using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

public class AzureService
{
  private readonly AzureSettings _settings;
  private readonly BlobContainerClient _containerClient;

  public AzureService(AzureSettings options)
  {
    _settings = options;
    _containerClient = new BlobContainerClient(_settings.ConnectionString, _settings.ContainerName);
  }

  public async Task UploadFileAsync(string filePath)
  {
    var fileName = Path.GetFileName(filePath);
    Console.WriteLine($"{fileName}");
    var blobClient = _containerClient.GetBlobClient($"{fileName}");

    await using var fileStream = File.OpenRead(filePath);
    var res = await blobClient.UploadAsync(fileStream, overwrite: true);
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