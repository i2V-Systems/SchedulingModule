


//using LoggingModule.Context;

using System.Reflection;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.EntityFrameworkCore;
using SchedulingModule.Context;
using SchedulingModule.Managers;
using SchedulingModule.Models;
using SchedulingModule.services;
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

            //services.AddTransient<VideoSourceScheduleHandler>();
            //services.AddTransient<IListener<ScheduledReccuringEventTrigger>, VideoSourceScheduleHandler>();


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

        // wiil be initialized through main app startup
        // will need configuration to be passed, plus db context
        public static void Start(
            IServiceProvider serviceProvider, IApplicationBuilder app)
        {
            ServiceProvider = serviceProvider;
            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                // You can add global schedules here if needed

                Scheduler = scheduler;
                

            });

            //IEventRegistration registration = provider.ConfigureEvents();
            //registration
            //    .Register<ScheduledReccuringEventTrigger>()
            //    .Subscribe<VideoSourceScheduleHandler>();


            ScheduleManager.InIt(configuration, Scheduler);


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
            //_context.Dispose();
        }
    }
}
