namespace backuppv2.Models;

public class DirectoryBackup : DirectoryCurrent
{
  public int FileCount => Files.Count;
  public int SubdirectoryCount => Subdirectories.Count();
  public string TotalSize => FormatBytes(GetTotalFileSize());

  private long GetTotalFileSize()
  {
    return Files.Keys
                .Where(File.Exists)
                .Select(path => new FileInfo(path).Length)
                .Sum();
  }

  public IEnumerable<string> Subdirectories =>
      Files.Keys
           .Select(path => Path.GetDirectoryName(path) ?? string.Empty)
           .Where(dir => !string.IsNullOrWhiteSpace(dir))
           .Distinct();
}