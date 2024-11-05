
using Microsoft.Extensions.DependencyInjection;
using SchedulingModule.Managers;
//using LoggingModule.Context;

namespace SchedulingModule
{
    public class ScheduleStartup
    {
        public static void configureServices(string connectionString)
        {
            //services.AddScheduler();

        }

     
        private static IServiceProvider ServiceProvider;

        // wiil be initialized through main app startup
        // will need configuration to be passed, plus db context
        public static void Start(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ScheduleManager.Init();


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
