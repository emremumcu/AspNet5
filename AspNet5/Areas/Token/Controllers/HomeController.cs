using AspNet5.AppLib.Attributes;
using AspNet5.AppLib.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNet5.Areas.Token.Controllers
{
    [Area("Token")]
    public class HomeController : Controller
    {
        // Base address of this application
        private Uri baseAddress = new Uri("https://localhost:44300");

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Microsoft recommends using IHttpClientFactory instead of HttpClient. See reference:
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1
        /// </summary>
        private string CreateTokenRequest(string username, string password)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = baseAddress;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        
            UserInfo userInfo = new UserInfo() { username = username, password = password };
            string userInfoJSON = JsonConvert.SerializeObject(userInfo);

            // HttpContent content = new StringContent(userInfoJSON, System.Text.Encoding.UTF8, "application/json");
            HttpContent content = new ByteArrayContent(Encoding.UTF8.GetBytes(userInfoJSON));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync("/Token/Home/TokenEndpoint", content).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get token from service");
            }

            string stringJWT = response.Content.ReadAsStringAsync().Result;
            JWT jwt = JsonConvert.DeserializeObject<JWT>(stringJWT);
            String returnedToken = jwt.Token;
            return returnedToken;
        }

        [IgnoreAntiforgeryToken]
        public IActionResult TokenEndpoint([FromForm] UserInfo userInfo)
        {
            // Check user credentials

            ClaimsIdentity ci = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "uid") });
            JWTFactory jwt = new JWTFactory(ci);
            string token = jwt.CreateToken();
            return Ok(new { Token = token });
        }

        public IActionResult CreateRequestWithToken(string token)
        {
            HttpClient client = new HttpClient();
            
            client.BaseAddress = baseAddress;
            client.Timeout = new TimeSpan(0, 0, 120);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = client.GetAsync("/Token/Home/AppRequest").Result;

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

        [JWTAuthorization]
        public IActionResult AppRequest()
        {
            return Content("OK");
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
