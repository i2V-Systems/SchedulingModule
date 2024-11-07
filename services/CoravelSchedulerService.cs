using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using SchedulingModule.Models;
using Serilog;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;
using static SchedulingModule.ScheduleTypeEnum;

namespace SchedulingModule.services
{
    [SingletonService]
    public class CoravelSchedulerService
    {
        private static IScheduler _scheduler;

        public CoravelSchedulerService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void UnScheduleJob(Schedules schedule)
        {
            try
            {
                (_scheduler as Scheduler).TryUnschedule(schedule.Id.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Error in Unscheduling Job ::"+ex);
            }
            
        }

        public void LogicAndUnscheduleJob(Action taskToPerform, Schedules schedule)
        {
            Console.WriteLine("Job Invoked");

            var currentTime = DateTime.Now;
                if (currentTime >= schedule.EndDateTime)
                {
                    //event throw that job close 
                    //id here of this running  schedule

                    UnScheduleJob(schedule);

                }
                else
                {
                    taskToPerform();
                }

            
        }
        public Task ScheduleJob(Action taskToPerform,Schedules schedule,TimeSpan executeTime)
        {
            //place here functionto perform and add logic to end and execute frame in it.
            var hours = executeTime.Hours;
            var minutes = executeTime.Minutes;
            switch (schedule.Type)
            {
                case Enum_ScheduleType.Daily:
                {
                    _scheduler.Schedule(
                            () =>
                            {
                                //LogicAndUnscheduleJob(taskToPerform, schedule);
                                Console.WriteLine("job invoked.");
                            }

                        )
                        .EveryMinute()
                        .PreventOverlapping(schedule.Id.ToString()); 
                            //.Zoned(TimeZoneInfo.Local);
                        break;
                }
                case Enum_ScheduleType.Weekly:
                {
                   
                    if (schedule.SubType == Enum_ScheduleSubType.Weekdays)
                    {
                        _scheduler.Schedule(
                                () =>
                                {
                                    LogicAndUnscheduleJob(taskToPerform, schedule);
                                }
                                )
                                .DailyAt(hours, minutes)
                                .Weekday()
                                .Zoned(TimeZoneInfo.Local);
                    }
                    else
                    {
                        _scheduler.Schedule(
                                () =>
                                {
                                    LogicAndUnscheduleJob(taskToPerform, schedule);
                                }
                                )
                                .DailyAt(hours, minutes)
                                .Weekend()
                                .Zoned(TimeZoneInfo.Local);
                    }
                    break;
                }
                case Enum_ScheduleType.DateWise:
                {
                    _scheduler.Schedule(
                            () =>
                            {
                                LogicAndUnscheduleJob(taskToPerform, schedule);
                            }
                            )
                            .DailyAt(hours,minutes)
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
