using Quartz;

namespace backuppv2.Services;


public class SchedulerService
{

  private IScheduler scheduler;
  public SchedulerService()
  {
    StartSchedule();
  }

  public async void StartSchedule()
  {
    if (scheduler != null) { return; }

    scheduler = await Quartz.Impl.StdSchedulerFactory.GetDefaultScheduler();
    await scheduler.Start();

    var job = JobBuilder.Create<BackupJob>()
        .WithIdentity("nightlyBackup", "backup")
        .Build();

    var trigger = TriggerBuilder.Create()
        .WithIdentity("backupTrigger", "backup")
        .StartNow()
        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 0)) // 2:00 AM
        .Build();

    await scheduler.ScheduleJob(job, trigger);
  }
}

public class BackupJob(BackupsService backupsService) : IJob
{
  private readonly BackupsService backupsService = backupsService;

  public async Task Execute(IJobExecutionContext context)
  {
    await backupsService.BackUpAllDirectories();
  }
}