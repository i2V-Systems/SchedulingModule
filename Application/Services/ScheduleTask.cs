using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Managers;
using SchedulingModule.Application.Models;
using SchedulingModule.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.Services
{
    [ScopedService]
    public class ScheduledTaskService
    {
        private ScheduleDto _schedule;
        
        private  readonly IDispatcher _dispatcher;
        private readonly TopicAwareDispatcher _topicAwareDispatcher;
        private readonly IScheduleManager _scheduleManager;

        public ScheduledTaskService(IDispatcher dispatcher,IServiceProvider serviceProvider,IScheduleManager scheduleManager)
        {
            _scheduleManager = scheduleManager;
            _dispatcher = dispatcher;
            _topicAwareDispatcher = new TopicAwareDispatcher(serviceProvider);
            
        }

        public async Task ExecuteAsync(ScheduleDto schedule, IScheduler scheduler)
        {
            _schedule = schedule;
            
            var currentTime = DateTime.Now;
            if (currentTime >= _schedule.StartDateTime && currentTime <= _schedule.EndDateTime)
            {
                await ScheduleEventManager.scheduleEventService.ScheduleJob(HandleScheduledJob, schedule, scheduler);
                await ScheduleEventManager.scheduleEventService.ScheduleJob(HandleScheduledJob, schedule, scheduler);
            }
            else if (currentTime > _schedule.EndDateTime)
            {
                Console.WriteLine("Task execution skipped as it is outside the allowed schedule range.");
            }
        }

        public void HandleScheduledJob(Guid scheduleId, ScheduleEventType type)
        {
            var resourceMapping =  _scheduleManager.GetResourcesByScheduleId(scheduleId);
            foreach (var mapping in resourceMapping)
            {
                _topicAwareDispatcher.Broadcast(
                    new ScheduleEventTrigger(scheduleId, type, mapping.ResourceType)
                );
            }
          
        }

        public async Task UpdateAsync(ScheduleDto schedule)
        {
        }

        public async Task DeleteAsync(Guid id)
        {
        }
    }

}
