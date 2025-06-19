using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using SchedulingModule.services;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.ScheduleStragies;
// Example of how to add a new strategy in the future
[ScopedService]
[ScheduleStrategy(ScheduleTypeEnum.Enum_ScheduleType.Custom)]
public class CustomScheduleStrategy : IScheduleJobStrategy
{
    public ScheduleTypeInfo SupportedType => new(ScheduleTypeEnum.Enum_ScheduleType.Custom, name: "Custom Schedule", description: "Custom scheduling logic");
    public bool CanHandle(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
    {
        return scheduleType == ScheduleTypeEnum.Enum_ScheduleType.Custom;
    }

    public Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor)
    {
        // Custom scheduling logic here
        Console.WriteLine($"Executing custom schedule for {schedule.Id}");
        return Task.CompletedTask;
    }
}
