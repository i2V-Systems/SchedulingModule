using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;


namespace SchedulingModule.Application.Context
{
    public class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)
            : base(options)
        {
        }
        //public DbSet<ActionData> ActionData { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleResourceMapping> ScheduleResourceMapping { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedule>()
                .Property(e => e.Type)
                .HasConversion(
                    new EnumToStringConverter<ScheduleType>()
                );
         

            // Configure the SubType property to convert Enum_ScheduleSubType? to string in the database
            modelBuilder.Entity<Schedule>()
                .Property(e => e.SubType)
                .HasConversion(
                    new EnumToStringConverter<ScheduleSubType>()
                );
            
            modelBuilder.Entity<Schedule>()
                .Property(e => e.StartDays)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v ?? new List<Days>()),
                    v => string.IsNullOrEmpty(v) ? new List<Days>() : JsonConvert.DeserializeObject<List<Days>>(v) ?? new List<Days>()
                );
            
            modelBuilder
                .Entity<ScheduleResourceMapping>()
                .HasKey(pvs => new { pvs.ScheduleId, pvs.ResourceId });

            base.OnModelCreating(modelBuilder);
        }
    }

}
