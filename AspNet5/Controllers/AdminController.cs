using AspNet5.AppLib.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet5.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]// prevent swagger to documentate controller
    [Authorize(Roles = "admin", AuthenticationSchemes = "Cookies")]
    public class AdminController : Controller
    {
        public IActionResult Index() => View(viewName: "_Temp", model: "Admin/Index");
    }
}


