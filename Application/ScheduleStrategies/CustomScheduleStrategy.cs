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
// Example of how to add a new strategy in the future
[ScopedService]
[ScheduleStrategy(ScheduleType.Custom)]
public class CustomScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleType.Custom, name: "Custom Schedule", description: "Custom scheduling logic");
    public bool CanHandle(ScheduleType scheduleType)
    {
        return scheduleType == ScheduleType.Custom;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        // Custom scheduling logic here
        Console.WriteLine($"Executing custom schedule for {schedule.Id}");
        return Task.CompletedTask;
    }
}
