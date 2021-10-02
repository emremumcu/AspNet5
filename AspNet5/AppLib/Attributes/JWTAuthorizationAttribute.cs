using AspNet5.AppLib.Tools;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Attributes
{
    // https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authorization/Core/src/AuthorizeAttribute.cs

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class JWTAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// JWT Token should be present in header as 
        /// Key: "Authorization" Value: "Bearer jwt.token.here" (set both key and value without quotes)
        /// </summary>
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext != null)
            {
                Microsoft.Extensions.Primitives.StringValues authorizationToken;

                filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);

                var tokenSend = authorizationToken.FirstOrDefault();

                if (tokenSend != null)
                {
                    if (ValidateToken(tokenSend, out string message))
                    {
                        filterContext.HttpContext.Response.Headers.Add("Authorization", tokenSend);
                        filterContext.HttpContext.Response.Headers.Add("RequestStatus", "Authorized");
                        return;
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Headers.Add("Authorization", tokenSend);
                        filterContext.HttpContext.Response.Headers.Add("RequestStatus", "Unauthorized");
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Unauthorized";
                        filterContext.Result = new JsonResult("Unauthorized")
                        {
                            Value = new
                            {
                                Status = "Unauthorized",
                                Message = $"Provided token is invalid. {message}"
                            },
                        };
                    }
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "TokenNotFound";
                    filterContext.Result = new JsonResult("TokenNotFound")
                    {
                        Value = new
                        {
                            Status = "ExpectationFailed",
                            Message = "Token is not provided"
                        },
                    };
                }
            }
        }

        public bool ValidateToken(string token, out string message)
        {
            if(token.StartsWith("Bearer ")) token = token.Split("Bearer ")[1];

            JWTFactory jwt = new JWTFactory();

            bool isValid = jwt.ValidateToken(token, out message);

            return isValid;
        }
    }
}
