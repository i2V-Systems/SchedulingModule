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

        public void UnscheduleJob(Schedule schedule, IScheduler scheduler)
        {
            try
            { 
                var iScheduler= scheduler as Scheduler;
                // Unschedule all related jobs
                iScheduler.TryUnschedule($"{schedule.Id}_start");
                iScheduler.TryUnschedule($"{schedule.Id}_end");
                iScheduler.TryUnschedule($"{schedule.Id}_weekday_start");
                iScheduler.TryUnschedule($"{schedule.Id}_weekday_end");
                iScheduler.TryUnschedule($"{schedule.Id}_weekend_start");
                iScheduler.TryUnschedule($"{schedule.Id}_weekend_end");
                iScheduler.TryUnschedule($"{schedule.Id}_date_start");
                iScheduler.TryUnschedule($"{schedule.Id}_date_end");
                
                
                Console.WriteLine($"Successfully unscheduled all jobs for schedule {schedule.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unscheduling jobs for schedule {schedule.Id}: {ex.Message}");
            }
        }

        private void ExecuteStartEvent(Action<Guid,ScheduleEventType> taskToPerform, Schedule schedule)
        {
            try
            {
                // Log start event
                Console.WriteLine($"Schedule {schedule.Id} started at {DateTime.Now}");
        
                // Execute the task with start event indicator
                // You could modify your Action to accept additional parameters for event type
                taskToPerform(schedule.Id, ScheduleEventType.Start);
        
                // Additional start logic here if needed
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in start event for schedule {schedule.Id}: {ex.Message}");
            }
        }
        private void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler)
        {
            try
            {
                // Log end event
                Console.WriteLine($"Schedule {schedule.Id} ended at {DateTime.Now}");
        
                // Execute the task with end event indicator
                taskToPerform(schedule.Id, ScheduleEventType.End);
        
                // Additional end logic here if needed
        
                // Optional: Unschedule if this was a one-time schedule
                if (schedule.Type == Enum_ScheduleType.DateWise)
                {
                    UnscheduleJob(schedule, scheduler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in end event for schedule {schedule.Id}: {ex.Message}");
            }
        }
        
        
        
        public Task ScheduleJob(Action<Guid,ScheduleEventType> taskToPerform,Schedule schedule,IScheduler scheduler)
        {
            //place here functionto perform and add logic to end and execute frame in it.
            var scheduleStartDateTime = schedule.StartDateTime;
            var scheduleEndDateTime = schedule.EndDateTime;
            switch (schedule.Type)
            {
                case Enum_ScheduleType.Daily:
                {
                    if (schedule.SubType == Enum_ScheduleSubType.Every)
                    {
                        
                    }
                    else
                    {
                        // Schedule start event
                        scheduler.Schedule(() =>
                            {
                                ExecuteStartEvent(taskToPerform, schedule);
                            })
                            .DailyAt(scheduleStartDateTime.Hour, scheduleStartDateTime.Minute)
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_start");
                        // Schedule end event
                        scheduler.Schedule(() =>
                            {
                                ExecuteEndEvent(taskToPerform, schedule, scheduler);
                            })
                            .DailyAt(scheduleEndDateTime.Hour, scheduleEndDateTime.Minute)
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_end");
                    }
                    break;
                }
                case Enum_ScheduleType.Weekly:
                {
                    if (schedule.SubType == Enum_ScheduleSubType.Selecteddays)
                    {
                        var startcronnjob = CronExpressionBuilder.BuildCronExpression(schedule.StartDays,scheduleStartDateTime);
                        var endcronnjob = CronExpressionBuilder.BuildCronExpression(schedule.StartDays,scheduleEndDateTime);
                        // Schedule start event
                        scheduler.Schedule(() =>
                            {
                                ExecuteStartEvent(taskToPerform, schedule);
                            })
                            .Cron(startcronnjob)
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_end");
                        // Schedule end event
                        scheduler.Schedule(() =>
                            {
                                ExecuteEndEvent(taskToPerform, schedule, scheduler);
                            })
                            .Cron(endcronnjob)
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_end");
                    }
                    else if (schedule.SubType == Enum_ScheduleSubType.Weekdays)
                    {
                        // Schedule start event for weekdays
                        scheduler.Schedule(() =>
                            {
                                ExecuteStartEvent(taskToPerform, schedule);
                            })
                            .DailyAt(scheduleStartDateTime.Hour, scheduleStartDateTime.Minute)
                            .Weekday()
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_weekday_start");

                        // Schedule end event for weekdays
                        scheduler.Schedule(() =>
                            {
                                ExecuteEndEvent(taskToPerform, schedule, scheduler);
                            })
                            .DailyAt(scheduleEndDateTime.Hour, scheduleEndDateTime.Minute)
                            .Weekday()
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_weekday_end");
                    }
                    else
                    {
                        // Schedule start event for weekends
                        scheduler.Schedule(() =>
                            {
                                ExecuteStartEvent(taskToPerform, schedule);
                            })
                            .DailyAt(scheduleStartDateTime.Hour, scheduleStartDateTime.Minute)
                            .Weekend()
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_weekend_start");

                        // Schedule end event for weekends
                        scheduler.Schedule(() =>
                            {
                                ExecuteEndEvent(taskToPerform, schedule, scheduler);
                            })
                            .DailyAt(scheduleEndDateTime.Hour, scheduleEndDateTime.Minute)
                            .Weekend()
                            .Zoned(TimeZoneInfo.Local)
                            .PreventOverlapping($"{schedule.Id}_weekend_end");
                    }
                    break;
                }
                case Enum_ScheduleType.DateWise:
                {
                    // Schedule start event on specific date
                    scheduler.Schedule(() =>
                        {
                            ExecuteStartEvent(taskToPerform, schedule);
                        })
                        .DailyAt(scheduleStartDateTime.Hour,scheduleStartDateTime.Minute)
                        .Zoned(TimeZoneInfo.Local)
                        .PreventOverlapping($"{schedule.Id}_date_start")
                        .When(() => 
                        {
                            var date = DateTime.Now.Date;
                            return Task.FromResult(scheduleStartDateTime.Date == date );
                        });

                    // Schedule end event on specific date
                    scheduler.Schedule(() =>
                        {
                            ExecuteEndEvent(taskToPerform, schedule, scheduler);
                        })
                        .DailyAt(scheduleEndDateTime.Hour,scheduleEndDateTime.Minute)
                        .Zoned(TimeZoneInfo.Local)
                        .PreventOverlapping($"{schedule.Id}_date_end")
                        .When(() => 
                        {
                            var date = DateTime.Now.Date;
                            return Task.FromResult(scheduleEndDateTime.Date == date );
                        });
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
