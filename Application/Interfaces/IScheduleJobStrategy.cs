using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Models;
using SchedulingModule.Application.Services;
using SchedulingModule.Domain.Enums;
using SchedulingModule.Domain.Models;

namespace SchedulingModule.Application.Interfaces;

public interface IScheduleJobStrategy
{
        ScheduleTypeInfo SupportedType { get; }
        bool CanHandle(ScheduleTypeEnum.Enum_ScheduleType scheduleType);
        Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor); 
}