
using SchedulingModule.services;

namespace SchedulingModule.Managers;

public static class ScheduleEventManager
{
    public static ISchedulerTaskService scheduleEventService;
    internal static async Task Init(IServiceProvider serviceProvider)
    {
        var configuration = ScheduleStartup.configuration;
        if (configuration.GetValue<bool>("UseCoravel"))
        {
            scheduleEventService = new CoravelSchedulerService(serviceProvider);
            await ((CoravelSchedulerService)scheduleEventService).InitService();
        }
        else
        { 
            scheduleEventService = new HangFireSchedulerService();
            await ((HangFireSchedulerService)scheduleEventService).InitService();
        }
    }
}