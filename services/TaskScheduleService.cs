using System;
using System.Collections.Generic;

using MassTransit;
using Microsoft.AspNetCore.Http;
using SchedulingModule.Models;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SchedulingModule.services
{
    //[TransientService]
    //internal class TaskScheduleService
    //{
    //    private IEntityBaseRepository<ScheduledTaskService> scheduledTaskRepository;
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public TaskScheduleService(AnalyticDbContext analyticDbContext , IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //         var httpContext = _httpContextAccessor.HttpContext;
    //         if (httpContext != null && httpContext.Request.Headers.TryGetValue("Userid", out var userId))
    //         {
    //             scheduledTaskRepository = new EntityBaseRepository<ScheduledTaskService>(analyticDbContext, new Guid(userId));
    //         }
    //         else
    //         {
    //             scheduledTaskRepository = new EntityBaseRepository<ScheduledTaskService>(analyticDbContext, new Guid());
    //         }
              
           
    //    }

    //    public ScheduledTaskService Get(Guid id)
    //    {
    //        return scheduledTaskRepository.Get(id);
    //    }

    //    public List<ScheduledTaskService> GetAllSchedulesJob()
    //    {
    //        return scheduledTaskRepository.GetAll();
    //    }

    //    public void Add(string jobId, Schedules schedules, int configurationId)
    //    {
    //        // create data according to requirements
    //        var scheduledTask = createScheduleTaskObject(jobId, schedules, configurationId);
    //        scheduledTaskRepository.Add(scheduledTask);
    //    }

    //    public void DeleteWithJobId(string jobId)
    //    {
    //        scheduledTaskRepository.DeleteWhere(x => x.JobId == jobId);
    //    }

    //    public void Update(ScheduledTaskService entity, string userName = "")
    //    {
           
    //        scheduledTaskRepository.Update(entity);
    //        scheduledTaskRepository.DetachEntity(entity);
    //    }

    //    public List<ScheduledTaskService> getByIdWithAttachedTasks(Guid ScheduleId)
    //    {
    //        return scheduledTaskRepository.FindAll((x) => x.ScheduleId == ScheduleId);
    //    }

    //    private ScheduledTaskService createScheduleTaskObject(
    //        string jobId,
    //        Schedules schedules,
    //        int configurationId
    //    )
    //    {
    //        ScheduledTaskService scheduledTask = new ScheduledTaskService();
    //        scheduledTask.ScheduleId = schedules.Id;
    //        scheduledTask.StartApiEndPoint = "/";
    //        scheduledTask.StopApiEndPoint = "/";
    //        scheduledTask.JobId = jobId;
    //        scheduledTask.DataId = Convert.ToString(configurationId);
    //        return scheduledTask;
    //    }
    //}
}
