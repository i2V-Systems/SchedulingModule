using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.Models;

[AttributeUsage(AttributeTargets.Class)]
public class ScheduleStrategyAttribute: Attribute
{
    public ScheduleTypeEnum.Enum_ScheduleType ScheduleType { get; }
    public ScheduleTypeEnum.Enum_ScheduleSubType? SubType { get; }
    public int Priority { get; }
    public ScheduleStrategyAttribute(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
    {
        ScheduleType = scheduleType;
    }
}