using Coravel.Scheduling.Schedule;
using SchedulingModule.Models;
using static SchedulingModule.ScheduleTypeEnum;

namespace SchedulingModule.services
{
    public class CoravelScheduler
    {

        public Task ScheduleJob(Schedules schedule)
        {
            //place here functionto perform and add logic to end and execute frame in it
            switch (schedule.Type)
            {
                case Enum_ScheduleType.Daily:
                {
                        scheduler.Schedule(
                                () => Console.WriteLine("Scheduled task.")
                        )
                            .DailyAt(2)
                            .Zoned(TimeZoneInfo.Local);
                        break;
                }
                case Enum_ScheduleType.Weekly:
                {
                    if (schedule.SubType == "1")
                    {
                        scheduler.Schedule(
                                () => Console.WriteLine("Scheduled task.")
                            )
                            .Weekday()
                            .Zoned(TimeZoneInfo.Local);
                        }
                    else
                    {
                        scheduler.Schedule(
                                () => Console.WriteLine("Scheduled task.")
                            )
                            .Weekend()
                            .Zoned(TimeZoneInfo.Local);
                        }
                    break;
                }
                case Enum_ScheduleType.DateWise:
                {
                    scheduler.Schedule(
                            () => Console.WriteLine("Scheduled task.")
                        )
                        .DailyAt(2)
                        .Zoned(TimeZoneInfo.Local);
                    break;
                }
                case Enum_ScheduleType.Custom:
                {
                 
                    break;
                }


            }

          
         



            // Your main job logic here
            Console.WriteLine("Executing scheduled task within the allowed time range.");
            return Task.CompletedTask;
        }
    }
}
