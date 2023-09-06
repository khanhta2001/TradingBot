using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        public async Task<IActionResult> PortfolioPage(string? userName)
        {
            if (User.Identity is not { IsAuthenticated: true }) return RedirectToAction("LoginPage", "User");
            
            userName ??= User.Identity.Name;
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = BsonSerializer.Deserialize<Account>(userContent);
            
            var response = await client.GetAsync($"https://localhost:7052/Portfolio/Portfolio?portfolioName={userData.Portfolio?.PortfolioName}");

            if (!response.IsSuccessStatusCode) return View();
            
            var content = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<Portfolio>(content);

            return View();

        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("CreatePortfolioPage")]
        public async Task<IActionResult> CreatePortfolioPage(string? userName)
        {
            if (User.Identity is not { IsAuthenticated: true }) return RedirectToAction("LoginPage", "User");
            
            userName ??= User.Identity.Name;
            
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = BsonSerializer.Deserialize<Account>(userContent);

            return View(userData);
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("CreatePortfolio")]
        public async Task<IActionResult> CreatePortfolio(string? userName)
        {
            if (User.Identity is not { IsAuthenticated: true }) return RedirectToAction("LoginPage", "User");
            
            userName ??= User.Identity.Name;
            
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = BsonSerializer.Deserialize<Account>(userContent);
            
            var json = userData.Portfolio.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://localhost:7052/Portfolio/CreatePortfolio", stringContent);

            if (!response.IsSuccessStatusCode) return View("Error");
            
            var content = await response.Content.ReadAsStringAsync();
    
            // Deserialize the JSON to a single UserAccount object
            var data = BsonSerializer.Deserialize<Account>(content);
    
            return RedirectToAction("PortfolioPage", "Portfolio");
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("StockPage")]
        public async Task<IActionResult> StockPage(string product)
        {
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/Portfolio/StockInfo?Stock={product}");
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
    }   
}