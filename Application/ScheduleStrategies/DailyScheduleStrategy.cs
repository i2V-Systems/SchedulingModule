using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Application.Models;
using SchedulingModule.Application.Services;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.ScheduleStrategies;

[ScopedService]
[ScheduleStrategy(ScheduleType.Daily)]
public class DailyScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleType.Daily, name: "Daily Schedule", description: "Executes tasks daily at specific times");


    public bool CanHandle(ScheduleType scheduleType)
    {
        return scheduleType == ScheduleType.Daily;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        if (schedule.SubType == ScheduleSubType.Every)
        {
            // TODO: Implement every N days logic
            ScheduleStartAndEndEventsEvery(taskToPerform, schedule, scheduler, eventExecutor);
            return Task.CompletedTask;
        }

        ScheduleStartAndEndEvents(taskToPerform, schedule, scheduler,eventExecutor);
        return Task.CompletedTask;
    }

    private void ScheduleStartAndEndEvents(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler,ISchedulerTaskService eventExecutor)
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
    private void ScheduleStartAndEndEventsEvery(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        // Specialized logic for "every N weeks"
        Console.WriteLine($"Executing weekly every N days schedule for {schedule.Id}");
    }
}
