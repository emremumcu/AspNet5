using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.Controllers
{
    [EnableCors("allow_all")]
    [Route("api/[controller]")]
    [ApiController]
    public class PublicApiController : ControllerBase
    {
        // https://localhost:44300/api/publicapi/getlistselectitem
        [Route("[action]")]
        [HttpGet]
        public List<SelectListItem> GetListSelectItem() // @Html.DropDownList("ddlName", (IEnumerable<SelectListItem>)Model)
        {
            string[] lines = System.IO.File.ReadAllLines(@"Static\countries.txt");

            List<SelectListItem> countries = new List<SelectListItem>();

            foreach (string line in lines)
            {
                countries.Add(new SelectListItem() { Value = "", Text = line });
            }

            return countries;            
        }

        // https://localhost:44300/api/publicapi/getlist
        [Route("[action]")]
        [HttpGet]
        public List<Country> GetList() // @Html.DropDownList("ddlName", new SelectList(Model, "CountryId", "CountryName"))
        {
            string[] lines = System.IO.File.ReadAllLines(@"Static\countries.txt");

            List<Country> countries = new List<Country>();

            foreach (string line in lines)
            {
                countries.Add(new Country() { CountryName = line });
            }

            return countries;
        }

        // https://localhost:44300/api/publicapi/getselectlist
        [Route("[action]")]
        [HttpGet]
        public SelectList GetSelectList() // @Html.DropDownList("ddlName", (IEnumerable<SelectListItem>)Model)
        {
            string[] lines = System.IO.File.ReadAllLines(@"Static\countries.txt");

            SelectList countries = new SelectList(lines.ToList(), "CountryId", "CountryName");

            return countries;
        }

        // https://localhost:44300/api/publicapi/getjson
        [Route("[action]")]
        [HttpGet]
        public JsonResult GetJson() 
        {
            string[] lines = System.IO.File.ReadAllLines(@"Static\countries.txt");

            List<Country> countries = new List<Country>();

            foreach (string line in lines)
            {
                countries.Add(new Country() { CountryId = "0", CountryName = line });
            }

            return new JsonResult(countries)
            {
                StatusCode = StatusCodes.Status201Created 
            };
        }

        // https://localhost:44300/api/publicapi/getjson
        [Route("[action]/{name}")]
        [HttpGet]
        public JsonResult GetJson(string name)
        {
            string[] lines = System.IO.File.ReadAllLines(@"Static\countries.txt");

            List<Country> countries = new List<Country>();

            foreach (string line in lines)
            {
                countries.Add(new Country() { CountryId = "0", CountryName = name+" "+line });
            }

            return new JsonResult(countries)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }

    public class Country
    {
        public string CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
