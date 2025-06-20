using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using SchedulingModule.services;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.ScheduleStragies;

[ScopedService]
[ScheduleStrategy(ScheduleTypeEnum.Enum_ScheduleType.Daily)]
public class DailyScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleTypeEnum.Enum_ScheduleType.Daily, name: "Daily Schedule", description: "Executes tasks daily at specific times");


    public bool CanHandle(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
    {
        return scheduleType == ScheduleTypeEnum.Enum_ScheduleType.Daily;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        if (schedule.SubType == ScheduleTypeEnum.Enum_ScheduleSubType.Every)
        {
            // TODO: Implement every N days logic
            ScheduleStartAndEndEventsEvery(taskToPerform, schedule, scheduler, eventExecutor);
            return Task.CompletedTask;
        }

        ScheduleStartAndEndEvents(taskToPerform, schedule, scheduler,eventExecutor);
        return Task.CompletedTask;
    }

    private void ScheduleStartAndEndEvents(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler,ISchedulerTaskService eventExecutor)
    {
        var startTime = schedule.StartDateTime;
        var endTime = schedule.EndDateTime;

        scheduler.Schedule(() => eventExecutor.ExecuteStartEvent(taskToPerform, schedule))
            .DailyAt(startTime.Hour, startTime.Minute)
            .Zoned(TimeZoneInfo.Local)
            .PreventOverlapping($"{schedule.Id}_start");
        
        scheduler.Schedule(() => eventExecutor.ExecuteEndEvent(taskToPerform, schedule, scheduler))
            .DailyAt(endTime.Hour, endTime.Minute)
            .Zoned(TimeZoneInfo.Local)
            .PreventOverlapping($"{schedule.Id}_end");
    }
    private void ScheduleStartAndEndEventsEvery(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        // Specialized logic for "every N weeks"
        Console.WriteLine($"Executing weekly every N days schedule for {schedule.Id}");
    }
}
