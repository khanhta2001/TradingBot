using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Mvc;
using TradingBotAPI.Models;

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
        
        public async Task<IActionResult> NotificationsPage()
        {
            return View("NotificationsPage");
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = BsonSerializer.Deserialize<Portfolio>(content);
                return View("NotificationsPage", data);
            }

            return View("Error");
        }

    }   
}