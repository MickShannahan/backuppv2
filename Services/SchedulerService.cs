using System.Diagnostics;
using Quartz;

namespace backuppv2.Services;


public class SchedulerService
{

  public SchedulerService()
  {
    // StartSchedule();
  }

  public async void StartSchedule()
  {
    // if (scheduler != null) { return; }

    // scheduler = await Quartz.Impl.StdSchedulerFactory.GetDefaultScheduler();
    // await scheduler.Start();

    var job = JobBuilder.Create<BackupJob>()
        .WithIdentity("nightlyBackup", "backup")
        .Build();


    DateTime date = DateTime.Now;
    var trigger = TriggerBuilder.Create()
        .WithIdentity("backupTrigger", "backup")
        .StartNow()
        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(date.Hour, date.Minute + 1)) // 2:00 AM
        .Build();

    // await scheduler.ScheduleJob(job, trigger);
    await BackupJobSchedule.Scheduler.ScheduleJob(job, trigger);
    Console.WriteLine("Schedule Created");
    Console.WriteLine(date.Hour + ":" + (date.Minute + 1));
  }
}

public class BackupJob : IJob
{
  private BackupsService backupsService;
  public BackupJob(BackupsService backupsService)
  {
    Console.WriteLine("howdy partner");
    this.backupsService = backupsService;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    try
    {
      Console.WriteLine("Backing up on schedule");
      await backupsService.BackUpAllDirectories();
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