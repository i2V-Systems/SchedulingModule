using SchedulingModule.Domain.Enums;
using SchedulingModule.Domain.Interfaces;

namespace SchedulingModule.Domain.Models
{
    public class Schedule : ISchedulesEntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ScheduleTypeEnum.Enum_ScheduleType Type { get; set; }
        //[NotMapped]
        public ScheduleTypeEnum.Enum_ScheduleSubType? SubType { get; set; }
        public string Details { get; set; }
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        public int? NoOfDays { get; set; }
        public List<string> StartDays { get; set; }
        
        


        // Property to store StartDateTime in UTC and retrieve in local time
        public DateTime StartDateTime
        {
            get => _startDateTime.ToLocalTime();
            set => _startDateTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
        public DateTime EndDateTime
        {
            get => _endDateTime.ToLocalTime();
            set => _endDateTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
        
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
