using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.services;

[ScopedService]
public class HangFireSchedulerService :ISchedulerTaskService
{
    public HangFireSchedulerService() { }
    public  Task InitService() { return Task.CompletedTask; }
    public  void UnscheduleJob(Schedule schedule, IScheduler scheduler) { }
    public  void ExecuteStartEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule) { }
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler) { }

    public  Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler) { return Task.CompletedTask; }

    public void test()
    {
        
    }
}
