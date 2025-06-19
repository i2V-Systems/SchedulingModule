
namespace SchedulingModule.Models
{ 
    public class SchedulAllDetails
    {
        public Schedule schedules { get; set; }

        public HashSet<string> AttachedResources { get; set; } = new HashSet<string>();
        
    }
    
}


  
