using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.Models;

[AttributeUsage(AttributeTargets.Class)]
public class ScheduleStrategyAttribute: Attribute
{
    public ScheduleType ScheduleType { get; }
    public ScheduleSubType? SubType { get; }
    public int Priority { get; }
    public ScheduleStrategyAttribute(ScheduleType scheduleType)
    {
        ScheduleType = scheduleType;
    }
}