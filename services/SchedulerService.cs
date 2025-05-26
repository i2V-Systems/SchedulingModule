using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using SchedulingModule.Abstract;
using SchedulingModule.Context;
using SchedulingModule.Models;
using SchedulingModule.Repositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.services
{
    [TransientService]
    public class SchedulerService
    {
        private ISchedulesEntityBaseRepository<Schedule> schedulesRepository;
        private ISchedulesEntityBaseRepository<ScheduleResourceMapping> schedulesResourceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
     

        public SchedulerService(SchedulingModuleDbContext scheduleDbContext, IHttpContextAccessor httpContextAccessor)
        {
            
            _httpContextAccessor = httpContextAccessor;
            var httpContext = _httpContextAccessor.HttpContext;
          

            //if (httpContext != null && httpContext.Request.Headers.TryGetValue("Userid", out var userId))
            //{
            //    schedulesRepository = new SchedulesEntityBaseRepository<Schedules>(scheduleDbContext, new Guid(userId));
            //}
            //else
            //{
                schedulesRepository = new SchedulesEntityBaseRepository<Schedule>(scheduleDbContext);
                schedulesResourceRepository =
                    new SchedulesEntityBaseRepository<ScheduleResourceMapping>(scheduleDbContext);
                //}


        }

        public Schedule Get(Guid id)
        {
            return schedulesRepository.Get(id);
        }

        public List<Schedule> GetAllSchedules()
        {
            return schedulesRepository.GetAll();
        }

        public void Add(Schedule entity, string userName = "")
        {
            schedulesRepository.Add(entity);
        }

        public void AddResourceMapping(ScheduleResourceMapping map)
        {
            schedulesResourceRepository.Add(map);
        }
        
        public void Delete(Schedule entity, string userName = "")
        {
         
            schedulesRepository.Delete(entity);
        }

        //public List<Schedules> FromSql(string query)
        //{
        //    return schedulesRepository.FromSql(query);
        //}

        public void Update(Schedule entity, string userName = "")
        {
        
            schedulesRepository.Update(entity);
            schedulesRepository.DetachEntity(entity);
        }
    }
}
