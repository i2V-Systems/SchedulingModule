using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.Enums;
using SchedulingModule.Domain.Models;

namespace SchedulingModule.Application.Services;
public interface  ISchedulerTaskService
{ 
    public  Task InitService();
    public  void UnscheduleJob(Schedule schedule, IScheduler scheduler); 
    public   void ExecuteStartEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule);
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule,IScheduler scheduler);
    public  Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler);

}
