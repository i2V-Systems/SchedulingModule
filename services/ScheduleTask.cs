using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Managers;
using SchedulingModule.Models;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.services
{
    [ScopedService]
    public class ScheduledTaskService
    {
        private Schedule _schedule;
        public  IDispatcher _dispatcher;
        public ScheduledTaskService(IDispatcher dispatcher) {}
        public async Task ExecuteAsync(Schedule schedule,IScheduler scheduler)
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

        public void HandleScheduledJob(Guid scheduleId,ScheduleEventType type)
        {
            _dispatcher.Broadcast(new ScheduledReccuringEventTrigger(scheduleId));
        }
        public async Task UpdateAsync(Schedule schedule) {}
        public async Task DeleteAsync(Schedule schedule) {}
    }
}
    


