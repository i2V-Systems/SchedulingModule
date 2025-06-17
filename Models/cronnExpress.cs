using SchedulingModule.Interfaces;
using static SchedulingModule.ScheduleTypeEnum;

namespace SchedulingModule.Models
{

    public class CronExpressionBuilder
    {
        private static readonly Dictionary<string, int> DayToCronMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Sunday", 0 }, { "Sun", 0 },
            { "Monday", 1 }, { "Mon", 1 },
            { "Tuesday", 2 }, { "Tue", 2 }, { "Tues", 2 },
            { "Wednesday", 3 }, { "Wed", 3 },
            { "Thursday", 4 }, { "Thu", 4 }, { "Thur", 4 }, { "Thurs", 4 },
            { "Friday", 5 }, { "Fri", 5 },
            { "Saturday", 6 }, { "Sat", 6 }
        };
        
        public static string BuildCronExpression(List<string> selectedDays, DateTime timeString)
        {
            // Convert days to cron format
            var cronDays = ConvertDaysToCron(selectedDays);
        
            // Build cron expression: minute hour day month dayOfWeek
            return $"{timeString.Minute} {timeString.Hour} * * {cronDays}";
        }
        
        private static string ConvertDaysToCron(List<string>selectedDays)
        {
            if (selectedDays == null || selectedDays.Count == 0)
            {
                throw new ArgumentException("No days selected");
            }
            var cronDayNumbers = new List<int>();
            foreach (var day in selectedDays)
            {
                if (DayToCronMap.TryGetValue(day.Trim(), out var cronDay))
                {
                    cronDayNumbers.Add(cronDay);
                }
                else
                {
                    throw new ArgumentException($"Invalid day name: {day}");
                }
            }
            // Sort and remove duplicates
            cronDayNumbers = cronDayNumbers.Distinct().OrderBy(x => x).ToList();
            // Join with commas for multiple days
            return string.Join(",", cronDayNumbers);
        }
    }
}
    