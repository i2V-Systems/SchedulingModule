using System.Collections.Concurrent;
using System.Collections.Immutable;
using CommonUtilityModule.CrudUtilities;
using CommonUtilityModule.Manager;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Scheduler;
using SchedulingModule.Application.ScheduleStrategies;
using SchedulingModule.Application.Services;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Presentation.Models;
using Serilog;

namespace SchedulingModule.Application.Managers
{
    public class ScheduleManager :IScheduleManager
    {
        private readonly  IServiceProvider _serviceProvider;
        private readonly  IUnifiedScheduler _scheduler;
        private readonly  IDispatcher _dispatcher;
        private readonly  IConfiguration _configuration;
        
        private readonly  ScheduledTaskService _taskService;
        private readonly  SchedulerService _crudService;
        
        public  ConcurrentDictionary<Guid, ScheduleDto> Schedules { get; } = new();
        public  ConcurrentDictionary<Guid, ScheduleResourceDto> ScheduleResourcesMap { get; } = new();
        public  ConcurrentDictionary<Guid, SchedulAllDetails> ScheduleDetailsMap { get; } = new();
        
        public ScheduleManager(IConfiguration configuration,
            IUnifiedScheduler scheduler,
            IDispatcher dispatcher,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            
            _taskService = ScheduleStartup.GetRequiredService<ScheduledTaskService>();
            _crudService = ScheduleStartup.GetRequiredService<SchedulerService>();
            
            InitializeAsync();
        }
        
        // Query methods implementation
        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByIdsAsync(IEnumerable<Guid> ids)
        {
            return ids.Where(id => Schedules.ContainsKey(id))
                .Select(id => Schedules[id])
                .ToList();
        }

        public async Task<ScheduleDto> GetScheduleFromCacheAsync(Guid id)
        {
            return Schedules.TryGetValue(id, out var schedule) ? schedule : null;
        }

        public IEnumerable<ScheduleResourceDto> GetResourcesByScheduleId(Guid scheduleId)
        {
            return ScheduleResourcesMap.Values
                .Where(r => r.ScheduleId == scheduleId)
                .ToList();
        }

        public async Task<SchedulAllDetails> GetScheduleDetailsFromCacheAsync(Guid id)
        {
            return ScheduleDetailsMap.TryGetValue(id, out var details) ? details : null;
        }

        
        // Cache status methods
        public bool IsScheduleLoaded(Guid scheduleId)
        {
            return Schedules.ContainsKey(scheduleId);
        }

        public bool IsResourceLoaded(Guid scheduleId)
        {
            return ScheduleResourcesMap.ContainsKey(scheduleId);
        }

        public int GetLoadedScheduleCount()
        {
            return Schedules.Count;
        }

        public int GetLoadedResourceCount()
        {
            return ScheduleResourcesMap.Count;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllCachedSchedulesAsync()
        {
            return Schedules.Values.ToList();
        }

        public async Task<IEnumerable<ScheduleResourceDto>> GetAllCachedResourcesAsync()
        {
            return ScheduleResourcesMap.Values.ToList();
        }

        public async Task RefreshCacheAsync()
        {
            // Clear existing cache
            Schedules.Clear();
            ScheduleResourcesMap.Clear();
            ScheduleDetailsMap.Clear();

            // Reload from database
            await InitializeAsync();
        }
        
        public async Task InitializeAsync()
        {
            ScheduleEventManager.Init(_serviceProvider);//TODO 
            var allSchedules = await _crudService.GetAllAsync();
            foreach (var schedule in  allSchedules)
            {
                Schedules.TryAdd(schedule.Id, schedule);
            }
            await UpdateScheduleDetailsAsync(Schedules.Values);
            await LoadScheduleResourceMapping();
            executeLoadedTasks();
        }
        
        public  async Task<ScheduleDto> GetAsync(Guid id) =>
            Schedules.TryGetValue(id, out var schedule) ? schedule : null;

        public async Task<SchedulAllDetails> GetDetailedAsync(Guid id)
        {
           return  ScheduleDetailsMap.TryGetValue(id, out var schedule) ? schedule : null;
        }
      
        public async Task<Guid> CreateScheduleAsync(ScheduleDto schedule, string userId = null) {
            var id= await _crudService.AddAsync(schedule);
            AddToMemory(id,schedule);
            await  _taskService.ExecuteAsync(schedule, _scheduler);
            return id;
        }
        
        public async Task UpdateScheduleAsync(ScheduleDto schedule)
        {
            await _crudService.UpdateAsync(schedule);
            UpdateInMemory(schedule);
            _taskService.UpdateAsync(schedule);
        }
        public async Task<bool> DeleteScheduleAsync(Guid id)
        {
            await  _crudService.DeleteAsync(id);
            RemoveFromMemory(id);
            _taskService.DeleteAsync(id);
            ScheduleEventManager.scheduleEventService.UnscheduleJob(id, _scheduler);
            return true;
        }
        
        public  async Task<IEnumerable<SchedulAllDetails>> GetScheduleWithAllDetailsAsync(
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

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            try
            {
                if (ScheduleDetailsMap.IsEmpty)
                {
                    await UpdateScheduleDetailsAsync(Schedules.Values);
                }
                return Schedules.Values;
            }
            catch (Exception ex)
            {
                Log.Error("[ScheduleManager][GetScheduleWithAllDetails] : {Message}", ex.Message);
                return null;
            }
        }
        
        public async Task DeleteMultipleSchedulesAsync(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                if (Schedules.TryGetValue(id, out var schedule))
                {
                    await  _crudService.DeleteAsync(id);
                    RemoveFromMemory(id);
                    _taskService.DeleteAsync(id);
                    ScheduleEventManager.scheduleEventService.UnscheduleJob(id, _scheduler);
                }
            }
        }

        public async Task SendCrudDataToClientAsync(
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

        
        
        private  async Task UpdateScheduleDetailsAsync(IEnumerable<ScheduleDto> schedules)
        {
            foreach (var schedule in schedules)
            {
                var details = new SchedulAllDetails { schedules = schedule };
                AddOrUpdateScheduleDetails(details);
            }
            await Task.CompletedTask;
        }

        private async Task LoadScheduleResourceMapping()
        {
            try
            {
                var allDetails =await  _crudService.GetAllResourceMappingAsync();
                foreach (var map in allDetails)
                {
                    ScheduleResourcesMap.TryAdd(map.Id, map);
                }
            }
            catch(Exception ex)
            {
                Log.Error("[ScheduleManager][LoadScheduleResourceMapping] : {Message}", ex.Message);
            }
        }
        public  void AddOrUpdateScheduleDetails(SchedulAllDetails details)
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
           
        
        //memory functions 
        public  void UpdateInMemory(ScheduleDto schedule)
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
        public  void RemoveFromMemory(Guid id)
        {
            Schedules.TryRemove(id, out _);
            ScheduleResourcesMap.TryRemove(id, out _);
            ScheduleDetailsMap.TryRemove(id, out _);
        }

      
        public  void AddToMemory(Guid id,ScheduleDto schedule)
        {
            Schedules.TryAdd(id, schedule);
            AddOrUpdateScheduleDetails(new SchedulAllDetails { schedules = schedule });
        }

        public async Task AddScheduleResourceMap(ScheduleResourceDto map)
        {  
            await _crudService.AddResourceMappingAsync(map);
            ScheduleResourcesMap.TryAdd(map.ScheduleId, map);
        }
        
        private  void executeLoadedTasks()
        {
            foreach (var item in Schedules)
            {
                _taskService.ExecuteAsync(item.Value, _scheduler);
            }
        }
    }
}