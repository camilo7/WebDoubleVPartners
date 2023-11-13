using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebDoubleVPartners.Models.Inputs;
using WebDoubleVPartners.Utils;

namespace WebDoubleVPartners.Controllers.People
{
    public class PeopleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PeopleController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult CreatePerson()
        {
            return View(ConstantsWebDVP.CreatePerson);
        }

        [HttpPost(ConstantsWebDVP.SavePerson)]
        public async Task<IActionResult> SavePerson(PersonInput model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var apiBaseAddress = new Uri(ConstantsWebDVP.ApiBackend);

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = apiBaseAddress;

                var requestContent = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        names = model.Names,
                        lastNames = model.LastNames,
                        creationDate = model.CreationDate,
                        identificationType = model.IdentificationType,
                        identificationNumber = model.IdentificationNumber,
                        email = model.Email
                    }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(ConstantsWebDVP.EndpointSavePerson, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(ConstantsWebDVP.GetPersons, ConstantsWebDVP.PeopleController);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ConstantsWebDVP.InvalidToCreateUser);
                }
            }

            return View(ConstantsWebDVP.Login, model);
        }

        [HttpGet(ConstantsWebDVP.GetPersons)]
        public async Task<IActionResult> GetPersons()
        {
            var apiBaseAddress = new Uri(ConstantsWebDVP.ApiBackend);

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = apiBaseAddress;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync(ConstantsWebDVP.EndpointgetPeople);


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var users = JsonSerializer.Deserialize<List<Models.Person>>(content);

                return View(ConstantsWebDVP.GetPersons, users);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
