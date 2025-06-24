
using SchedulingModule.Domain.Interfaces;

namespace SchedulingModule.Domain.Models;

public class ScheduleResourceMapping :ISchedulesEntityBase
{
   
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid ResourceId { get; set; }
        public string ResourceType { get; set; }

        public void CreateScheduleResourceMapping()
        {
            EntityIdGenerator.GenerateIdIfEmpty(this);
        }

}