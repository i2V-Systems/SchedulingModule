using SchedulingModule.Interfaces;
using static SchedulingModule.ScheduleTypeEnum;

namespace SchedulingModule.Models
{

    
    public class Schedule : ISchedulesEntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Enum_ScheduleType Type { get; set; }
        //[NotMapped]
        public Enum_ScheduleSubType? SubType { get; set; }
        public string Details { get; set; }
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        public TimeSpan RecurringStartTime { get; set; }
        public TimeSpan RecurringEndTime { get; set; }
        


        // Property to store StartDateTime in UTC and retrieve in local time
        public DateTime StartDateTime
        {
            get => _startDateTime.ToLocalTime();
            set => _startDateTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        // Property to store EndDateTime in UTC and retrieve in local time
        //public DateTime RecurringStartTime
        //{
        //    get => _recurringStartTime.ToLocalTime();
        //    set => _recurringStartTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        //}
        //public DateTime RecurringEndTime
        //{
        //    get => _recurringEndTime.ToLocalTime();
        //    set => _recurringEndTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        //}

        // Property to store EndDateTime in UTC and retrieve in local time
        public DateTime EndDateTime
        {
            get => _endDateTime.ToLocalTime();
            set => _endDateTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        //public DateTime StartDateTime {get; set; }
        //public DateTime EndDateTime { get; set; }
        //public DateTime? RecurringTime { get; set; }
        public string? StartCronExp { get; set; }
        public string? StopCronExp { get; set; }

        public void CreateSchedules()
        {
            EntityIdGenerator.GenerateIdIfEmpty(this);
        }
        public void ConvertToUTC()
        {
            StartDateTime = StartDateTime.ToUniversalTime();
            EndDateTime = EndDateTime.ToUniversalTime();
        }
    }


}
