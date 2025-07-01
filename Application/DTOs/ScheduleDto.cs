using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.DTOs;

public record ScheduleDto(Guid Id, string Name, string Details, DateTime StartDateTime, DateTime EndDateTime,
    ScheduleType Type, ScheduleSubType? SubType, int? NoOfDays, List<Days> StartDays, ScheduleStatus Status, DateTime? RecurringTime);

public record ScheduleSearchDto (string? SearchTerm, ScheduleStatus? Status );