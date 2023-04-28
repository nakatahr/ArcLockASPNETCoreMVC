using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ArcHundred.Lock
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class WebHookController : ControllerBase
    {
        private readonly ILogger<WebHookController> _logger;

        public WebHookController(ILogger<WebHookController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Post()
        {
            try
            {
                using (var reader = new StreamReader(
                           Request.Body,
                           encoding: Encoding.UTF8,
                           detectEncodingFromByteOrderMarks: false))
                {
                    var json = await reader.ReadToEndAsync();
                    var doc = JsonSerializer.Deserialize<JsonElement>(json);

                    var deviceType = doc.GetProperty("context").GetProperty("deviceType").GetString();
                    if (deviceType == "WoLock")
                    {
                        var deviceMac = doc.GetProperty("context").GetProperty("deviceMac").GetString();
                        if (deviceMac == "E667EBA0816A") // Check if the mac address is identical with the one you bought
                        {
                            // "LOCKED" or "UNLOCKED" can be retrieved
                            var lockState = doc.GetProperty("context").GetProperty("lockState").GetString();

                            // Send to WebSocket (work in progress)
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
            return Ok();
        }

        /*
        Example of WebHook message
        {
            "eventType": "changeReport",
            "eventVersion": "1",
            "context": {
                "deviceType": "WoLock",
                "deviceMac": "E667EBA0816A",
                "lockState": "UNLOCKED",
                "battery": 95,
                "timeOfSample": 1680104122939
            }
        }
        */

    }
}
