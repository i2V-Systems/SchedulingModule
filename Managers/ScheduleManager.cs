
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using CommonUtilityModule.CrudUtilities;
using CommonUtilityModule.Manager;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.EntityFrameworkCore;
using SchedulingModule.Models;
using SchedulingModule.services;
using Serilog;


namespace SchedulingModule.Managers
{
    public static class ScheduleManager
    {
        private static  IServiceProvider _serviceProvider;
        private static  IScheduler _scheduler;
        private static  IDispatcher _dispatcher;
        private static  IConfiguration _configuration;
        public static ConcurrentDictionary<Guid, Schedule> Schedules { get; private set; } = new ConcurrentDictionary<Guid, Schedule>();
        public static ConcurrentDictionary<Guid, ScheduleResourceMapping> scheduleResourcesMap { get; private set; } = new ConcurrentDictionary<Guid, ScheduleResourceMapping>();
        public static ConcurrentDictionary<Guid, SchedulAllDetails> scheduleWithAllDetailsDictionary { get; private set; } =
            new ConcurrentDictionary<Guid, SchedulAllDetails>();
        
        private static  ScheduledTaskService _scheduleTaskService;
        private static  SchedulerService _schedulerCRUDService;
        
        // initialise
        public static async Task  Init( 
             IConfiguration configuration,
             IScheduler scheduler,
             IDispatcher dispatcher,
             IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _dispatcher = dispatcher;
            _configuration = configuration;
            _scheduler = scheduler;
            // get all schedules in Memory
            _scheduleTaskService = ScheduleStartup.GetRequiredService<ScheduledTaskService>();
            
            ScheduleEventManager.Init(serviceProvider);
            _scheduleTaskService._dispatcher = dispatcher;
            _schedulerCRUDService = ScheduleStartup.GetRequiredService<SchedulerService>();
            var allSchedule = _schedulerCRUDService.GetAllSchedules();
            foreach (var item in allSchedule)
            {
                Schedules.TryAdd(item.Id, item);
            }
            await UpdateScheduleWithAllDetailsDictBySchedules(Schedules.Values);
            updateScheduleResourceMapping();

        }
       
        public static Schedule Get(Guid id)
        {
            Schedule value;
            Schedules.TryGetValue(id, out value);
            return value;
        }

        public static async Task<IEnumerable<SchedulAllDetails>> GetScheduleWithAllDetails(
            string userName
        )
        {
            try
            {
                if (scheduleWithAllDetailsDictionary.Count == 0)
                {
                    IEnumerable<Schedule> scheduleValues = Schedules.Values;
                    await UpdateScheduleWithAllDetailsDictBySchedules(scheduleValues);
                }
                if (scheduleWithAllDetailsDictionary.Count > 0)
                {
                    if (userName == "admin")
                    {
                        return scheduleWithAllDetailsDictionary.Values;
                    }
                    else
                    {
                    }
                }
                return ImmutableList<SchedulAllDetails>.Empty;
            }
            catch (Exception ex)
            {
                Log.Error("[ScheduleManager][getAllDetailsOfVideoSource] : " + ex.Message);
                return null;
            }
        }
        private static async Task UpdateScheduleWithAllDetailsDictBySchedules(
            IEnumerable<Schedule> schedulesValues
            )
        {
            foreach (var schedule in schedulesValues)
            {
                SchedulAllDetails scheduleWithDetails = new SchedulAllDetails();
                scheduleWithDetails.schedules = schedule;

                InsertAndUpdateValueInScheduleWithAllDetailsDictionary(scheduleWithDetails);
            }
        }

