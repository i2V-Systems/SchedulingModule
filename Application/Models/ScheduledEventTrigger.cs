using Coravel.Events.Interfaces;
using SchedulingModule.Application.Enums;

namespace SchedulingModule.Application.Models
{
    public class ScheduleEventTrigger : IEvent
    {
        public DateTime triggeredAt { get; set; }
        public Guid scheduleId { get; set; }
        public ScheduleEventType eventType { get; set; }
        
        public string eventTopic { get; set; }
        public ScheduleEventTrigger(Guid id,ScheduleEventType type, string topic)
        {
            triggeredAt = DateTime.UtcNow;
            scheduleId= id;
            eventType = type;
            eventTopic = topic;
        }
    }
}
