namespace backuppv2.Models;

public class FileBackupRecord
{
  public DateTime LastModifiedUtc { get; set; }
  public string Hash { get; set; } = "";

  public FileBackupRecord(string filePath)
  {
    LastModifiedUtc = File.GetLastAccessTimeUtc(filePath);
    Hash = FileHasher.ComputeHash(filePath);
  }

  [JsonConstructor]
  public FileBackupRecord(DateTime lastModifiedUtc, string hash)
  {
    LastModifiedUtc = lastModifiedUtc;
    Hash = hash;
  }
}