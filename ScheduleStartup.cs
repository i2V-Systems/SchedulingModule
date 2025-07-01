using System.Reflection;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SchedulingModule.Application.Context;
using SchedulingModule.Application.Interfaces;
using SchedulingModule.Application.Managers;
using SchedulingModule.Application.ScheduleStrategies;
using SchedulingModule.Infrastructure.Repositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule
{
    public enum SchedulerType
    {
        Coravel,
        Hangfire
    }

    public static class ScheduleStartup
    {
        public static IConfiguration configuration;
        public static SchedulerType CurrentSchedulerType { get; private set; }
        public static IServiceCollection AddSchedlingModuleServices(this IServiceCollection services,IConfiguration _configuration)
        {
            configuration = _configuration;
            var scheduleAssembly = Assembly.Load("SchedulingModule");
            services.AddMvc().AddApplicationPart(scheduleAssembly).AddControllersAsServices();
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            
            ConfigureScheduler(services,configuration);
            
            //coravel services
         
          
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
            switch (CurrentSchedulerType)
            {
                case SchedulerType.Coravel:
                    StartCoravel(serviceProvider, app, dispatcher);
                    break;
                    
                case SchedulerType.Hangfire:
                    StartHangfire(serviceProvider, app);
                    break;
            }
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
        
        private static void StartCoravel(
            IServiceProvider serviceProvider, 
            IApplicationBuilder app, 
            IDispatcher? dispatcher)
        {
            var provider = app.ApplicationServices;
            IScheduler Scheduler;
            provider.UseScheduler(scheduler =>
            {
                IUnifiedScheduler UnifiedScheduler = new CoravelUnifiedScheduler(scheduler);
                var scheduleManager= new ScheduleManager(configuration, UnifiedScheduler, dispatcher,serviceProvider);
            });
        }

        private static void StartHangfire( 
            IServiceProvider serviceProvider, 
            IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
        
        private static void ConfigureScheduler(
            IServiceCollection services,
            IConfiguration configuration)
        {
            string serviceName = configuration.GetValue<string>("SchedulingService") ??
                                 throw new InvalidOperationException(
                                     "SchedulingService configuration value is missing or empty.");
            switch (serviceName)
            {
                case "coravel":
                    ConfigureCoravel(services);
                    break;
                case "hangfire":
                    ConfigureHangfire(services, configuration);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported scheduler type: {serviceName}");
            }
        }

        private static void ConfigureCoravel(IServiceCollection services)
        {
            services.AddScheduler();
            services.AddEvents();
            services.AddSingleton<IUnifiedScheduler, CoravelUnifiedScheduler>();
        }

        private static void ConfigureHangfire(IServiceCollection services,IConfiguration configuration)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_110)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
               );
            services.AddHangfireServer();
        }
    
    }
    
}
