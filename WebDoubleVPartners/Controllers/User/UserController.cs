using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using WebDoubleVPartners.Models;
using WebDoubleVPartners.Models.Inputs;
using WebDoubleVPartners.Utils;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;

namespace WebDoubleVPartners.Controllers.User
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult CreateUser()
        {
            return View(ConstantsWebDVP.CreateUser);
        }

        [HttpPost(ConstantsWebDVP.SaveUser)]
        public async Task<IActionResult> SaveUser(UserInput model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var apiBaseAddress = new Uri(ConstantsWebDVP.ApiBackend);

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = apiBaseAddress;

                var requestContent = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        userName = model.UserName,
                        pass = model.Pass,
                        creationDate = model.CreationDate,
                    }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(ConstantsWebDVP.EndpointSaveUser, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(ConstantsWebDVP.GetUsers, ConstantsWebDVP.UserController);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ConstantsWebDVP.InvalidToCreateUser);
                }
            }

            return View(ConstantsWebDVP.Login, model);
        }

        [HttpGet(ConstantsWebDVP.GetUsers)]
        public async Task<IActionResult> GetUsers()
        {
            var apiBaseAddress = new Uri(ConstantsWebDVP.ApiBackend);

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = apiBaseAddress;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync(ConstantsWebDVP.EndpointGetUsers);


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var users = JsonSerializer.Deserialize<List<Models.User>>(content);

                return View(ConstantsWebDVP.GetUsers, users);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
