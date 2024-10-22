using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }

}
