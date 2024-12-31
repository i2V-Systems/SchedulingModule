using Coravel.Events.Interfaces;

namespace SchedulingModule.Models
{
    public class ScheduledStartEventTrigger : IEvent
    {

        public DateTime TriggeredAt { get; set; }
        public Schedules schedule { get; set; }
         

        public ScheduledStartEventTrigger(Schedules _schedule)
        {
            TriggeredAt = DateTime.UtcNow;
            schedule= _schedule;
        }
    }
    public class ScheduledEndEventTrigger : IEvent
    {

        public DateTime TriggeredAt { get; set; }
        public Schedules schedule { get; set; }


        public ScheduledEndEventTrigger(Schedules _schedule)
        {
            TriggeredAt = DateTime.UtcNow;
            schedule = _schedule;
        }
    }
    public class ScheduledReccuringEventTrigger : IEvent
    {

        public DateTime TriggeredAt { get; set; }
        public Guid ScheduleId { get; set; }


        public ScheduledReccuringEventTrigger(Guid scheduleId)
        {
            TriggeredAt = DateTime.UtcNow;
            ScheduleId = scheduleId;
            //schedule = _schedule;
        }
    }



}
