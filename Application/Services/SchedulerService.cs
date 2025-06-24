using SchedulingModule.Domain.Abstract;
using SchedulingModule.Domain.Context;
using SchedulingModule.Domain.Models;
using SchedulingModule.Domain.Repositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.Services
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
            schedulesRepository = new SchedulesEntityBaseRepository<Schedule>(scheduleDbContext);
            schedulesResourceRepository =
                    new SchedulesEntityBaseRepository<ScheduleResourceMapping>(scheduleDbContext);
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
        public List<ScheduleResourceMapping> GetAllResourceMapping()
        {
            return schedulesResourceRepository.GetAll();
        }
        public void Delete(Schedule entity, string userName = "")
        {
            schedulesRepository.Delete(entity);
        }

        public void Update(Schedule entity, string userName = "")
        {
            schedulesRepository.Update(entity);
            schedulesRepository.DetachEntity(entity);
        }
    }
}
