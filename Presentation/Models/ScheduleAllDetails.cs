

using SchedulingModule.Application.DTOs;
using SchedulingModule.Domain.Entities;

namespace SchedulingModule.Presentation.Models
{ 
    public class SchedulAllDetails
    {
        public ScheduleDto schedules { get; set; }

        public HashSet<string> AttachedResources { get; set; } = new HashSet<string>();
        
    }
    
}


  
