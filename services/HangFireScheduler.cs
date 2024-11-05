using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;


namespace SchedulingModule.services
{
    [SingletonService]
    internal class HangFireScheduler
    {
        /* public void ScheduleAnalytics(string JobId, int ScheduleId, string VideoConfigurationId, string startCronExpression, string stopCronExpression, VideoSource videoSource)
         {
             // Schedule the start analytics task to run according to the specified cron expression
             RecurringJob.AddOrUpdate(JobId + "_start", () => StartAnalyticsAsync(JobId, ScheduleId, VideoConfigurationId,videoSource), startCronExpression, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

             // Schedule the stop analytics task to run according to the specified cron expression
             RecurringJob.AddOrUpdate(JobId + "_stop", () => StopAnalyticsAsync(JobId, ScheduleId, VideoConfigurationId,videoSource), stopCronExpression, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
         }



         // Remove Schedule Analytics
         public void RemoveScheduleFromJobId(string JobId)
         {

             // remove start Schedule
             RecurringJob.RemoveIfExists(JobId + "_start");

             // remove end Schedule

             RecurringJob.RemoveIfExists(JobId + "_stop");
         }



         public async Task StartAnalyticsAsync(string JobId, int ScheduleId, string VideoConfigurationId, VideoSource videoSource)
         {
             UpdateScheduleAnalytics(JobId, ScheduleId, VideoConfigurationId, videoSource,"addAnalytics");

         }


         public async Task StopAnalyticsAsync(string JobId, int ScheduleId, string VideoConfigurationId, VideoSource videoSource)
         {
             UpdateScheduleAnalytics(JobId, ScheduleId, VideoConfigurationId, videoSource,"removeAnalytics");
         }

         // Remove Schedule Analytics or Add Schedule Analtics

         public void UpdateScheduleAnalytics(string JobId, int ScheduleId, string VideoConfigurationId, VideoSource videoSource,string operationType)
         {
             Dictionary<int, List<Enum_AnalyticType>> analyticsPerDevice = new Dictionary<int, List<Enum_AnalyticType>>();
             var server =AnalyticServerManager.GetVideoSourceServerMappingByDeviceId(videoSource.Id);
             List<VideoSource> videoSources = new List<VideoSource>
                     {
                         videoSource
                     };
             if (server != null)
             {
                 var serverBM = (AnalyticServerCpuBM)AnalyticServerManager.GetServerBM(server.Id);
                 // getting analytics from particular scheule only
                 analyticsPerDevice = serverBM.GetCamerasDictThatAreConfiguredForAnalytics(videoSources, true, Convert.ToInt32(VideoConfigurationId));
                 if(operationType=="addAnalytics")
                 {
                     serverBM.ApplyAnalytics(analyticsPerDevice, videoSources, Convert.ToInt32(VideoConfigurationId));
                 }
                 else
                 {
                     serverBM.RemoveAnalytics(analyticsPerDevice);
                 }
             }

         }*/
    }
}
