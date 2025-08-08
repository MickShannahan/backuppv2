namespace backuppv2.Models;

public class DirectoryBackup
{
  public string? Name { get; set; }
  public string? FullPath { get; set; }
  public int ByteSize { get; set; }
  public Dictionary<string, FileBackupRecord> Files { get; set; } = [];

  public int FileCount => Files.Count;

  // Computed: total size in human-readable format
  public string TotalSize => FormatBytes(GetTotalFileSize());

  private long GetTotalFileSize()
  {
    return Files.Keys
                .Where(File.Exists)
                .Select(path => new FileInfo(path).Length)
                .Sum();
  }

  // Computed: unique subdirectories from file paths
  public IEnumerable<string> Subdirectories =>
      Files.Keys
           .Select(path => Path.GetDirectoryName(path) ?? string.Empty)
           .Where(dir => !string.IsNullOrWhiteSpace(dir))
           .Distinct();

  // Computed: count of subdirectories
  public int SubdirectoryCount => Subdirectories.Count();

  private static string FormatBytes(long bytes)
  {
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    double len = bytes;
    int order = 0;

    while (len >= 1024 && order < sizes.Length - 1)
    {
      order++;
      len /= 1024;
    }

    return $"{len:0.##} {sizes[order]}";
  }
}