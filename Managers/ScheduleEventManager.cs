
using System.Reflection;
using SchedulingModule.services;

namespace SchedulingModule.Managers;

public static class ScheduleEventManager
{
    public static ISchedulerTaskService scheduleEventService;
    internal static void Init(IServiceProvider serviceProvider)
    {
        var configuration = ScheduleStartup.configuration;
        string serviceName= configuration.GetValue<string>("SchedulingService");
        
        
        Type pType = typeof(ISchedulerTaskService);
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        Type schedulingService = types
            .Where(t => t.IsClass && t != pType && pType.IsAssignableFrom(t)).First(res => res.Name.ToLower().Contains(serviceName));
        
        if(schedulingService != null)
        {
            scheduleEventService = CreateInstance(schedulingService, serviceProvider);
        }
        else
        {
            // Fallback to direct instantiation
            scheduleEventService = serviceProvider.GetRequiredService<CoravelSchedulerService>();
        }
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