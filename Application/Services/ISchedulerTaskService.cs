using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Scheduler;

namespace SchedulingModule.Application.Services;
public interface  ISchedulerTaskService
{ 
    public  Task InitService();
    public  void UnscheduleJob(Guid scheduleId, IUnifiedScheduler scheduler); 
    public   void ExecuteStartEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule);
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule,IUnifiedScheduler scheduler);
    public  Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IUnifiedScheduler scheduler);

}
