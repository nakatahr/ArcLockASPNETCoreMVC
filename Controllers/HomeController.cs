using ArcHundred.Lock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ArcHundred.Lock.Controllers
{

    //[Authorize]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SwitchBot.LockDevice _device = new SwitchBot.LockDevice();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("lockState") == false)
            {
                await GetStatus();
            }
            return View();
        }

        public async Task<IActionResult> GetStatus()
        {
            var status = await _device.GetStatus();
            TempData["lockState"] = status.lockState;
            TempData["doorState"] = status.doorState;
            TempData["battery"] = status.battery;
            TempData["error"] = status.error;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LockUnlock()
        {
            var status = await _device.LockUnlock();
            TempData["lockState"] = status.lockState;
            TempData["doorState"] = status.doorState;
            TempData["battery"] = status.battery;
            TempData["error"] = status.error;
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
