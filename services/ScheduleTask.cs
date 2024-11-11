using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.services
{
    [SingletonService]
    public class ScheduledTaskService
    {
        private Schedules _schedule;
        //public static IScheduler _scheduler;
        private static CoravelSchedulerService _coravelService;
        private readonly  IDispatcher _dispatcher;


        public ScheduledTaskService(
            IDispatcher dispatcher
          
        )
        {
            _dispatcher = dispatcher;
            _dispatcher.Broadcast(new ScheduledReccuringEventTrigger());

            _coravelService = ScheduleStartup.GetRequiredService<CoravelSchedulerService>();
          
        }

        public async Task ExecuteAsync(Schedules schedule,IScheduler scheduler)
        {
            _schedule = schedule;
            //this._dispatcher = dispatcher;

            var currentTime = DateTime.Now;

            if (currentTime >= _schedule.StartDateTime && currentTime <= _schedule.EndDateTime)
            {
                // Execute the main task since it is within the scheduled range
               
                if (schedule.RecurringStartTime == schedule.RecurringEndTime)
                {
                    await _coravelService.ScheduleJob(HandleScheduledJob, schedule,schedule.RecurringStartTime,scheduler);
                }
                else
                {
                    await _coravelService.ScheduleJob(HandleScheduledJob, schedule, schedule.RecurringStartTime, scheduler);
                    await _coravelService.ScheduleJob(HandleScheduledJob, schedule, schedule.RecurringEndTime, scheduler);

                }
               
            }
            else if (currentTime > _schedule.EndDateTime)
            {
                // Stop scheduling this task if it's beyond the end date
                //remove job
                Console.WriteLine("Task execution skipped as it is outside the allowed schedule range.");
            }
        }

        public void HandleScheduledJob()
        {
            //logic here and event callback for start and end,removejob, events 
            //throw event 
             _dispatcher.Broadcast(new ScheduledReccuringEventTrigger());




        }
        public async Task UpdateAsync(Schedules schedule)
        {
            //update the scheduled job

        }
        public async Task DeleteAsync(Schedules schedule)
        {
            //update the scheduled job

        }

        

        // Remove Schedule Form Hangfire
        //private static void RemoveScheduleFromHangFire(string key)
        //{
        //    hangFireSchedulerService.RemoveScheduleFromJobId(key);
        //    AddOrRemove("remove", key);
        //}

        //// Add Schedule In HangFire
        //private static void AddScheduleInHangFire(int ConfigurationdId, string JobId, Schedules schedules, VideoSource videoSource)
        //{
        //    // add update analytics
        //    hangFireSchedulerService.ScheduleAnalytics(JobId, schedules.Id, Convert.ToString(ConfigurationdId), schedules.StartCronExp, schedules.StopCronExp, videoSource);

        //}
    }
}
    


