using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
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
            client.BaseAddress = new Uri("https://localhost:7052/");
            var response = await client.GetAsync("api/Portfolio");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = BsonSerializer.Deserialize<Portfolio>(content);

                return View(); 
            }
            return View();
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
            client.BaseAddress = new Uri("https://localhost:7052/");
            var response = await client.GetAsync("api/CreatePortfolio");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = BsonSerializer.Deserialize<Portfolio>(content);
                return this.RedirectToAction("PortfolioPage", "Portfolio"); 
            }

            return View("Error");
        }
    }   
}