        private static void updateScheduleResourceMapping()
        {
            try
            {
                var resourceMapping =_schedulerCRUDService.GetAllResourceMapping();
                foreach (var item in resourceMapping)
                {
                    scheduleResourcesMap.TryAdd(item.Id, item);
                }
            }
            catch(Exception ex)
            {
                Log.Error("[ScheduleManager][getAllDetailsOfVideoSource] : " + ex.Message);
            }
        }
        public static void InsertAndUpdateValueInScheduleWithAllDetailsDictionary(
               SchedulAllDetails schedulAllDetails
           )
           {
               try
               {
                   scheduleWithAllDetailsDictionary[schedulAllDetails.schedules.Id] =
                       schedulAllDetails;
               }
               catch (Exception ex)
               {
                   Log.Error(
                       "[InsertAndUpdateValueInScheduleWithAllDetailsDictionary] : " + ex.Message
                   );
               }
           }
           
        public static void Add(Schedule source, string userid = null) {
                _schedulerCRUDService.Add(source);
                 AddScheduleInMemory(source);
                Schedules.TryAdd(source.Id, source);
                _scheduleTaskService.ExecuteAsync(source, _scheduler);

        }
        public static void AddScheduleInMemory(Schedule source)
        {
            Schedules.TryAdd(source.Id, source);
            SchedulAllDetails scheduleAllDetails = new SchedulAllDetails();
            scheduleAllDetails.schedules = source;
            InsertAndUpdateValueInScheduleWithAllDetailsDictionary(scheduleAllDetails);
        }

        public static void AddScheduleResourceMap(ScheduleResourceMapping map)
        {  
            _schedulerCRUDService.AddResourceMapping(map);
            scheduleResourcesMap.TryAdd(map.ScheduleId, map);
        }
        public static void UpdateInDbandMemory(Schedule source)
        {
          
            _schedulerCRUDService.Update(source);
            UpdateScheduleInMemory(source);
            //job related
            _scheduleTaskService.UpdateAsync(source);
        }
        
      
        public static async Task SendCrudDataToClient(
            CrudMethodType crudMethodType,
            Dictionary<string, dynamic> resources,
            List<string>? userClientIdsToBeNeglected = null,
            List<string>? userClientIdsToBeNotified = null
        )
        {
            await CrudManager.SendCrudDataToClient(
                CrudRelatedEntity.Schedule,
                crudMethodType,
                resources,
                userClientIdsToBeNeglected,
                userClientIdsToBeNotified
            );
        }

        
        public static bool Delete(Schedule source)
        {
            _schedulerCRUDService.Delete(source);
            Schedules.TryRemove(source.Id, out Schedule previousValue);
            _scheduleTaskService.DeleteAsync(source);
            ScheduleEventManager.scheduleEventService.UnscheduleJob(source, _scheduler);
            return true;
        }

        public static void DeleteMultipleFromDbAndInMemory(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                if (Schedules.ContainsKey(id))
                {
                    var schedule = Schedules[id];
                    _schedulerCRUDService.Delete(schedule);
                    RemoveScheduleFromMemory(schedule);
                    
                    //job related
                    _scheduleTaskService.DeleteAsync(schedule);
                    ScheduleEventManager.scheduleEventService.UnscheduleJob(schedule, _scheduler);
                   
                }
            }
        }

        public static void RemoveScheduleFromMemory(Schedule source)
        {

            Schedules.TryRemove(source.Id, out var removedSchedule);
            scheduleResourcesMap.TryRemove(source.Id, out var removedScheduleMap);
            scheduleWithAllDetailsDictionary.TryRemove(
                source.Id,
                out var removedScheduleWithAllDetails
            );
        }

        public static void UpdateScheduleInMemory(Schedule source)
        {
            Schedules.TryGetValue(source.Id, out Schedule previousValue);
            var isschedulesDictWithGuidUpdated = Schedules.TryUpdate(
                source.Id,
                source,
                previousValue
            );
            var scheduleUpdated = Schedules.TryGetValue(source.Id, out var newValue);
            SchedulAllDetails schedulAllDetails = new SchedulAllDetails();
            schedulAllDetails.schedules = source;
            schedulAllDetails.AttachedResources = scheduleWithAllDetailsDictionary[
                source.Id
            ].AttachedResources;
            InsertAndUpdateValueInScheduleWithAllDetailsDictionary(schedulAllDetails);
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