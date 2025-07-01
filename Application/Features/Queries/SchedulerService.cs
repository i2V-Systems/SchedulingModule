using SchedulingModule.Application.Context;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Extensions;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Exceptions;
using SchedulingModule.Infrastructure.Repositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.Application.Services
{
    
    public interface ISchedulerService
    {
        // DTO methods for manager
        Task<ScheduleDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ScheduleDto>> GetAllAsync();
        Task<Guid> AddAsync(ScheduleDto dto, string userName = "");
        Task UpdateAsync(ScheduleDto dto, string userName = "");
        Task DeleteAsync(Guid id, string userName = "");
        
        // Resource mapping methods
        Task AddResourceMappingAsync(ScheduleResourceDto mapping);
        Task<IEnumerable<ScheduleResourceDto>> GetAllResourceMappingAsync();
        Task DeleteResourceMappingAsync(Guid mappingId);
      
    }
    

    public class SchedulerService : ISchedulerService
    {
        private readonly IScheduleRepository _schedulesRepository;
        private readonly IScheduleResourceRepository _resourceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SchedulerService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public SchedulerService(
            ScheduleDbContext scheduleDbContext, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,ILogger<SchedulerService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _schedulesRepository = new ScheduleRepository(scheduleDbContext);
            _resourceRepository =
                    new ScheduleResourceRepository(scheduleDbContext);
        }

        public async Task<ScheduleDto> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _schedulesRepository.GetByIdAsync(id);
                return entity?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedule {ScheduleId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            try
            {
                var entities = await _schedulesRepository.GetAllAsync();
                return entities.Select(e => e.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all schedules");
                throw;
            }
        }

        public async Task<Guid> AddAsync(ScheduleDto  dto, string userName = "")
        {
            try
            {
                _logger.LogInformation("Creating schedule {ScheduleName} by {UserName}", dto.Name, userName);

                // Convert DTO to domain entity
                var entity = dto.ToDomain();
                
                await _schedulesRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Schedule created with ID {ScheduleId}", entity.Id);
                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule {ScheduleName}", dto.Name);
                throw;
            }
        }
        
        public async Task DeleteAsync(Guid entityId, string userName = "")
        {
               try
            {
                _logger.LogInformation("Deleting schedule {ScheduleId} by {UserName}", entityId, userName);

                var entity = await _schedulesRepository.GetByIdAsync(entityId);
                if (entity == null)
                    throw new NotFoundException($"Schedule with ID {entityId} not found");

                await _schedulesRepository.DeleteAsync(entityId);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Schedule {ScheduleId} deleted successfully", entityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule {ScheduleId}", entityId);
                throw;
            }
        }

        public async Task UpdateAsync(ScheduleDto dto, string userName = "")
        {
            try
            {
                _logger.LogInformation("Updating schedule {ScheduleId} by {UserName}", dto.Id, userName);
                var existingEntity = await _schedulesRepository.GetByIdAsync(dto.Id);
                if (existingEntity == null)
                    throw new NotFoundException($"Schedule with ID {dto.Id} not found");

                // Update domain entity from DTO
                existingEntity.UpdateFromDto(dto);
                await _schedulesRepository.UpdateAsync(existingEntity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Schedule {ScheduleId} updated successfully", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule {ScheduleId}", dto.Id);
                throw;
            }
        }
        
        public  async Task AddResourceMappingAsync(ScheduleResourceDto mapping)
        {
            try
            {
                var entity = mapping.ToDomain();
                await _resourceRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding resource mapping");
                throw;
            }
        }
        public async Task<IEnumerable<ScheduleResourceDto>> GetAllResourceMappingAsync()
        {
            try
            {
                var entities = await _resourceRepository.GetAllAsync();
                return entities.Select(e => e.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resource mappings");
                throw;
            }
        }
        public async Task DeleteResourceMappingAsync(Guid mappingId)
        {
            try
            {
                var entity = await _resourceRepository.GetByIdAsync(mappingId);
                if (entity != null)
                {
                    await _resourceRepository.DeleteAsync(entity.Id);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource mapping {MappingId}", mappingId);
                throw;
            }
        }
    }
    
    
}
