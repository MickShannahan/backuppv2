namespace backuppv2.Models;

public class DirectoryCurrent
{
  public string? Name { get; set; }
  public string? FullPath { get; set; }
  public Dictionary<string, FileBackupRecord> Files { get; set; } = [];

  public bool isBusy => currentJobCount < jobCount;

  public int jobCount { get; set; } = 0;
  public int currentJobCount { get; set; } = 0;

  public DateTime? LastBackupTime =>
    Files.Values.OrderByDescending(f => f.LastModifiedUtc).FirstOrDefault()?.LastModifiedUtc;

  public int CurrentFileCount => GetCurrentFiles().Count();

  public List<string> MissingFiles =>
    Files.Keys.Where(path => !File.Exists(path)).ToList();

  public List<string> NewFiles =>
    GetCurrentFiles().Where(path => !Files.ContainsKey(path)).ToList();

  public List<string> ChangedFiles =>
    GetCurrentFiles().Where(path => Files.ContainsKey(path) &&
     (File.GetLastWriteTimeUtc(path) > Files[path].LastModifiedUtc || Files[path].Hash != FileHasher.ComputeHash(path))).ToList();

  public string CurrentTotalSize => FormatBytes(GetCurrentFiles()
    .Where(File.Exists)
    .Select(path => new FileInfo(path).Length)
    .Sum());

  public IEnumerable<string> GetCurrentFiles()
  {
    if (string.IsNullOrWhiteSpace(FullPath) || !Directory.Exists(FullPath))
      return Enumerable.Empty<string>();

    return Directory.GetFiles(FullPath, "*", SearchOption.AllDirectories);
  }

  internal static string FormatBytes(long bytes)
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