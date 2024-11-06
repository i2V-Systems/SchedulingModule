using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        public DbSet<Schedules> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedules>()
                .Property(e => e.Type)
                .HasConversion(
                    new EnumToStringConverter<ScheduleTypeEnum.Enum_ScheduleType>()
                );

            // Configure the SubType property to convert Enum_ScheduleSubType? to string in the database
            modelBuilder.Entity<Schedules>()
                .Property(e => e.SubType)
                .HasConversion(
                    new EnumToStringConverter<ScheduleTypeEnum.Enum_ScheduleSubType>()
                );

            base.OnModelCreating(modelBuilder);
        }
    }

}
