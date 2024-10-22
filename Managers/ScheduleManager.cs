
using System.Collections.Concurrent;
using System.Text;
using SchedulingModule.Models;
using SchedulingModule.services;
using Serilog;


namespace SchedulingModule.Managers
{
    public static class ScheduleManager
    {

        public static ConcurrentDictionary<Guid, Schedules> Schedules { get; private set; } = new ConcurrentDictionary<Guid, Schedules>();

        public static ConcurrentDictionary<string, bool> ScheduleDict { get; set; } = new ConcurrentDictionary<string, bool>();

        //private static HangFireScheduler hangFireSchedulerService { get; set; }

        // initialise
        public static void Init()
        {
            // get all schedules in Memory
            var schedulerService = ScheduleStartup.GetRequiredService<SchedulerService>();
            var allSchedule = schedulerService.GetAllSchedules();
            foreach (var item in allSchedule)
            {
                Schedules.TryAdd(item.Id, item);
            }

            // get All Jobs In Memory
            //var TaskschedulerService = ScheduleStartup.GetRequiredService<TaskScheduleService>();
            //var allJob = TaskschedulerService.GetAllSchedulesJob();
            //foreach (var job in allJob)
            //{
            //    ScheduleDict[job.JobId] = true;
            //}

            ////get hangFireScheduler Service from Startup
            //hangFireSchedulerService = ScheduleStartup.GetRequiredService<HangFireScheduler>();

        }

        public static Schedules Get(Guid id)
        {
            Schedules value;
            Schedules.TryGetValue(id, out value);
            return value;
        }

        public static void Add(Schedules source)
        {
            var schedulerService = ScheduleStartup.GetRequiredService<SchedulerService>();
            schedulerService.Add(source);
            Schedules.TryAdd(source.Id, source);
        }

        public static void Update(Schedules source)
        {
            var schedulerService = ScheduleStartup.GetRequiredService<SchedulerService>();
            schedulerService.Update(source);
            Schedules previousValue;
            Schedules.TryGetValue(source.Id, out previousValue);
            Schedules.TryUpdate(source.Id, source, previousValue);

        }

        //public static List<ScheduledTask> getByIdwithTasks(int id)
        //{
        //    var TaskschedulerService = ScheduleStartup.GetRequiredService<TaskScheduleService>();
        //    return TaskschedulerService.getByIdWithAttachedTasks(id);
        //}

        public static bool Delete(Schedules source)
        {
            var schedulerService = ScheduleStartup.GetRequiredService<SchedulerService>();
            // get all video source from videosource manager
            //var videosources = VideoSourceManager.VideoSources;
            // get attach video source configuration



            //foreach (var videoSource in videosources)
            //{
            //    var videoSourceConfig = VideoSourceManager.GetVideoSourceWithConfigsAsync(videoSource.Value.Id).Result.VideoSourceConfig;
            //    // iterate over all video source configuration
            //    if (videoSourceConfig != null)
            //    {
            //        foreach (var config in videoSourceConfig)
            //        {
            //            // if any configuration have this schedule return schdule is attached with configuration
            //            if (config.TaskId == source.Id)
            //            {
            //                return false;
            //            }

            //        }
            //    }
            //}
            schedulerService.Delete(source);
            Schedules previousValue;
            Schedules.TryRemove(source.Id, out previousValue);
            return true;
        }


        // Update EnqueJob
        //public static void EnqueueJob(int ConfigurationdId, VideoSource videoSource)
        //{
        //    // Create Job Id and update in schedule dictionary
        //    CreateJobIdAndUpdateInScheduleDict(ConfigurationdId, videoSource);
        //}

        // create job Id
        //public static void CreateJobIdAndUpdateInScheduleDict(int ConfigurationdId, VideoSource videoSource)
        //{
        //    Schedules schedules = GetCurrentSchedule(ConfigurationdId, videoSource);
        //    // if  configuration not attached with schedule
        //    // if configuration attached with schedule Added ist time
        //    if (schedules != null)
        //    {
        //        string jobId = $"{ConfigurationdId}_{schedules.Id}_{schedules.Type}";

        //        // check this configuration have another schedule or not if have update with new one
        //        CheckConfigurationIdHaveAnotherScheduleOrNotIfExistRemove(ConfigurationdId);
        //        UpdateScheduleDictionary(ConfigurationdId, jobId, schedules, videoSource);

        //    }
        //}

        // get CurrentSchedule
        //private static Schedules? GetCurrentSchedule(int ConfigurationdId, VideoSource videoSource)
        //{

        //    foreach (var config in videoSource.VideoSourceConfig)
        //    {
        //        try
        //        {
        //            Schedules schedules;
        //            if (config.Id == ConfigurationdId)
        //            {
        //                if (config.TaskId != null)
        //                {
        //                    schedules = Get((int)config.TaskId);
        //                    return schedules;
        //                }
        //                else
        //                {
        //                    CheckConfigurationIdHaveAnotherScheduleOrNotIfExistRemove(ConfigurationdId); // remove schedule if selected none from hang fire

        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }

        //    }
        //    return null;
        //}



        // update in schedule dictionary
        private static void UpdateScheduleDictionary(int ConfigurationdId, string JobId, Schedules schedules)
            //VideoSource videoSource)
        {
            // add in dict
            ScheduleDict.TryAdd(JobId, true);
            // enqeue in dict jobs
            //AddScheduleInHangFire(ConfigurationdId, JobId, schedules, videoSource);
            //// add in database
            //AddOrRemove("Add", JobId, schedules, ConfigurationdId);
        }

        // check configuration have another schedule or not if have remove
        //public static void CheckConfigurationIdHaveAnotherScheduleOrNotIfExistRemove(int ConfigurationId)
        //{

        //    foreach (var key in ScheduleDict.Keys)
        //    {
        //        int prevConfigId = Convert.ToInt16(key.Split('_')[0]);
        //        if (prevConfigId == ConfigurationId)
        //        {
        //            bool removingValue;
        //            ScheduleDict.TryRemove(key, out removingValue);
        //            RemoveScheduleFromHangFire(key);

        //        }
        //    }
        //}

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


        // add in database or remove in database
        //private static void AddOrRemove(string OperationType, string jobId, Schedules schedules = null, int configurationId = 0)
        //{
        //    var taskScheduleService = ScheduleStartup.GetRequiredService<TaskScheduleService>();
        //    if (OperationType == "Add")
        //    {
        //        taskScheduleService.Add(jobId, schedules, configurationId);
        //    }
        //    else
        //    {
        //        taskScheduleService.DeleteWithJobId(jobId);
        //    }
        //}


    }

}