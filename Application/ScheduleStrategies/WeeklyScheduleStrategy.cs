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
[ScheduleStrategy(ScheduleType.Weekly)]
public class WeeklyScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleType.Weekly, name: "Weekly Schedule", description: "Executes tasks on specific days of the week");
    public bool CanHandle(ScheduleType scheduleType)
    {
        return scheduleType ==ScheduleType.Weekly;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
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

    private Action ScheduleSelectedDays(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        return () =>
        {
            var startCronJob = CronExpressionBuilder.BuildCronExpression(schedule.StartDays, schedule.StartDateTime);
            var endCronJob = CronExpressionBuilder.BuildCronExpression(schedule.StartDays, schedule.EndDateTime);

            scheduler.Schedule(() => eventExecutor.ExecuteStartEvent(taskToPerform, schedule))
                .Cron(startCronJob)
                .Zoned(TimeZoneInfo.Local)
                .PreventOverlapping($"{schedule.Id}_start");

            scheduler.Schedule(() => eventExecutor.ExecuteEndEvent(taskToPerform, schedule, scheduler))
                .Cron(endCronJob)
                .Zoned(TimeZoneInfo.Local)
                .PreventOverlapping($"{schedule.Id}_end");
        };
    }

    private Action ScheduleWeekdays(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        return () =>
        {
            var startTime = schedule.StartDateTime;
            var endTime = schedule.EndDateTime;

            scheduler.Schedule(() => eventExecutor.ExecuteStartEvent(taskToPerform, schedule))
                .DailyAt(startTime.Hour, startTime.Minute)
                .Weekday()
                .Zoned(TimeZoneInfo.Local)
                .PreventOverlapping($"{schedule.Id}_weekday_start");

            scheduler.Schedule(() => eventExecutor.ExecuteEndEvent(taskToPerform, schedule, scheduler))
                .DailyAt(endTime.Hour, endTime.Minute)
                .Weekday()
                .Zoned(TimeZoneInfo.Local)
                .PreventOverlapping($"{schedule.Id}_weekday_end");
        };
    }

    private Action ScheduleWeekends(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        return () =>
        {
            var startTime = schedule.StartDateTime;
            var endTime = schedule.EndDateTime;

            scheduler.Schedule(() => eventExecutor.ExecuteStartEvent(taskToPerform, schedule))
                .DailyAt(startTime.Hour, startTime.Minute)
                .Weekend()
                .Zoned(TimeZoneInfo.Local)
                .PreventOverlapping($"{schedule.Id}_weekend_start");

            scheduler.Schedule(() => eventExecutor.ExecuteEndEvent(taskToPerform, schedule, scheduler))
                .DailyAt(endTime.Hour, endTime.Minute)
                .Weekend()
                .Zoned(TimeZoneInfo.Local)
                .PreventOverlapping($"{schedule.Id}_weekend_end");
        };
    }
}
