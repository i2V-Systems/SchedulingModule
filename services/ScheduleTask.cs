using SchedulingModule.Models;

namespace SchedulingModule.services
{

    public class ScheduledTask
    {
        private Schedules _schedule;



        public async Task ExecuteAsync(Schedules schedule)
        {
            _schedule = schedule;
            var currentTime = DateTime.UtcNow;

            if (currentTime >= _schedule.StartDateTime && currentTime <= _schedule.EndDateTime)
            {
                // Execute the main task since it is within the scheduled range
                var coravelScheduler = ScheduleStartup.GetRequiredService<CoravelScheduler>();
                await coravelScheduler.PerformJobAction(schedule);
            }
            else if (currentTime > _schedule.EndDateTime)
            {
                // Stop scheduling this task if it's beyond the end date
                //remove job
                Console.WriteLine("Task execution skipped as it is outside the allowed schedule range.");
            }
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
    


