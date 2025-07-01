
using System.Reflection;
using SchedulingModule.Application.Services;

namespace SchedulingModule.Application.Managers;

public  class ScheduleEventManager
{
    public static ISchedulerTaskService scheduleEventService { get; private set; }
    internal static void Init(IServiceProvider serviceProvider)
    {
        var configuration = ScheduleStartup.configuration;
        string serviceName= configuration.GetValue<string>("SchedulingService") ??  throw new InvalidOperationException("SchedulingService configuration value is missing or empty.");;
        
        scheduleEventService = ResolveService(serviceProvider, serviceName)
                               ?? throw new InvalidOperationException("Unable to resolve the scheduler task service.");
    }
 
    private static ISchedulerTaskService ResolveService(IServiceProvider serviceProvider, string serviceName)
    {
        var targetType = typeof(ISchedulerTaskService);
        var matchingType = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t =>
                t.IsClass &&
                !t.IsAbstract &&
                targetType.IsAssignableFrom(t) &&
                t.Name.ToLower().Contains(serviceName));

        if (matchingType != null)
        {
            return CreateInstance(matchingType, serviceProvider);
        }
        // fallback to default implementation
        return serviceProvider.GetService<CoravelSchedulerService>();
    }
    
    private static ISchedulerTaskService CreateInstance(Type serviceType, IServiceProvider serviceProvider)
    {
        try
        {
            // get from DI container 
            var serviceFromDi = serviceProvider.GetRequiredService(serviceType);
            if (serviceFromDi != null)
            {
                return (ISchedulerTaskService)serviceFromDi;
            }
            return serviceProvider.GetRequiredService<CoravelSchedulerService>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create instance of {serviceType.Name}: {ex.Message}", ex);
        }
    }
    
}