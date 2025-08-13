namespace backuppv2.Models;



public class BackupCache
{
  public Dictionary<string, DirectoryBackup> LinkedDirectories { get; set; } = [];

  public bool IsFileChanged(string filePath, string folderName)
  {
    DateTime lastModified = File.GetLastWriteTimeUtc(filePath);
    string fileHash = FileHasher.ComputeHash(filePath);
    LinkedDirectories.TryGetValue(folderName, out var directoryBackup);
    if (directoryBackup != null && directoryBackup.Files.TryGetValue(filePath, out var cached)) // is in cache
    {
      return cached.LastModifiedUtc != lastModified || cached.Hash != fileHash; // both same, or change has happened
    }

    return true; // no file in cache
  }


  public void UpdateRecord(string filePath, string folderName)
  {
    if (!LinkedDirectories.ContainsKey(folderName))
      LinkedDirectories.Add(folderName, new DirectoryBackup { Name = folderName, Files = [], FullPath = filePath });

    LinkedDirectories[folderName].Files[filePath] = new FileBackupRecord(filePath);
  }


  public void RemoveRecord(string filePath, string folderName)
  {
    LinkedDirectories.TryGetValue(folderName, out var directoryBackup);
    if (directoryBackup == null) return;
    directoryBackup.Files.Remove(filePath);
    if (directoryBackup.Files.Count == 0) LinkedDirectories.Remove(folderName);
  }


  public IEnumerable<string> GetTrackedFiles(string folderName)
  {
    if (!LinkedDirectories.ContainsKey(folderName)) return new Dictionary<string, FileBackupRecord>().Keys;
    return LinkedDirectories[folderName].Files.Keys;
  }


  public static BackupCache Load(string path)
  {
    if (!File.Exists(path))
      return new BackupCache();

    var json = File.ReadAllText(path);
    json = json.StartsWith('{') ? json : "{}";
    var loaded = JsonSerializer.Deserialize<BackupCache>(json) ?? new BackupCache();
    return loaded;
  }


  public void Save(string path)
  {
    var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(path, json);
  }

}