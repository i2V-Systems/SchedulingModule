using Riok.Mapperly.Abstractions;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Domain.Entities;

namespace SchedulingModule.Application.Extensions;

[Mapper]
public static partial class ScheduleMapper
{
    #region Schedule Mappings

    public static partial ScheduleDto ToDto(this Schedule entity);
   

    public static partial Schedule ToDomain(this ScheduleDto dto);

    public static void UpdateFromDto(this Schedule entity, ScheduleDto dto)
    {
        // Call the UpdateDetails method on the existing entity
        entity.UpdateDetails(
            dto.Name,
            dto.Type,
            dto.SubType,
            dto.Details,
            dto.NoOfDays,
            dto.StartDays , 
            dto.StartDateTime,
            dto.EndDateTime,
            dto.RecurringTime
        );
        // Update status if provided
        if (dto.Status != null)
        {
            entity.UpdateStatus(dto.Status);
        }
    }

    #endregion

    #region Schedule Resource Mappings

    public static ScheduleResourceDto ToDto(this ScheduleResourceMapping entity)
    {
        if (entity == null) return null;
        return new ScheduleResourceDto(entity.Id, entity.ScheduleId, entity.ResourceId, entity.ResourceType);
    }

    public static ScheduleResourceMapping ToDomain(this ScheduleResourceDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        // Create a new ScheduleResource entity
        var scheduleResource = new ScheduleResourceMapping
        {
            ScheduleId = dto.ScheduleId,
            ResourceId = dto.ResourceId,
            ResourceType = dto.ResourceType,
        };
        return scheduleResource;
    }

    public static void UpdateFromDto(this ScheduleResourceMapping entity, ScheduleResourceDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        // Update resource mapping properties
        if (dto.ResourceId != Guid.Empty && dto.ResourceId != entity.ResourceId)
        {
            entity.ResourceId = dto.ResourceId;
        }
        
        if (dto.ResourceType>0)
        {
            entity.ResourceType = dto.ResourceType;
        }
    }

    #endregion

    #region Batch Operations

    public static IEnumerable<ScheduleDto> ToDto(this IEnumerable<Schedule> entities)
    {
        return entities?.Select(e => e.ToDto()) ?? Enumerable.Empty<ScheduleDto>();
    }

    public static IEnumerable<ScheduleResourceDto> ToDto(this IEnumerable<ScheduleResourceMapping> entities)
    {
        return entities?.Select(e => e.ToDto()) ?? Enumerable.Empty<ScheduleResourceDto>();
    }

    public static IEnumerable<Schedule> ToDomain(this IEnumerable<ScheduleDto> dtos)
    {
        return dtos?.Select(dto => dto.ToDomain()) ?? Enumerable.Empty<Schedule>();
    }

    public static IEnumerable<ScheduleResourceMapping> ToDomain(this IEnumerable<ScheduleResourceDto> dtos)
    {
        return dtos?.Select(dto => dto.ToDomain()) ?? Enumerable.Empty<ScheduleResourceMapping>();
    }

    #endregion
}