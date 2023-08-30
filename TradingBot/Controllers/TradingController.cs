using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using TradingBotModels.Models;

namespace TradingBot.Controllers
{
    public class TradingController : Controller
    {
        private readonly ILogger<TradingController> _logger;
        public TradingController(ILogger<TradingController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("ConductTrading")]
        public async Task<IActionResult> ConductTrading(int totalAmount, string action)
        {
            var client = new HttpClient();
            if (action == "sell")
            {
                var response = await client.GetAsync("https://localhost:7101/OrderAction");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = BsonSerializer.Deserialize<Portfolio>(content);

                    return Ok(); 
                }
            }
            else if (action == "buy")
            {
                var response = await client.GetAsync("https://localhost:7101/OrderAction");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = BsonSerializer.Deserialize<Portfolio>(content);

                    return Ok(); 
                }
            }
            return View("Error");
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("SearchProducts")]
        public async Task<IActionResult> SearchProducts(string userName, string search)
        {
            var client = new HttpClient();
            var userResponse = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = BsonSerializer.Deserialize<Account>(userContent);
            var response = await client.GetAsync($"https://localhost:7101/OrderAction/GetQuotes?quoteSymbols={search}&accessToken={userData.ConnectionAuth?.OAuthToken}&accountIdKey={userData.AccountIdKey}");

            if (!response.IsSuccessStatusCode) return View("Error");
            var content = await response.Content.ReadAsStringAsync();
    
            // Deserialize the JSON to a single UserAccount object
            var data = BsonSerializer.Deserialize<Account>(content);
    
            return Ok(); 
        }
    }   
}