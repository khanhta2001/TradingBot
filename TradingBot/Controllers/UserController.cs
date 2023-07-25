using System.Text;
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
        
        private readonly SignInManager<UserAccount> _signInManager;
        public UserController(ILogger<UserController> logger, SignInManager<UserAccount> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }
        
        public async Task<IActionResult> UserAccount()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7052/");
            var response = await client.GetAsync("api/UserAccount");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);

                return View("UserAccount", data); 
            }

            return View("Error");
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
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7052/");
            var userAccount = new UserAccount()
            {
                UserName = username,
                Password = password
            };
            var json = JsonConvert.SerializeObject(userAccount);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("AccountInfo/LoginAccount", stringContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IEnumerable<ConnectionAuth>>(content);
                // Use data as needed, pass it to your view, etc.
                await _signInManager.SignInAsync(userAccount, isPersistent: false);
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
        public async Task<IActionResult> Register(string username, string email, string password, string passwordConfirm)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7052/"); // or use http://localhost:5010/ if needed
            var userAccount = new UserAccount()
            {
                UserName = username,
                Email = email,
                Password = password,
            };
            var json = JsonConvert.SerializeObject(userAccount);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("AccountInfo/RegisterAccount", stringContent);

            if (response.IsSuccessStatusCode)
            {
                // Use data as needed, pass it to your view, etc.

                return this.RedirectToAction("LoginPage", "User");
            }

            return View("RegisterPage");
        }
        
    }   
}