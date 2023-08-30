using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using TradingBotModels.Models;

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
        public async Task<IActionResult> PortfolioPage(string userName)
        {
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = BsonSerializer.Deserialize<Account>(userContent);
            var response = await client.GetAsync($"https://localhost:7052/Portfolio/Portfolio?portfolioName={userData.Portfolio?.PortfolioName}");
            
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
        public async Task<IActionResult> CreatePortfolioPage(string userName)
        {
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = BsonSerializer.Deserialize<Account>(userContent);
            var response = await client.GetAsync($"https://localhost:7052/Portfolio/CreatePortfolio?userName={userName}");

            if (!response.IsSuccessStatusCode) return View("Error");
            var content = await response.Content.ReadAsStringAsync();
    
            // Deserialize the JSON to a single UserAccount object
            var data = BsonSerializer.Deserialize<Account>(content);
    
            return View(data);
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("CreatePortfolio")]
        public async Task<IActionResult> CreatePortfolio()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7052/CreatePortfolio");

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