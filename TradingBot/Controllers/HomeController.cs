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
        public async Task<IActionResult> HomePage()
        {
            if (User.Identity.IsAuthenticated)
            {
                var client = new HttpClient();
                var response = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={User.Identity.Name}");

                if (!response.IsSuccessStatusCode) return View("Error");
                var content = await response.Content.ReadAsStringAsync();
    
                // Deserialize the JSON to a single UserAccount object
                var data = BsonSerializer.Deserialize<Account>(content);
            
                return View(data);
            }

            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult AboutMe()
        {
            return View();
        }
    }   
}