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
[ScheduleStrategy(ScheduleType.DateWise)]
public class DateWiseScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleType.DateWise, name: "Date-wise Schedule", description: "Executes tasks on specific dates");

    public bool CanHandle(ScheduleType scheduleType)
    {
        return scheduleType == ScheduleType.DateWise;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
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
    