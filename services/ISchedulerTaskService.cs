using System;
using System.Collections.Generic;
using Coravel.Scheduling.Schedule.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using SchedulingModule.Models;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.services;
public interface  ISchedulerTaskService
{ 
    public  Task InitService();
    public  void UnscheduleJob(Schedule schedule, IScheduler scheduler); 
    public   void ExecuteStartEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule);
    public  void ExecuteEndEvent(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule,IScheduler scheduler);
    public  Task ScheduleJob(Action<Guid, ScheduleEventType> taskToPerform, Schedule schedule, IScheduler scheduler);

}
