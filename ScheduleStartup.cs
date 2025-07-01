using System.Reflection;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SchedulingModule.Application.Context;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Application.Managers;
using SchedulingModule.Infrastructure.Repositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule
{
    public static class ScheduleStartup
    {
        public static IScheduler Scheduler;
        public static IConfiguration configuration;
        public static IServiceCollection AddSchedlingModuleServices(this IServiceCollection services,IConfiguration _configuration)
        {
            configuration = _configuration;
            var scheduleAssembly = Assembly.Load("SchedulingModule");
            services.AddMvc().AddApplicationPart(scheduleAssembly).AddControllersAsServices();
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            
            
            //coravel services
            services.AddScheduler();
            services.AddEvents();
            services.AddServicesOfType<IScopedService>();
            services.AddServicesWithAttributeOfType<ScopedServiceAttribute>();
            services.AddServicesOfType<ITransientService>();
            services.AddServicesWithAttributeOfType<TransientServiceAttribute>();
            services.AddServicesOfType<ISingletonService>();
            services.AddServicesWithAttributeOfType<SingletonServiceAttribute>();
            
            services.AddDbContext<ScheduleDbContext>(
                options =>
                {
                    options
                        .UseNpgsql(configuration.GetConnectionString("analytic"),
                            b =>
                            {
                                b.MigrationsAssembly("DataLayer");
                            }
                        )
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .EnableSensitiveDataLogging();
                },
                ServiceLifetime.Transient
            );
           
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IScheduleManager, ScheduleManager>();
            
            
            return services;

        }

     
        private static IServiceProvider ServiceProvider;
        public static void Start(IServiceProvider serviceProvider, IApplicationBuilder app,IDispatcher dispatcher)
        {
            ServiceProvider = serviceProvider;
            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                Scheduler = scheduler;
            });
            var scheduleManager= new ScheduleManager(configuration, Scheduler,dispatcher,serviceProvider);
             scheduleManager.InitializeAsync();
        }

        public static T GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        public static IServiceScope CreateScope<T>(out T service)
        {
            var scope = ServiceProvider.CreateScope();
            service = scope.ServiceProvider.GetRequiredService<T>();
            return scope;
        }

        public static void Dispose()
        {
        }
    }
}
