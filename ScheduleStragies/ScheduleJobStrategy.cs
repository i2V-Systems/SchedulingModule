using Serilog;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.ScheduleStragies;

[SingletonService]
public class ScheduleJobStrategy
{
        private readonly Dictionary<string, IScheduleJobStrategy> _instanceCache;
        private readonly Dictionary<string, Type> _strategyCache;
        private readonly IServiceProvider _serviceProvider;

        ScheduleJobStrategy(IServiceProvider serviceProvider)
        {
                _serviceProvider = serviceProvider;
                _instanceCache = new Dictionary<string, IScheduleJobStrategy>();
                _strategyCache = new Dictionary<string, Type>();
        }
        public IScheduleJobStrategy GetStrategy(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
        {
                var key = CreateCacheKey(scheduleType);
                // First try exact match from instance cache
                if (_instanceCache.TryGetValue(key, out var cachedInstance))
                { 
                        return cachedInstance;
                }
                // Try to create from type cache
                if (_strategyCache.TryGetValue(key, out var strategyType))
                {
                        var instance = CreateStrategyInstance(strategyType);
                        if (instance != null)
                        { 
                                _instanceCache[key] = instance;
                                return instance;
                        }
                }
                // Try fallback - look for strategies that can handle this type
                var fallbackStrategy = _instanceCache.Values.Where(s => s.CanHandle(scheduleType)).FirstOrDefault();
                if (fallbackStrategy != null)
                {
                       Log.Error($"Using fallback strategy {fallbackStrategy.GetType().Name} for {key}");
                       return fallbackStrategy;
                }
                throw new NotSupportedException($"No strategy found for schedule type {scheduleType}. " +
                                                $"Available strategies: {string.Join(", ", _instanceCache.Keys)}");
        }
        private IScheduleJobStrategy CreateStrategyInstance(Type strategyType)
        {
                try
                { 
                        // Try to get from DI container first
                        var instance = _serviceProvider.GetService(strategyType) as IScheduleJobStrategy;
                        if (instance != null)
                        {
                                return instance;
                        }
                        // Fallback to Activator
                        return Activator.CreateInstance(strategyType) as IScheduleJobStrategy;
                }
                catch (Exception ex)
                {
                        Log.Error(ex, $"Failed to create instance of strategy {strategyType.Name}");
                        return null;
                }
        }
        private static string CreateCacheKey(ScheduleTypeEnum.Enum_ScheduleType scheduleType)
        {
                return  scheduleType.ToString();
        }
}
