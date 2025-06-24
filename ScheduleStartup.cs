using System.Reflection;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.EntityFrameworkCore;
using SchedulingModule.Application.Managers;
using SchedulingModule.Domain.Context;
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
            //coravel services
            services.AddScheduler();
            services.AddEvents();
            services.AddServicesOfType<IScopedService>();
            services.AddServicesWithAttributeOfType<ScopedServiceAttribute>();
            services.AddServicesOfType<ITransientService>();
            services.AddServicesWithAttributeOfType<TransientServiceAttribute>();
            services.AddServicesOfType<ISingletonService>();
            services.AddServicesWithAttributeOfType<SingletonServiceAttribute>();
            
            services.AddDbContext<SchedulingModuleDbContext>(
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
            ScheduleManager.Init(configuration, Scheduler,dispatcher,serviceProvider);
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
