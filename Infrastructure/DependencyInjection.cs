using Microsoft.EntityFrameworkCore;
using SchedulingModule.Application.Context;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Application.Managers;
using SchedulingModule.Application.Services;
using SchedulingModule.Infrastructure.Data.Seeder;
using SchedulingModule.Infrastructure.Repositories;

namespace SchedulingModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Use MySQL
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Port=3306;Database=BookStoreDb;User=root;Password=password;";

        services.AddDbContext<ScheduleDbContext>(options =>
            options.UseNpgsql(connectionString,
                mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(5);
                }));
        
        services.AddScoped<ISchedulerService, SchedulerService>();
        // services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
        
        services.AddSingleton<IScheduleManager, ScheduleManager>();
        
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register the database seeder service
        services.AddScoped<IDatabaseSeederService, DatabaseSeederService>();

        return services;
    }
} 