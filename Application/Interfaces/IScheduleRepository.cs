using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Application.Interfaces;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<IEnumerable<Schedule>> SearchAsync(string searchTerm);
    Task<IEnumerable<Schedule>> GetByStatusAsync(ScheduleStatus status);
    Task<Schedule> AddAsync(Schedule Schedule);
    Task<Schedule> UpdateAsync(Schedule Schedule);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}