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
        private ISchedulesEntityBaseRepository<Schedules> schedulesRepository;
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
                schedulesRepository = new SchedulesEntityBaseRepository<Schedules>(scheduleDbContext);
            //}
           
            
        }

        public Schedules Get(Guid id)
        {
            return schedulesRepository.Get(id);
        }

        public List<Schedules> GetAllSchedules()
        {
            return schedulesRepository.GetAll();
        }

        public void Add(Schedules entity, string userName = "")
        {
            schedulesRepository.Add(entity);
        }

        public void Delete(Schedules entity, string userName = "")
        {
         
            schedulesRepository.Delete(entity);
        }

        //public List<Schedules> FromSql(string query)
        //{
        //    return schedulesRepository.FromSql(query);
        //}

        public void Update(Schedules entity, string userName = "")
        {
        
            schedulesRepository.Update(entity);
            schedulesRepository.DetachEntity(entity);
        }
    }
}
