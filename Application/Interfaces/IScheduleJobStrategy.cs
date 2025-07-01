using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Models;
using SchedulingModule.Application.Services;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.Interfaces;

public interface IScheduleJobStrategy
{
        ScheduleTypeInfo SupportedType { get; }
        bool CanHandle(ScheduleType scheduleType);
        Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, ScheduleDto schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor); 
}