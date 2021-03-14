using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController( IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser user)
        {
            if (!ModelState.IsValid) return View(user);

            var stringContent = new StringContent(JsonConvert.SerializeObject(user),
                Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PostAsync($"{_config["BaseApiUrl"]}api/Account/login", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<ApiAuthResponse>(contentString);

                HttpContext.Session.SetString("token", data.Token);

                return RedirectToAction("Index", "home");
            }

            ModelState.AddModelError(string.Empty, "invalid login attempt");
            return View(user);
        }
        

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login", "account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);
            

            var stringContent = new StringContent(JsonConvert.SerializeObject(model),
                Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PostAsync($"{_config["BaseApiUrl"]}api/Account/register", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<ApiAuthResponse>(contentString);

                HttpContext.Session.SetString("token", data.Token);

                return RedirectToAction("Index", "home");
            }

            ModelState.AddModelError(string.Empty, "invalid registration attempt");
            return View(model);
        }
   
        [HttpGet]
        [HttpPost]
        public async Task<JsonResult> CheckEmail(string email)
        {
            var client = new HttpClient();
            
            var responseString = await client.GetStringAsync($"{_config["BaseApiUrl"]}api/Account/emailexists?email={email}");

            var result = JsonConvert.DeserializeObject<bool>(responseString);

            return result ?  Json($"email {email}  belongs to someone else") : Json(true);
        }
    }
}