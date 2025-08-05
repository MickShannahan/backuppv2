namespace backuppv2.Models;

public class AppSettings
{
  public List<DirectoryBackup> Directories { get; set; } = [];
  public string AzureConnectionString { get; set; } = "";
  public string ContainerName { get; set; } = "";
  public bool WatchForChanges { get; set; } = false;
  public int FrequencyMinutes { get; set; } = 0;
  public string ScheduledTime { get; set; } = ""; // e.g., "02:00"
}