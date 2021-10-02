using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNet5.Controllers
{
    [EnableCors("allow_all")]
    public class DataBindingController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // HttpClient GET Json:
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44300"); 
            HttpResponseMessage response = await client.GetAsync("/api/publicapi/getlistselectitem");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            List<SelectListItem> list = JsonConvert.DeserializeObject<List<SelectListItem>>(responseBody);



            return View(model: list);
        }
    }
}
