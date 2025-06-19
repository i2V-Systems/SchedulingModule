using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using SchedulingModule.Models;


namespace SchedulingModule.Context
{
    public class SchedulingModuleDbContext : DbContext
    {
        public SchedulingModuleDbContext(DbContextOptions<SchedulingModuleDbContext> options)
            : base(options)
        {
        }
        //public DbSet<ActionData> ActionData { get; set; }

        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<ScheduleResourceMapping> ScheduleResourceMapping { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedule>()
                .Property(e => e.Type)
                .HasConversion(
                    new EnumToStringConverter<ScheduleTypeEnum.Enum_ScheduleType>()
                );
         

            // Configure the SubType property to convert Enum_ScheduleSubType? to string in the database
            modelBuilder.Entity<Schedule>()
                .Property(e => e.SubType)
                .HasConversion(
                    new EnumToStringConverter<ScheduleTypeEnum.Enum_ScheduleSubType>()
                );
            
            modelBuilder.Entity<Schedule>()
                .Property(e => e.StartDays)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v ?? new List<string>()),
                    v => string.IsNullOrEmpty(v) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
                );
            
            modelBuilder
                .Entity<ScheduleResourceMapping>()
                .HasKey(pvs => new { pvs.ScheduleId, pvs.ResourceId });

            base.OnModelCreating(modelBuilder);
        }
    }

}
