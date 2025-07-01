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
[ScheduleStrategy(ScheduleType.DateWise)]
public class DateWiseScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleType.DateWise, name: "Date-wise Schedule", description: "Executes tasks on specific dates");

    public bool CanHandle(ScheduleType scheduleType)
    {
        return scheduleType == ScheduleType.DateWise;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        var startTime = schedule.StartDateTime;
        var endTime = schedule.EndDateTime;

        scheduler.ScheduleDateWise(
            schedule.Id+ nameof(jobIds._date_start),
            () => eventExecutor.ExecuteStartEvent(taskToPerform, schedule),
            startTime.Hour,
            startTime.Minute,
            startTime.Date 
            );
        scheduler.ScheduleDateWise(
            schedule.Id+ nameof(jobIds._date_end),
            () => eventExecutor.ExecuteEndEvent(taskToPerform, schedule,scheduler),
            endTime.Hour,
            endTime.Minute,
            endTime.Date 
        );
        return Task.CompletedTask;
    }
}
    