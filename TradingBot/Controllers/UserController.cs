using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TradingBotAPI.Models;
using MongoDB.Bson.Serialization;

namespace TradingBot.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        
        public async Task<IActionResult> UserAccount(string userName)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://localhost:7052/AccountInfo/UserAccount?userName={userName}");

            if (!response.IsSuccessStatusCode) return View("Error");
            var content = await response.Content.ReadAsStringAsync();
    
            // Deserialize the JSON to a single UserAccount object
            var data = BsonSerializer.Deserialize<Account>(content);
    
            // Pass the single UserAccount object to the view
            return View("UserAccount", data);

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
            var userAccount = new Account()
            {
                UserName = username,
                Email = "no one cares",
                PasswordHash = password,
                VerificationCode = "Verified",
                ConnectionAuth = new ConnectionAuth()
                {
                    ConsumerKey = "None",
                    ConsumerSecret = "None",
                    VerificationCode = "None",
                    OAuthToken = "None",
                    OAuthTokenSecret = "none",
                    DailyToken = "None"
                }
            };
            var json = userAccount.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7052/AccountInfo/LoginAccount", stringContent);
            
            if (!response.IsSuccessStatusCode) return View("Error");
            var content = await response.Content.ReadAsStringAsync();
            var user = BsonSerializer.Deserialize<Account>(content);
            var claims = new List<Claim>
            {
                new Claim("UserId", "User Controller"),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "User"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                }).ConfigureAwait(false);

            return this.RedirectToAction("HomePage", "Home");

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
            
            var passwordHasher = new PasswordHasher<string>();
            if (password != passwordConfirm)
            {
                return View("RegisterPage");
            }
            var userAccount = new Account()
            {
                UserName = username,
                Email = email,
                PasswordHash = password,
                VerificationCode = "Verified",
                ConnectionAuth = new ConnectionAuth()
                {
                    ConsumerKey = "None",
                    ConsumerSecret = "None",
                    VerificationCode = "None",
                    OAuthToken = "None",
                    OAuthTokenSecret = "none",
                    DailyToken = "None"
                }
            };
            var json = userAccount.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7052/AccountInfo/RegisterAccount", stringContent);
            
            if (response.IsSuccessStatusCode)
            {
                return this.RedirectToAction("LoginPage", "User");
            }

            return View("RegisterPage");
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return this.RedirectToAction("HomePage", "Home");
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("EditConsumerDetails")]
        public async Task<IActionResult> EditConsumerDetails(string userName, string consumerKey, string consumerSecret)
        {

            var connectionAuth = new ConnectionAuth
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };
            var client = new HttpClient();
            var connectionResponse = await client.GetAsync($"https://localhost:7052/Connection/GetConnection?connectionAuth={connectionAuth}");

            if (!connectionResponse.IsSuccessStatusCode) return View("Error");
            var connectionContent = await connectionResponse.Content.ReadAsStringAsync();
    
            // Deserialize the JSON to a single UserAccount object
            var connectionData = BsonSerializer.Deserialize<Account>(connectionContent);
            var userAccount = new Account()
            {
                UserName = userName,
                ConnectionAuth = connectionAuth
            };
            var json = userAccount.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7052/AccountInfo/ChangeConsumerAuth", stringContent);

            if (!response.IsSuccessStatusCode) return View("Error");
            var content = await response.Content.ReadAsStringAsync();
    
            return View("RegisterPage", content);
        }
    }   
}