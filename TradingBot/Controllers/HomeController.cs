using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.

                return View("NotificationsPage", data); // Pass data to the view
            }

            return View("Error"); // Return an error view or another appropriate response if the request was not successful.
        }

    }   
}