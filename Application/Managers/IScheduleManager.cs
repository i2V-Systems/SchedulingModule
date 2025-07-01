using CommonUtilityModule.CrudUtilities;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Presentation.Models;

namespace SchedulingModule.Application.Managers;

public interface IScheduleManager
{
        // Initialization and lifecycle
        Task InitializeAsync();
        
        // Query methods for dictionary access
        Task<IEnumerable<ScheduleDto>> GetSchedulesByIdsAsync(IEnumerable<Guid> ids);
        Task<ScheduleDto> GetScheduleFromCacheAsync(Guid id);
        IEnumerable<ScheduleResourceDto> GetResourcesByScheduleId(Guid scheduleId);
        Task<SchedulAllDetails> GetScheduleDetailsFromCacheAsync(Guid id);
        
        // Cache status methods
        bool IsScheduleLoaded(Guid scheduleId);
        bool IsResourceLoaded(Guid scheduleId);
        int GetLoadedScheduleCount();
        int GetLoadedResourceCount();
        
        // Bulk cache operations
        Task<IEnumerable<ScheduleDto>> GetAllCachedSchedulesAsync();
        Task<IEnumerable<ScheduleResourceDto>> GetAllCachedResourcesAsync();
        Task RefreshCacheAsync();
        
        // Core CRUD operations
        Task<ScheduleDto> GetAsync(Guid id);
        Task<SchedulAllDetails> GetDetailedAsync(Guid id);
        Task<Guid> CreateScheduleAsync(ScheduleDto dto, string userId = null);
        Task UpdateScheduleAsync(ScheduleDto dto);
        Task<bool> DeleteScheduleAsync(Guid id);
    
        // Complex queries
        Task<IEnumerable<SchedulAllDetails>> GetScheduleWithAllDetailsAsync(string userName);
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        // Resource mapping operations
        Task AddScheduleResourceMap(ScheduleResourceDto map); // Fixed type
    
        // Memory management operations
        void AddToMemory(Guid id, ScheduleDto schedule);
        void UpdateInMemory(ScheduleDto schedule);
        void RemoveFromMemory(Guid id);
        void AddOrUpdateScheduleDetails(SchedulAllDetails details);
    
        // Bulk operations
        Task DeleteMultipleSchedulesAsync(IEnumerable<Guid> ids);
    
        // Cross-cutting concerns
        Task SendCrudDataToClientAsync(CrudMethodType method, Dictionary<string, dynamic> resources, List<string> skipUserIds = null, List<string> targetUserIds = null);
}
