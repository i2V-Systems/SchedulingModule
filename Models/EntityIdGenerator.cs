using MassTransit;
using SchedulingModule.Interfaces;

namespace SchedulingModule.Models
{
    public class EntityIdGenerator
    {
        public static void GenerateIdIfEmpty(ISchedulesEntityBase entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = NewId.NextSequentialGuid();
            }
        }

        public static Guid GenerateGuid()
        {
            return NewId.NextSequentialGuid();
        }
    }
}
