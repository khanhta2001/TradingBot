using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TradingBotModels.Models;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

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
            var userAccount = new
            {
                Id = ObjectId.GenerateNewId().ToString(),
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
                    OAuthTokenSecret = "none"
                }
            };
            var json = userAccount.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7052/AccountInfo/LoginAccount", stringContent);
            
            if (!response.IsSuccessStatusCode) return View("Error");
            var content = await response.Content.ReadAsStringAsync();
            var user = BsonSerializer.Deserialize<Account>(content);
            if (user.UserName != null)
            {
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
            else
            {
                return this.RedirectToAction("LoginPage", "User");
            }

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
                    OAuthTokenSecret = "none"
                }
            };
            var json = userAccount.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7052/AccountInfo/RegisterAccount", stringContent);
            
            if (response.IsSuccessStatusCode)
            {
                return this.RedirectToAction("LoginPage", "User");
            }

            var errorMessage = new ErrorResponse()
            {
                ErrorMessage = "Unable to authenticate with the server."
            };
            return View("RegisterPage", errorMessage);
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
            
            var client = new HttpClient();
            var connectionResponse = await client.GetAsync($"https://localhost:7052/Connection/GetConnection?ConsumerKey={consumerKey}&ConsumerSecret={consumerSecret}");

            if (connectionResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Json(new { isSuccess = false, ErrorMessage = "You are not authorized to access this resource." });
            }
            else if (connectionResponse.StatusCode == HttpStatusCode.InternalServerError)
            {
                return Json(new { isSuccess = false, ErrorMessage = "An unexpected error has occurred. The error has been logged and is being investigated." });
            }
            else if (connectionResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return Json(new { isSuccess = false, ErrorMessage = "The request was malformed or invalid." });
            }

            var connectionContent = await connectionResponse.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(connectionContent);
            var oauthToken = jsonObject["OauthToken"]?.ToString();
            var oauthSecret = jsonObject["OauthSecret"]?.ToString();
            var authorizationUrl = jsonObject["AuthorizationUrl"]?.ToString();
            
            return Json(new { isSuccess = true, OauthToken = oauthToken, OauthSecret = oauthSecret, AuthorizationUrl = authorizationUrl });
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyCode")]
        public async Task<IActionResult> VerifyCode(string userName, string consumerKey, string consumerSecret, string oauthToken, string oauthSecret, string verificationCode)
        {
            var client = new HttpClient();
            
            var connectionAuth = new ConnectionAuth()
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                OAuthToken = oauthToken,
                OAuthTokenSecret = oauthSecret,
                VerificationCode = verificationCode
            };
            var json = connectionAuth.ToJson();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://localhost:7052/Connection/PostAuthorization?userName={userName}", stringContent);

            if (!response.IsSuccessStatusCode) return View("Error");
            
            return Json(new { isSuccess = true});
        }
    }   
}