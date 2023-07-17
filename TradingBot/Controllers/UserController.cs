using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradingBotAPI.Models;
using Newtonsoft.Json;

namespace TradingBot.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        
        public async Task<IActionResult> UserAccount()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.

                return View(); // Pass data to the view
            }

            return View("Error"); // Return an error view or another appropriate response if the request was not successful.
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("LoginPage")]
        public IActionResult LoginPage()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.

                return this.RedirectToAction("HomePage", "Home"); // Pass data to the view
            }

            return View("Error"); // Return an error view or another appropriate response if the request was not successful.
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("RegisterPage")]
        public IActionResult RegisterPage()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://your-api-url.com/");
            var response = await client.GetAsync("api/your-endpoint");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.

                return this.RedirectToAction("HomePage", "Home"); // Pass data to the view
            }

            return View("Error"); // Return an error view or another appropriate response if the request was not successful.
        }
        
    }   
}