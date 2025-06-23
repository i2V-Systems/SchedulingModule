
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
            IEnumerable<Schedule> videosource = ScheduleManager.Schedules.Values;
            return videosource;
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
        public async Task<IActionResult> Put( [FromRoute] Guid id, [FromBody] Schedule schedule)
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
                HttpContext.Request.Headers.TryGetValue("clientId", out StringValues clientId);
                ScheduleManager.UpdateInDbandMemory(schedule);
                SchedulAllDetails createdSchedulee =
                    ScheduleManager.scheduleWithAllDetailsDictionary[schedule.Id];
                var objectToSend = new Dictionary<string, dynamic>()
                {
                    {
                        "scheduleAllDetailsList",
                        new List<SchedulAllDetails>() { createdSchedulee }
                    },
                };
                
                await ScheduleManager.SendCrudDataToClient(
                    CrudMethodType.Update,
                    objectToSend
                );
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            StringValues clientId;
            HttpContext.Request.Headers.TryGetValue("clientId", out clientId);
            if (!ScheduleManager.Schedules.ContainsKey(id))
            {
                return NotFound();
            }
            try
            {
                var schedule = ScheduleManager.Schedules[id];
                SchedulAllDetails scheduleWithAllDetails =
                    ScheduleManager.scheduleWithAllDetailsDictionary[schedule.Id];
                ScheduleManager.Delete(schedule);
                var objectToSend = new Dictionary<string, dynamic>()
                {
                    {
                        "scheduleAllDetailsList",
                        new List<SchedulAllDetails>() { scheduleWithAllDetails }
                    },
                };
                await ScheduleManager.SendCrudDataToClient(
                    CrudMethodType.Delete,
                    objectToSend
                );
                return Ok(schedule);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("DeleteMultiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<Guid> ScheduleToBeDeleted)
        {
            
            StringValues clientId;
            HttpContext.Request.Headers.TryGetValue("clientId", out clientId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            List<SchedulAllDetails> scheduleAllDetailsList =
                new List<SchedulAllDetails>();
            foreach (var id in ScheduleToBeDeleted)
            {
                scheduleAllDetailsList.Add(
                    ScheduleManager.scheduleWithAllDetailsDictionary[id]
                );
            }

            ScheduleManager.DeleteMultipleFromDbAndInMemory(ScheduleToBeDeleted);
            var objectToSend = new Dictionary<string, dynamic>()
            {
                { "scheduleAllDetailsList", scheduleAllDetailsList },
            };
            await ScheduleManager.SendCrudDataToClient(
                CrudMethodType.Delete,
                objectToSend
            );
            return Ok();
        }
    }
}
