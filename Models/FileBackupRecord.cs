namespace backuppv2.Models;

public class FileBackupRecord
{
  public DateTime LastModifiedUtc { get; set; }
  public string Hash { get; set; } = "";
}