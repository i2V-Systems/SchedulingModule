using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using SchedulingModule.services;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.ScheduleStragies;

[ScopedService]
[ScheduleStrategy(ScheduleTypeEnum.Enum_ScheduleType.DateWise)]
public class DateWiseScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleTypeEnum.Enum_ScheduleType.DateWise, name: "Date-wise Schedule", description: "Executes tasks on specific dates");

    public bool CanHandle(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
    {
        return scheduleType == ScheduleTypeEnum.Enum_ScheduleType.DateWise;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        var startTime = schedule.StartDateTime;
        var endTime = schedule.EndDateTime;

        scheduler.Schedule(() => eventExecutor.ExecuteStartEvent(taskToPerform, schedule))
            .DailyAt(startTime.Hour, startTime.Minute)
            .Zoned(TimeZoneInfo.Local)
            .PreventOverlapping($"{schedule.Id}_date_start")
            .When(() => Task.FromResult(startTime.Date == DateTime.Now.Date));

        scheduler.Schedule(() => eventExecutor.ExecuteEndEvent(taskToPerform, schedule, scheduler))
            .DailyAt(endTime.Hour, endTime.Minute)
            .Zoned(TimeZoneInfo.Local)
            .PreventOverlapping($"{schedule.Id}_date_end")
            .When(() => Task.FromResult(endTime.Date == DateTime.Now.Date));

        return Task.CompletedTask;
    }
}
    