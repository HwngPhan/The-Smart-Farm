using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TSF_mustidisProj.Data;
using TSF_mustidisProj.Models;
using TSF_mustidisProj.Services;

namespace TSF_mustidisProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MqttController : ControllerBase
    {
        private readonly IMqttService _mqttService;
        private readonly ApplicationDbContext _dbContext;

        public MqttController(
            IMqttService mqttService,
            ApplicationDbContext dbContext)
        {
            _mqttService = mqttService;
            _dbContext = dbContext;
        }

        [HttpGet("feeds")]
        public async Task<IActionResult> GetFeeds()
        {
            var feeds = await _dbContext.Feeds
                .Select(f => new {
                    f.Id,
                    f.Name,
                    f.Key,
                    f.LastValue,
                    f.RecordedAt,
                    f.UserId
                })
                .ToListAsync();
                
            return Ok(feeds);
        }

        [HttpPost("publish")]
        public async Task<IActionResult> PublishMessage([FromBody] PublishRequest request)
        {
            if (string.IsNullOrEmpty(request.Feed) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Feed and message are required");
            }
            
            try
            {
                await _mqttService.PublishAsync(request.Feed, request.Message);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeToFeed([FromBody] SubscribeRequest request)
        {
            if (string.IsNullOrEmpty(request.Feed))
            {
                return BadRequest("Feed is required");
            }
            
            try
            {
                await _mqttService.SubscribeAsync(request.Feed);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class PublishRequest
    {
        public string Feed { get; set; }
        public string Message { get; set; }
    }

    public class SubscribeRequest
    {
        public string Feed { get; set; }
    }
}