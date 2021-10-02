using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.Areas.Admin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)] // prevent swagger to documentate controller
    // [Authorize(Roles = "admin", AuthenticationSchemes = "Cookies")]
    [Area("Admin")]
    public class AdminBaseController : Controller
    {

    }
}


//[ClaimRequirement(ClaimTypes.Role, "CanReadResource")]
//[RoleRequirement(Role:"admin")]