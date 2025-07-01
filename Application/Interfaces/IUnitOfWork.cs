namespace SchedulingModule.Application.Interfaces;


public interface IUnitOfWork : IDisposable
{
    IScheduleRepository Schedules { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}