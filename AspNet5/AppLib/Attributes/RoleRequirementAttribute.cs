using AspNet5.AppLib.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Attributes
{
    /// <summary>
    /// ClaimTypes.Role must be SET with the required Role
    /// </summary>
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RoleRequirementAttribute : TypeFilterAttribute
    {
        public RoleRequirementAttribute(string Role) : base(typeof(RoleRequirementFilter))
        {
            Arguments = new object[] { Role };
        }
    }
}
