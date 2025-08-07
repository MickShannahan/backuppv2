namespace backuppv2.Models;


public class BackupCache
{
  public Dictionary<string, Dictionary<string, FileBackupRecord>> Files { get; set; } = [];
  private Dictionary<string, Dictionary<string, FileBackupRecord>> _files => Files;

  public bool IsFileChanged(string filePath, string folderName)
  {
    DateTime lastModified = File.GetLastWriteTimeUtc(filePath);
    string fileHash = FileHasher.ComputeHash(filePath);
    _files.TryGetValue(folderName, out var trackedFolder);
    if (trackedFolder != null && trackedFolder.TryGetValue(filePath, out var cached)) // is in cache
    {
      Console.WriteLine($">{cached.Hash} : {cached.LastModifiedUtc}");
      return cached.LastModifiedUtc != lastModified || cached.Hash != fileHash; // both same, or change has happened
    }

    return true; // no file in cache
  }

  public void UpdateRecord(string filePath, string folderName)
  {
    DateTime lastModified = File.GetLastWriteTimeUtc(filePath);
    string fileHash = FileHasher.ComputeHash(filePath);
    if (!_files.ContainsKey(folderName)) _files.Add(folderName, []);
    _files[folderName][filePath] = new FileBackupRecord() { LastModifiedUtc = lastModified, Hash = fileHash };
  }

  public void RemoveRecord(string filePath, string folderName)
  {
    _files.TryGetValue(folderName, out var trackedFolder);
    if (trackedFolder == null) return;
    _files[folderName].Remove(filePath);
    if (_files[folderName].Count == 0) _files.Remove(folderName);
  }

  public IEnumerable<string> GetTrackedFiles(string folderName)
  {
    if (!_files.ContainsKey(folderName)) return new Dictionary<string, FileBackupRecord>().Keys;
    return _files[folderName].Keys;
  }

  public static BackupCache Load(string path)
  {
    if (!File.Exists(path))
      return new BackupCache();

    var json = File.ReadAllText(path);
    var loaded = JsonSerializer.Deserialize<BackupCache>(json) ?? new BackupCache();
    return loaded;
  }

  public void Save(string path)
  {
    var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(path, json);
  }

}