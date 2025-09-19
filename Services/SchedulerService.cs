using System.Diagnostics;
using backuppv2.WinUI;
using Microsoft.Extensions.Logging;
using Quartz;

namespace backuppv2.Services;


public class SchedulerService
{
  private readonly ISchedulerFactory _schedulerFactory;
  private IScheduler _scheduler;

  public SchedulerService(ISchedulerFactory factory)
  {
    _schedulerFactory = factory;
  }

  public async Task SetupAsync()
  {
    Console.WriteLine("Set up scheduler");
    _scheduler = await _schedulerFactory.GetScheduler();
    await _scheduler.Start();
  }

  public async Task StartSchedule()
  {
    if (_scheduler == null)
    {
      Console.WriteLine("no scheduler");
      await SetupAsync();
      // return;
    }

    var job = JobBuilder.Create<BackupJob>()
        .WithIdentity("nightlyBackup", "backup")
        .Build();
    Console.WriteLine("Job Created");

    DateTime date = DateTime.Now;
    var trigger = TriggerBuilder.Create()
        .WithIdentity("backupTrigger", "backup")
        .StartNow()
        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(date.Hour, date.Minute + 1))
        .Build();
    Console.WriteLine("Trigger Created");

    await _scheduler.ScheduleJob(job, trigger);
    // await BackupJobSchedule.Scheduler.ScheduleJob(job, trigger);
    Console.WriteLine($" {date.Hour + ":" + (date.Minute + 1)} Job+Trigger Scheduled");
  }
}

public class BackupJob : IJob
{
  private readonly BackupsService _backupService;
  private readonly INotificationService _notificationService;

  private readonly ILogger logger;
  public BackupJob(BackupsService backupsService, INotificationService notificationService, ILogger logger)
  {
    Console.WriteLine("backup job constructor");
    Console.WriteLine(_backupService);
    Console.WriteLine(backupsService);
    _backupService = backupsService;
    _notificationService = notificationService;
    this.logger = logger;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    try
    {
      Console.WriteLine("Backing up on schedule");
      await _backupService.BackUpAllDirectories();
      _notificationService.ShowNotification("Back up Finished on Schedule", "");
    }
    catch (Exception e)
    {
      logger.Log(LogLevel.Error, "📆 " + e.Message);
    }
  }
}