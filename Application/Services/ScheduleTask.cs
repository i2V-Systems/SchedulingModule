using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.Enums;
using SchedulingModule.Application.Managers;
using SchedulingModule.Application.Models;
using SchedulingModule.Domain.Models;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.Services
{
    [ScopedService]
    public class ScheduledTaskService
    {
        private Schedule _schedule;
        public IDispatcher _dispatcher;
        private TopicAwareDispatcher _topicAwareDispatcher;

        public ScheduledTaskService(IDispatcher dispatcher,IServiceProvider serviceProvider)
        {
            _topicAwareDispatcher = new TopicAwareDispatcher(serviceProvider);
        }

        public async Task ExecuteAsync(Schedule schedule, IScheduler scheduler)
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
            if (ScheduleManager.ScheduleResourcesMap.TryGetValue(scheduleId, out var resourceMapping))
            { 
                _topicAwareDispatcher.Broadcast(
                    new ScheduleEventTrigger(scheduleId, type, resourceMapping.ResourceType)
                );
            }
        }

        public async Task UpdateAsync(Schedule schedule)
        {
        }

        public async Task DeleteAsync(Schedule schedule)
        {
        }
    }

}
