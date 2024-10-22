using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using SchedulingModule.Interfaces;

namespace SchedulingModule.Models
{
    public class Schedules : ISchedulesEntityBase
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public string StartCronExp { get; set; }
        public string StopCronExp { get; set; }

        public void CreateSchedules()
        {
            EntityIdGenerator.GenerateIdIfEmpty(this);
        }
    }
}
