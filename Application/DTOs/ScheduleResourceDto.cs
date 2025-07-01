using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.DTOs;

public record ScheduleResourceDto(Guid Id, Guid ScheduleId, Guid ResourceId, Resources ResourceType);
