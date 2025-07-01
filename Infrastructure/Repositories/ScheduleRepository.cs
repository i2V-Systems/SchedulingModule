using Microsoft.EntityFrameworkCore;
using SchedulingModule.Application.Context;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Domain.Entities;
using SchedulingModule.Domain.Enums;

namespace SchedulingModule.Infrastructure.Repositories;

public class ScheduleRepository :IScheduleRepository
{
    private readonly ScheduleDbContext _context;
    private IScheduleRepository _scheduleRepositoryImplementation;

    public ScheduleRepository(ScheduleDbContext context)
    {
        _context = context;
    }
    public async Task<Schedule?> GetByIdAsync(Guid id)
        => await _context.Schedules.FindAsync(id);

    public async Task<IEnumerable<Schedule>> GetAllAsync()
        => await _context.Schedules.ToListAsync();

    public async Task<IEnumerable<Schedule>> SearchAsync(string searchTerm)
        => await _context.Schedules.Where(b => b.Name.Contains(searchTerm) || b.Details.Contains(searchTerm)).ToListAsync();

    public async Task<IEnumerable<Schedule>> GetByStatusAsync(ScheduleStatus status)
        => await _context.Schedules.Where(b => b.Status == status).ToListAsync();

    public async Task<Schedule> AddAsync(Schedule book)
    {
        _context.Schedules.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Schedule> UpdateAsync(Schedule book)
    {
        _context.Schedules.Update(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _context.Schedules.FindAsync(id);
        if (book != null)
        {
            _context.Schedules.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await _context.Schedules.AnyAsync(b => b.Id == id);

  
}

