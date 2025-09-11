namespace backuppv2.Models;

public class AppSettings
{
  public string AzureConnectionString { get; set; } = "";
  public string ContainerName { get; set; } = "";
  public bool WatchForChanges { get; set; } = false;
  public int FrequencyMinutes { get; set; } = 0;
  public string ScheduledTime { get; set; } = ""; // e.g., "02:00"
}

public class AppTheme
{
  public string BackgroundImage { get; set; } = "";
  public string ColorPrimary { get; set; } = "indigo";
  public string ColorSecondary { get; set; } = "blue";
}