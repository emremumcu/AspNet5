namespace AspNet5.Controllers
{
    using AspNet5.AppLib.Tools;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    [ApiExplorerSettings(IgnoreApi = true)]// prevent swagger to documentate controller
    [AllowAnonymous]
    public partial class JWTController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44300");

        [NonAction]
        public void Post1()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = baseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            UserInfo userInfo = new UserInfo() { username = "username", password = "password" };

            string stringData = JsonConvert.SerializeObject(userInfo);
            
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = client.PostAsync("/GetToken", contentData).Result;
            
            string stringJWT = response.Content.ReadAsStringAsync().Result;
            
            JWT jwt = JsonConvert.DeserializeObject<JWT>(stringJWT);
            
            String returnedToken = jwt.Token;
        }

        [NonAction]
        public async Task Post2(IHttpClientFactory clientFactory)
        {
            UserInfo userInfo = new UserInfo() { username = "uuu", password = "ppp" };

            var userInfoJson = new StringContent(
        System.Text.Json.JsonSerializer.Serialize(userInfo),
        Encoding.UTF8,
        "application/json");

            var httpClient = clientFactory.CreateClient();
            httpClient.BaseAddress = baseAddress;

            using var httpResponse = await httpClient.PostAsync("/GetToken", userInfoJson);

            httpResponse.EnsureSuccessStatusCode();

        }

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IHttpClientFactory clientFactory)        
        {
            Post1();

            await Post2(clientFactory);



            //var httpClient = new HttpClient();

            //var parameters = new Dictionary<string, string>();
            //parameters["text"] = "txt";

            //httpClient.BaseAddress = baseAddress;

            //var response = await httpClient.PostAsync("/GetTokenPost", new FormUrlEncodedContent(parameters));
            //var contents = await response.Content.ReadAsStringAsync();


            return View();




            // ***



            // ***

            ////HttpClient client2 = new HttpClient();
            ////client2.Timeout = new TimeSpan(0, 0, 120);
            ////client2.DefaultRequestHeaders.Accept.Clear();
            ////client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //////client2.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            //////client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TokenHere");
            ////var tokenResult = client2.GetStringAsync("https://localhost:44300/GetToken").Result;

            ////return View(model: tokenResult);
        }






        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login2([FromServices]IHttpClientFactory _clientFactory) // services.AddHttpClient();
        {
            UserInfo userInfo = new UserInfo() { username = "uuu", password = "ppp" };

            string stringData = JsonConvert.SerializeObject(userInfo);

            var todoItemJson = new StringContent(
            stringData,
            Encoding.UTF8,
            "application/json");

            var httpClient = _clientFactory.CreateClient();

            httpClient.BaseAddress = baseAddress;

            using var httpResponse =
            await httpClient.PostAsync("/TokenEndpoint", todoItemJson);

            httpResponse.EnsureSuccessStatusCode();

            return Content("hehe");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            

            HttpClient client = new HttpClient();

            client.BaseAddress = baseAddress;

            //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            //client.DefaultRequestHeaders.Accept.Add(contentType);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            UserInfo userInfo = new UserInfo() { username = username, password = password };

            string stringData = JsonConvert.SerializeObject(userInfo);

            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("/TokenEndpoint", contentData).Result;

            string stringJWT = response.Content.ReadAsStringAsync().Result;

            JWT jwt = JsonConvert.DeserializeObject<JWT>(stringJWT);

            String returnedToken = jwt.Token;

            return View(model: returnedToken);
        }

        [HttpPost, HttpGet]
        public IActionResult AppRequest(string token)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = baseAddress;

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");

            client.DefaultRequestHeaders.Accept.Add(contentType);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = client.GetAsync("/Home/Index").Result;

            string stringData = "";

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                stringData = "Unauthorized";
            }
            else
            {
                stringData = response.Content.ReadAsStringAsync().Result;
                //List<Employee> data = JsonConvert.DeserializeObject<List<Employee>>(stringData);
            }

            return Content(stringData);
        }







        public IActionResult Forbidden()
        {
            return Content("Forbidden");
        }

    }

    /// <summary>
    /// Endpoints
    /// </summary>
    public partial class JWTController
    {
        [Route("/GetToken")]
        [IgnoreAntiforgeryToken]
        public IActionResult GetToken()
        {
            ClaimsIdentity ci = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "uid") });
            JWTFactory jwt = new JWTFactory(ci);
            string token = jwt.CreateToken();
            return Ok(new { Token = token });
        }
    }


    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class JWT
    {
        public string Token { get; set; }
    }
}





////private async Task CreateCompany()
////{
////    var companyForCreation = new CompanyForCreationDto
////    {
////        Name = "Eagle IT Ltd.",
////        Country = "USA",
////        Address = "Eagle IT Street 289"
////    };
////    var company = JsonSerializer.Serialize(companyForCreation);
////    var requestContent = new StringContent(company, Encoding.UTF8, "application/json");
////    var response = await _httpClient.PostAsync("companies", requestContent);
////    response.EnsureSuccessStatusCode();
////    var content = await response.Content.ReadAsStringAsync();
////    var createdCompany = JsonSerializer.Deserialize<CompanyDto>(content, _options);
////}