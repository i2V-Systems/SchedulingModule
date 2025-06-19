using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using SchedulingModule.services;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.ScheduleStragies;

[ScopedService]
[ScheduleStrategy(ScheduleTypeEnum.Enum_ScheduleType.Weekly)]
public class WeeklyScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleTypeEnum.Enum_ScheduleType.Weekly, name: "Weekly Schedule", description: "Executes tasks on specific days of the week");
    public bool CanHandle(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
    {
        return scheduleType == ScheduleTypeEnum.Enum_ScheduleType.Weekly;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        switch (schedule.SubType)
        {
            case ScheduleTypeEnum.Enum_ScheduleSubType.Selecteddays:
                ScheduleSelectedDays(taskToPerform, schedule, scheduler, eventExecutor);
                break;
            case ScheduleTypeEnum.Enum_ScheduleSubType.Weekdays:
                ScheduleWeekdays(taskToPerform, schedule, scheduler, eventExecutor);
                break;
            default:
                ScheduleWeekends(taskToPerform, schedule, scheduler, eventExecutor);
                break;
        }

        return Task.CompletedTask;
    }

    private void ScheduleSelectedDays(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
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
    }

    private void ScheduleWeekdays(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
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
    }

    private void ScheduleWeekends(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
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
    }
}
