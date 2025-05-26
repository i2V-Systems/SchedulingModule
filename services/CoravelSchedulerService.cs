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
       

        public CoravelSchedulerService()
        {
            //_scheduler = scheduler;
        }

        public void UnScheduleJob(Schedule schedule,IScheduler scheduler)
        {
            try
            {
                (scheduler as Scheduler).TryUnschedule(schedule.Id.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Error in Unscheduling Job ::"+ex);
            }
            
        }

        public void LogicAndUnscheduleJob(Action<Guid> taskToPerform, Schedule schedule,IScheduler scheduler)
        {
            Console.WriteLine("Job Invoked");

            var currentTime = DateTime.Now;
                if (currentTime >= schedule.EndDateTime)
                {
                    //event throw that job close 
                    //id here of this running  schedule


                    UnScheduleJob(schedule,scheduler);

                }
                else
                {
                    taskToPerform( schedule.Id);
                }

            
        }
        public Task ScheduleJob(Action<Guid> taskToPerform,Schedule schedule,TimeSpan executeTime,IScheduler scheduler)
        {
            //place here functionto perform and add logic to end and execute frame in it.
            var hours = executeTime.Hours;
            var minutes = executeTime.Minutes;
            switch (schedule.Type)
            {
                case Enum_ScheduleType.Daily:
                {
                    scheduler.Schedule(
                            () =>
                            {
                                LogicAndUnscheduleJob(taskToPerform, schedule, scheduler);
                            }

                        )
                        .EveryMinute();
                        //.Zoned(TimeZoneInfo.Local)
                        //.PreventOverlapping(schedule.Id.ToString());
                        break;
                }
                case Enum_ScheduleType.Weekly:
                {
                   
                    if (schedule.SubType == Enum_ScheduleSubType.Weekdays)
                    {
                        scheduler.Schedule(
                                () =>
                                {
                                    LogicAndUnscheduleJob(taskToPerform, schedule, scheduler);
                                }
                                )
                                .DailyAt(hours, minutes)
                                .Weekday()
                                .Zoned(TimeZoneInfo.Local)
                                .PreventOverlapping(schedule.Id.ToString());
                    }
                    else
                    {
                        scheduler.Schedule(
                                () =>
                                {
                                    LogicAndUnscheduleJob(taskToPerform, schedule, scheduler);
                                }
                                )
                                .DailyAt(hours, minutes)
                                .Weekend()
                                .Zoned(TimeZoneInfo.Local)
                                .PreventOverlapping(schedule.Id.ToString());
                    }
                    break;
                }
                case Enum_ScheduleType.DateWise:
                {
                    scheduler.Schedule(
                            () =>
                            {
                                LogicAndUnscheduleJob(taskToPerform, schedule,scheduler);
                            }
                            )
                            .DailyAt(hours,minutes)
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping(schedule.Id.ToString());
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
