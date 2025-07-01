using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.Services;

[ScopedService]
public class HangFireSchedulerService :ISchedulerTaskService
{
    public HangFireSchedulerService() { }
    public  Task InitService() { return Task.CompletedTask; }
    public  void UnscheduleJob(Guid scheduleId, IScheduler scheduler) { }
    public  void ExecuteStartEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule) { }
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler) { }

    public  Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler) { return Task.CompletedTask; }

    public void test()
    {
        
    }
}
