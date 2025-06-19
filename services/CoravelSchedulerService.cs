using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using SchedulingModule.ScheduleStragies;
using Serilog;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;
using static SchedulingModule.ScheduleTypeEnum;
namespace SchedulingModule.services;

[ScopedService]
public class CoravelSchedulerService : ISchedulerTaskService
{
    private  ScheduleJobStrategy _strategyFactory;
    private readonly IServiceProvider _serviceProvider;
    
    public CoravelSchedulerService(IServiceProvider serviceProvider)
    {
        _strategyFactory = _serviceProvider.GetService<ScheduleJobStrategy>();
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      
    }

    public  Task InitService()
    {
        return Task.CompletedTask;
    }
    public  void UnscheduleJob(Schedule schedule, IScheduler scheduler)
        {
            try
            { 
                var iScheduler= scheduler as Scheduler;
                // Unschedule all related jobs
                iScheduler.TryUnschedule($"{schedule.Id}_start");
                iScheduler.TryUnschedule($"{schedule.Id}_end");
                iScheduler.TryUnschedule($"{schedule.Id}_weekday_start");
                iScheduler.TryUnschedule($"{schedule.Id}_weekday_end");
                iScheduler.TryUnschedule($"{schedule.Id}_weekend_start");
                iScheduler.TryUnschedule($"{schedule.Id}_weekend_end");
                iScheduler.TryUnschedule($"{schedule.Id}_date_start");
                iScheduler.TryUnschedule($"{schedule.Id}_date_end");
                Console.WriteLine($"Successfully unscheduled all jobs for schedule {schedule.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unscheduling jobs for schedule {schedule.Id}: {ex.Message}");
            }
        }
    public   void ExecuteStartEvent(Action<Guid,ScheduleEventType> taskToPerform, Schedule schedule)
    {
        try {
            Log.Debug("{schedule.Id} executed for start at: ", DateTime.Now);
            taskToPerform(schedule.Id, ScheduleEventType.Start);
        }
        catch (Exception ex)
        {
            Log.Error($"Error in start event for schedule {schedule.Id}", ex);
        }
    }
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler)
    {
        try {
            Log.Debug("{schedule.Id} executed for end at: ", DateTime.Now);
            taskToPerform(schedule.Id, ScheduleEventType.End);

            // Unschedule one-time schedules
            if (schedule.Type == Enum_ScheduleType.DateWise)
            {
                UnscheduleJob(schedule, scheduler);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error in end event for schedule {schedule.Id}", ex);
        }
    }
    public  Task ScheduleJob(Action<Guid,ScheduleEventType> taskToPerform,Schedule schedule,IScheduler scheduler)
    {
        try
        {
            var strategy = _strategyFactory.GetStrategy(schedule.Type);
            return strategy.ScheduleJob(taskToPerform, schedule, scheduler,this);
        }
        catch (Exception ex)
        {
            Log.Error($"Error scheduling job for schedule {schedule.Id}", ex);
            throw;
        }
    }
}
