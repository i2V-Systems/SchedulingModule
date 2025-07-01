using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Domain.Entities;


public class ScheduleResourceMapping :BaseEntity
{
    public Guid ScheduleId { get; set; }
    public Guid ResourceId { get; set; }
    
    public Resources ResourceType { get; set; }


}