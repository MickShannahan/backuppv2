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

    // Create client once per container
    _containerClient = new BlobContainerClient(_settings.ConnectionString, _settings.ContainerName);
  }

  public async Task UploadFileAsync(string filePath)
  {
    var fileName = Path.GetFileName(filePath);
    var blobClient = _containerClient.GetBlobClient(fileName);

    await using var fileStream = File.OpenRead(filePath);
    var res = await blobClient.UploadAsync(fileStream, overwrite: true);
  }
}