using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using SchedulingModule.services;
using Serilog;

namespace SchedulingModule.ScheduleStragies;

// Enhanced interface with metadata for automatic discovery
public interface IScheduleJobStrategy
{
        // Strategy metadata for automatic discovery
        ScheduleTypeInfo SupportedType { get; }
        bool CanHandle(ScheduleTypeEnum.Enum_ScheduleType scheduleType);
        Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler, ISchedulerTaskService eventExecutor);
}
// Metadata class for strategy discovery
public class ScheduleTypeInfo
{
        public ScheduleTypeEnum.Enum_ScheduleType ScheduleType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public ScheduleTypeInfo(ScheduleTypeEnum.Enum_ScheduleType scheduleType, string name = null, string description = null)
        {
                ScheduleType = scheduleType;
                Name = name ?? scheduleType.ToString();
                Description = description ?? $"Strategy for {scheduleType}";
        }
        
}
// Attribute for marking strategy classes (optional but helpful for reflection-based discovery)
[AttributeUsage(AttributeTargets.Class)]
public class ScheduleStrategyAttribute : Attribute
{
        public ScheduleTypeEnum.Enum_ScheduleType ScheduleType { get; }
        public ScheduleTypeEnum.Enum_ScheduleSubType? SubType { get; }
        public int Priority { get; }
        public ScheduleStrategyAttribute(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
        {
                ScheduleType = scheduleType;
        }
}
