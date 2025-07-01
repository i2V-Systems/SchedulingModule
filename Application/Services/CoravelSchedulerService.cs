using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.ScheduleStrategies;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;
using Serilog;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;


namespace SchedulingModule.Application.Services;

[ScopedService]
public class CoravelSchedulerService : ISchedulerTaskService
{
    private  ScheduleJobStrategy _strategyFactory;
    private readonly IServiceProvider _serviceProvider;
    
    public CoravelSchedulerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _strategyFactory = _serviceProvider.GetService<ScheduleJobStrategy>();
      
      
    }

    public  Task InitService()
    {
        return Task.CompletedTask;
    }
    public  void UnscheduleJob(Guid scheduleId, IScheduler scheduler)
        {
            try
            { 
                var iScheduler= scheduler as Scheduler;
                // Unschedule all related jobs
                iScheduler.TryUnschedule($"{scheduleId}_start");
                iScheduler.TryUnschedule($"{scheduleId}_end");
                iScheduler.TryUnschedule($"{scheduleId}_weekday_start");
                iScheduler.TryUnschedule($"{scheduleId}_weekday_end");
                iScheduler.TryUnschedule($"{scheduleId}_weekend_start");
                iScheduler.TryUnschedule($"{scheduleId}_weekend_end");
                iScheduler.TryUnschedule($"{scheduleId}_date_start");
                iScheduler.TryUnschedule($"{scheduleId}_date_end");
                Console.WriteLine($"Successfully unscheduled all jobs for schedule {scheduleId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unscheduling jobs for schedule {scheduleId}: {ex.Message}");
            }
        }
    public   void ExecuteStartEvent(Action<Guid,ScheduleEventType> taskToPerform, ScheduleDto schedule)
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
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler)
    {
        try {
            Log.Debug("{schedule.Id} executed for end at: ", DateTime.Now);
            taskToPerform(schedule.Id, ScheduleEventType.End);

            // Unschedule one-time schedules
            if (schedule.SubType == (ScheduleSubType?)ScheduleType.DateWise)
            {
                UnscheduleJob(schedule.Id, scheduler);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error in end event for schedule {schedule.Id}", ex);
        }
    }
    public  Task ScheduleJob(Action<Guid,ScheduleEventType> taskToPerform,ScheduleDto schedule,IScheduler scheduler)
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
