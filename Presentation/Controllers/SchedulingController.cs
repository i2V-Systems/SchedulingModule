
using CommonUtilityModule.CrudUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SchedulingModule.Application.DTOs;
using SchedulingModule.Application.Managers;
using SchedulingModule.Presentation.Models;
using Serilog;

namespace SchedulingModule.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[LicenceValid]
    public  class SchedulingController : Controller
    {
        private readonly IScheduleManager _scheduleManager;
        private readonly ILogger<SchedulingController> _logger;
        public SchedulingController(ILogger<SchedulingController> logger,IScheduleManager scheduleManager)
        {
            _logger = logger;
            _scheduleManager = scheduleManager;
        }

        [HttpGet]
        public async Task<IEnumerable<ScheduleDto>> GetAll()
        {
            IEnumerable<ScheduleDto> schedules = await _scheduleManager.GetAllCachedSchedulesAsync();
            return schedules;
        }
       
        [HttpGet("~/api/Schedules/GetAllResourceDetails")]
        public async Task<IActionResult> GetAllResourceDetails()
        { 
            try
            {
                StringValues UserName;
                HttpContext.Request.Headers.TryGetValue("Username", out UserName);
                return Ok(await _scheduleManager.GetScheduleWithAllDetailsAsync(UserName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return BadRequest(ex.Message.ToString());
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!_scheduleManager.IsScheduleLoaded(id))
                {
                    return NotFound();
                }
                return Ok(await _scheduleManager.GetAsync(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScheduleDto schedule)
        {
            try
            {
                HttpContext.Request.Headers.TryGetValue("clientId", out  StringValues clientId);
                HttpContext.Request.Headers.TryGetValue("userid", out StringValues userid);
                
                Guid scheduleId=  await _scheduleManager.CreateScheduleAsync(schedule,userid);
                SchedulAllDetails schedulesource = await _scheduleManager.GetDetailedAsync(scheduleId);
                var objectToSend = new Dictionary<string, dynamic>()
                {
                    {
                        "scheduleAllDetailsList",
                        new List<SchedulAllDetails>() { schedulesource }
                    }
                };
                await _scheduleManager.SendCrudDataToClientAsync(
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
        public async Task<IActionResult> Update( [FromRoute] Guid id, [FromBody] ScheduleDto schedule)
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
                await  _scheduleManager.UpdateScheduleAsync(schedule);
                SchedulAllDetails createdSchedulee =  await  _scheduleManager.GetDetailedAsync(schedule.Id);
                var objectToSend = new Dictionary<string, dynamic>()
                {
                    {
                        "scheduleAllDetailsList",
                        new List<SchedulAllDetails>() { createdSchedulee }
                    },
                };
                
                await _scheduleManager.SendCrudDataToClientAsync(
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
            if (!_scheduleManager.IsScheduleLoaded(id))
            {
                return NotFound();
            }
            try
            {
                var schedule = await _scheduleManager.GetScheduleFromCacheAsync(id);
                SchedulAllDetails scheduleWithAllDetails = await  _scheduleManager.GetScheduleDetailsFromCacheAsync(id);
                await _scheduleManager.DeleteScheduleAsync(id);
                var objectToSend = new Dictionary<string, dynamic>()
                {
                    {
                        "scheduleAllDetailsList",
                        new List<SchedulAllDetails>() { scheduleWithAllDetails }
                    },
                };
                await _scheduleManager.SendCrudDataToClientAsync(
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
               SchedulAllDetails scheduleDetailed= await  _scheduleManager.GetScheduleDetailsFromCacheAsync(id);
               scheduleAllDetailsList.Add(scheduleDetailed);
            }
            
            await _scheduleManager.DeleteMultipleSchedulesAsync(ScheduleToBeDeleted);
            var objectToSend = new Dictionary<string, dynamic>()
            {
                { "scheduleAllDetailsList", scheduleAllDetailsList },
            };
            await _scheduleManager.SendCrudDataToClientAsync(
                CrudMethodType.Delete,
                objectToSend
            );
            return Ok();
        }
    }
}
