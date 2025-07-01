using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Domain.Entities;

namespace SchedulingModule.Application.Services;
public interface  ISchedulerTaskService
{ 
    public  Task InitService();
    public  void UnscheduleJob(Guid scheduleId, IScheduler scheduler); 
    public   void ExecuteStartEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule);
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule,IScheduler scheduler);
    public  Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler);

}
