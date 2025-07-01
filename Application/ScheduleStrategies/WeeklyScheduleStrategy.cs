using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Application.Models;
using SchedulingModule.Application.Scheduler;
using SchedulingModule.Application.Services;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.ScheduleStrategies;

[ScopedService]
[ScheduleStrategy(ScheduleType.Weekly)]
public class WeeklyScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleType.Weekly, name: "Weekly Schedule", description: "Executes tasks on specific days of the week");
    public bool CanHandle(ScheduleType scheduleType)
    {
        return scheduleType ==ScheduleType.Weekly;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        var scheduledTask = schedule.SubType switch
        {
            ScheduleSubType.Selecteddays => ScheduleSelectedDays(taskToPerform, schedule, scheduler, eventExecutor),
            ScheduleSubType.Weekdays => ScheduleWeekdays(taskToPerform, schedule, scheduler, eventExecutor),
            _ => ScheduleWeekends(taskToPerform, schedule, scheduler, eventExecutor)
        };
        scheduledTask?.Invoke();

        return Task.CompletedTask;
    }

    private Action ScheduleSelectedDays(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        return () =>
        {
            var startCronJob = CronExpressionBuilder.BuildCronExpression(schedule.StartDays, schedule.StartDateTime);
            var endCronJob = CronExpressionBuilder.BuildCronExpression(schedule.StartDays, schedule.EndDateTime);

            scheduler.ScheduleSelectedDays(
                schedule.Id+ nameof(jobIds._start),
                () => eventExecutor.ExecuteStartEvent(taskToPerform, schedule),
                schedule.StartDateTime.Hour,
                schedule.StartDateTime.Minute,
                startCronJob);
            
            
            scheduler.ScheduleSelectedDays(
                schedule.Id+ nameof(jobIds._end),
                () => eventExecutor.ExecuteEndEvent(taskToPerform, schedule,scheduler),
                schedule.EndDateTime.Hour,
                schedule.EndDateTime.Minute,
                startCronJob);

          
        };
    }

    private Action ScheduleWeekdays(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        return () =>
        {
            var startTime = schedule.StartDateTime;
            var endTime = schedule.EndDateTime;
            scheduler.ScheduleWeekDays(
                schedule.Id+ nameof(jobIds._weekday_start),
                () => eventExecutor.ExecuteStartEvent(taskToPerform, schedule),
                startTime.Hour,
                startTime.Minute);
            
            scheduler.ScheduleWeekDays(
                schedule.Id+ nameof(jobIds._weekday_end),
                () => eventExecutor.ExecuteEndEvent(taskToPerform, schedule,scheduler),
                endTime.Hour,
                endTime.Minute);
        };
    }

    private Action ScheduleWeekends(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        return () =>
        {
            var startTime = schedule.StartDateTime;
            var endTime = schedule.EndDateTime;
            scheduler.ScheduleWeekDays(
                schedule.Id+ nameof(jobIds._weekend_start),
                () => eventExecutor.ExecuteStartEvent(taskToPerform, schedule),
                startTime.Hour,
                startTime.Minute);
            
            scheduler.ScheduleWeekDays(
                schedule.Id+ nameof(jobIds._weekend_end),
                () => eventExecutor.ExecuteEndEvent(taskToPerform, schedule,scheduler),
                endTime.Hour,
                endTime.Minute);
        };
    }
}
