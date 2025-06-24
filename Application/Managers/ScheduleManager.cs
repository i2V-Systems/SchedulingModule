using System.Collections.Concurrent;
using System.Collections.Immutable;
using CommonUtilityModule.CrudUtilities;
using CommonUtilityModule.Manager;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.Services;
using SchedulingModule.Domain.Models;
using SchedulingModule.Presentation.Models;
using Serilog;

namespace SchedulingModule.Application.Managers
{
    public static class ScheduleManager
    {
        private static  IServiceProvider _serviceProvider;
        private static  IScheduler _scheduler;
        private static  IDispatcher _dispatcher;
        private static  IConfiguration _configuration;
        
        private static  ScheduledTaskService _taskService;
        private static  SchedulerService _crudService;
        
        public static ConcurrentDictionary<Guid, Schedule> Schedules { get; } = new();
        public static ConcurrentDictionary<Guid, ScheduleResourceMapping> ScheduleResourcesMap { get; } = new();
        public static ConcurrentDictionary<Guid, SchedulAllDetails> ScheduleDetailsMap { get; } = new();
        
        public static async Task  Init( 
             IConfiguration configuration,
             IScheduler scheduler,
             IDispatcher dispatcher,
             IServiceProvider serviceProvider
        )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            
            _taskService = ScheduleStartup.GetRequiredService<ScheduledTaskService>();
            _crudService = ScheduleStartup.GetRequiredService<SchedulerService>();
            
            _taskService._dispatcher = _dispatcher;
            
            ScheduleEventManager.Init(_serviceProvider);
            
            foreach (var schedule in _crudService.GetAllSchedules())
            {
                Schedules.TryAdd(schedule.Id, schedule);
            }

            await UpdateScheduleDetailsAsync(Schedules.Values);
            LoadScheduleResourceMapping();

        }
       
        public static Schedule Get(Guid id) =>
            Schedules.TryGetValue(id, out var schedule) ? schedule : null;

        public static async Task<IEnumerable<SchedulAllDetails>> GetScheduleWithAllDetailsAsync(
            string userName
        )
        {
            try
            {
                if (ScheduleDetailsMap.IsEmpty)
                {
                    await UpdateScheduleDetailsAsync(Schedules.Values);
                }
                return userName == "admin"
                    ? ScheduleDetailsMap.Values
                    : ImmutableList<SchedulAllDetails>.Empty;
            }
            catch (Exception ex)
            {
                Log.Error("[ScheduleManager][GetScheduleWithAllDetails] : {Message}", ex.Message);
                return null;
            }
        }
        private static async Task UpdateScheduleDetailsAsync(IEnumerable<Schedule> schedules)
        {
            foreach (var schedule in schedules)
            {
                var details = new SchedulAllDetails { schedules = schedule };
                AddOrUpdateScheduleDetails(details);
            }
            await Task.CompletedTask;
        }

        private static void LoadScheduleResourceMapping()
        {
            try
            {
                foreach (var map in _crudService.GetAllResourceMapping())
                {
                    ScheduleResourcesMap.TryAdd(map.Id, map);
                }
            }
            catch(Exception ex)
            {
                Log.Error("[ScheduleManager][LoadScheduleResourceMapping] : {Message}", ex.Message);
            }
        }
        public static void AddOrUpdateScheduleDetails(SchedulAllDetails details)
        {
            try
            {
                ScheduleDetailsMap[details.schedules.Id] = details;
            }
            catch (Exception ex)
            {
                Log.Error("[ScheduleManager][AddOrUpdateScheduleDetails] : {Message}", ex.Message);
            }
        }
           
        public static void Add(Schedule schedule, string userId = null) {
            _crudService.Add(schedule);
            AddToMemory(schedule);
            _taskService.ExecuteAsync(schedule, _scheduler);

        }
        
        public static void AddToMemory(Schedule schedule)
        {
            Schedules.TryAdd(schedule.Id, schedule);
            AddOrUpdateScheduleDetails(new SchedulAllDetails { schedules = schedule });
        }

        public static void AddScheduleResourceMap(ScheduleResourceMapping map)
        {  
            _crudService.AddResourceMapping(map);
            ScheduleResourcesMap.TryAdd(map.ScheduleId, map);
        }
        
        public static void UpdateInDbandMemory(Schedule schedule)
        {
          
            _crudService.Update(schedule);
            UpdateInMemory(schedule);
            _taskService.UpdateAsync(schedule);
        }
        
        public static bool Delete(Schedule schedule)
        {
            _crudService.Delete(schedule);
            RemoveFromMemory(schedule);
            _taskService.DeleteAsync(schedule);
            ScheduleEventManager.scheduleEventService.UnscheduleJob(schedule, _scheduler);
            return true;
        }
        
        public static void DeleteMultiple(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                if (Schedules.TryGetValue(id, out var schedule))
                {
                    _crudService.Delete(schedule);
                    RemoveFromMemory(schedule);
                    _taskService.DeleteAsync(schedule);
                    ScheduleEventManager.scheduleEventService.UnscheduleJob(schedule, _scheduler);
                }
            }
        }
        
        public static void RemoveFromMemory(Schedule schedule)
        {
            Schedules.TryRemove(schedule.Id, out _);
            ScheduleResourcesMap.TryRemove(schedule.Id, out _);
            ScheduleDetailsMap.TryRemove(schedule.Id, out _);
        }
        
        public static void UpdateInMemory(Schedule schedule)
        {
            if (Schedules.TryGetValue(schedule.Id, out var existing))
            {
                Schedules.TryUpdate(schedule.Id, schedule, existing);
                var updatedDetails = new SchedulAllDetails
                {
                    schedules = schedule,
                    AttachedResources = ScheduleDetailsMap.ContainsKey(schedule.Id)
                        ? ScheduleDetailsMap[schedule.Id].AttachedResources
                        : null
                };
                AddOrUpdateScheduleDetails(updatedDetails);
            }
        }
        
        public static async Task SendCrudDataToClientAsync(
            CrudMethodType method,
            Dictionary<string, dynamic> resources,
            List<string>? skipUserIds  = null,
            List<string>? targetUserIds  = null
        )
        {
            await CrudManager.SendCrudDataToClient(
                CrudRelatedEntity.Schedule,
                method,
                resources,
                skipUserIds,
                targetUserIds
            );
        }
    }
}