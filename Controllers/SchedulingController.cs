
using CommonUtilityModule.CrudUtilities;
using Microsoft.AspNetCore.Mvc;
using Coravel.Events.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SchedulingModule.Managers;
using SchedulingModule.Models;
using SchedulingModule.services;
using Serilog;

namespace SchedulingModule.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    //[LicenceValid]
    public  class SchedulingController : Controller
    {
        
        private readonly ILogger<SchedulingController> _logger;
        public SchedulingController(ILogger<SchedulingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Schedule> GetAll()
        {
            
             
            return ScheduleManager.Schedules.Values;
        }
       
        [HttpGet("~/api/Schedules/GetAllResourceDetails")]
        public async Task<IActionResult> GetAllResourceDetails()
        { 
            try
            {
                StringValues UserName;
                HttpContext.Request.Headers.TryGetValue("Username", out UserName);
                return Ok(await ScheduleManager.GetScheduleWithAllDetails(UserName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return BadRequest(ex.Message.ToString());
            }
        }
        
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!ScheduleManager.Schedules.ContainsKey(id))
                {
                    return NotFound();
                }
                return Ok(ScheduleManager.Get(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Schedule schedule)
        {
            try
            {
                HttpContext.Request.Headers.TryGetValue("clientId", out  StringValues clientId);
                HttpContext.Request.Headers.TryGetValue("userid", out StringValues userid);
                
                ScheduleManager.Add(schedule,userid);
                
                SchedulAllDetails schedulesource =
                    ScheduleManager.scheduleWithAllDetailsDictionary[schedule.Id];
                var objectToSend = new Dictionary<string, dynamic>()
                {
                    {
                        "scheduleAllDetailsList",
                        new List<SchedulAllDetails>() { schedulesource }
                    }
                };
                await ScheduleManager.SendCrudDataToClient(
                    CrudMethodType.Add,
                    objectToSend
                );
               
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            return Ok(schedule);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != schedule.Id)
            {
                return BadRequest();
            }

            try
            {
                ScheduleManager.Update(schedule);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ex.Message);
                //if (!VideoSourceManager.VideoSources.ContainsKey(id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                    throw;
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }

            return Ok(schedule);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!ScheduleManager.Schedules.ContainsKey(id))
            {
                return NotFound();
            }
            try
            {
                var schedule = ScheduleManager.Schedules[id];
                ScheduleManager.Delete(schedule);
                return Ok(schedule);
               
                //return Json(new { status = "error", message = "Schedule Deleted Failed its Attached to a Configuration" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ex.Message);
                //if (!VideoSourceManager.VideoSources.ContainsKey(id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                throw;
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            //return Ok(schedule);

        }

    }
}
