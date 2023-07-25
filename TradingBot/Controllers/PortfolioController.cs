using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TradingBotAPI.Models;

namespace TradingBot.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ILogger<PortfolioController> _logger;
        public PortfolioController(ILogger<PortfolioController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("PortfolioPage")]
        public async Task<IActionResult> PortfolioPage()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.

                return View(); 
            }

            return View("Error");
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("CreatePortfolioPage")]
        public IActionResult CreatePortfolioPage()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("CreatePortfolio")]
        public async Task<IActionResult> CreatePortfolio()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.

                return this.RedirectToAction("PortfolioPage", "Portfolio"); 
            }

            return View("Error");
        }
    }   
}