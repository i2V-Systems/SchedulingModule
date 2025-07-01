using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.Models;

public class ScheduleTypeInfo
{
    public ScheduleType ScheduleType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ScheduleTypeInfo(ScheduleType scheduleType, string name = null, string description = null)
    {
        ScheduleType = scheduleType;
        Name = name ?? scheduleType.ToString();
        Description = description ?? $"Strategy for {scheduleType}";
    }
}
