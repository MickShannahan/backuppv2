using System.Diagnostics;
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
  private BackupsService _backupService;
  public BackupJob(BackupsService backupsService)
  {
    Console.WriteLine("backup job constructor");
    Console.WriteLine(_backupService);
    Console.WriteLine(backupsService);
    _backupService = backupsService;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    try
    {
      Console.WriteLine("Backing up on schedule");
      await _backupService.BackUpAllDirectories();
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }
}

public static class BackupJobSchedule
{
  public static IScheduler Scheduler;

  public async static void SetScheduler(ISchedulerFactory schedulerFactory)
  {
    var scheduler = await schedulerFactory.GetScheduler();
    Scheduler = scheduler;
    await Scheduler.Start();
    Console.WriteLine("schedule setup");
  }
}