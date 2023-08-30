using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Mvc;
using TradingBotModels.Models;

namespace TradingBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IActionResult HomePage()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
    }   
}