using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;

namespace SchedulingModule.services
{

    public class ScheduledTaskService
    {
        private Schedules _schedule;
        private static IScheduler _scheduler;

        public ScheduledTaskService(
           IScheduler scheduler
        )
        {
            _scheduler = scheduler;
        }

        public async Task ExecuteAsync(Schedules schedule)
        {
            _schedule = schedule;
            var currentTime = DateTime.Now;

            if (currentTime >= _schedule.StartDateTime && currentTime <= _schedule.EndDateTime)
            {
                // Execute the main task since it is within the scheduled range
                var coravelScheduler = ScheduleStartup.GetRequiredService<CoravelSchedulerService>();
                if (schedule.RecurringStartTime == schedule.RecurringEndTime)
                {
                    await coravelScheduler.ScheduleJob(HandleScheduledJob, schedule,schedule.RecurringStartTime);
                }
                else
                {
                    await coravelScheduler.ScheduleJob(HandleScheduledJob, schedule, schedule.RecurringStartTime);
                    await coravelScheduler.ScheduleJob(HandleScheduledJob, schedule, schedule.RecurringEndTime);

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
    


