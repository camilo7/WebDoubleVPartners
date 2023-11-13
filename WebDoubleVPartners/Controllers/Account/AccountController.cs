using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WebDoubleVPartners.Models;
using Microsoft.VisualBasic;
using WebDoubleVPartners.Utils;

namespace WebDoubleVPartners.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var apiBaseAddress = new Uri(ConstantsWebDVP.ApiBackend);

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = apiBaseAddress;

                var requestContent = new StringContent(
                    JsonSerializer.Serialize(new { userName = model.UserName, pass = model.Pass }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(ConstantsWebDVP.EndpointValidateUser, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(ConstantsWebDVP.Index, ConstantsWebDVP.HomeController);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ConstantsWebDVP.InvalidUserOrPassword);
                }
            }

            return View(ConstantsWebDVP.Login, model);
        }
    }
}
