using AspNetDemo_GlobalExceptions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using NLog;

namespace AspNet5.AppLib.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        private ExceptionContext _context;

        public override void OnException(ExceptionContext context)
        {
            _context = context;

            if (Is_WebAPI())
            {
                /// Handle WebAPI exception
                ApiException();
            }
            else if (Is_View_Or_Page())
            {
                /// Handle View/Page exception
                /// Return one of them: Content, Response or View
                /// ---------------------------------------------
                /// ViewPage_Error_In_ContentResult();
                /// ViewPage_Error_In_Response();
                ViewPage_Error_In_View();
            }
            else
            {                
                logger.Log(LogLevel.Error, $"Unspecified Exception {_context?.Exception?.Message}");
            }

            base.OnException(context);
        }

        private bool Is_WebAPI()
        {
            ControllerActionDescriptor actionDescriptor = _context.ActionDescriptor as ControllerActionDescriptor;

            /// API (Api implements ControllerBase but not Controller)
            if (actionDescriptor.ControllerTypeInfo.IsSubclassOf(typeof(ControllerBase)) && !actionDescriptor.ControllerTypeInfo.IsSubclassOf(typeof(Controller)))
                return true;
            else
                return false;
        }

        private bool Is_View_Or_Page()
        {
            ControllerActionDescriptor actionDescriptor = _context.ActionDescriptor as ControllerActionDescriptor;

            /// View/Page (View/Page implements ControllerBase and Controller)
            if (actionDescriptor.ControllerTypeInfo.IsSubclassOf(typeof(ControllerBase)) && actionDescriptor.ControllerTypeInfo.IsSubclassOf(typeof(Controller)))
                return true;
            else
                return false;
        }

        private string ExceptionAsJson()
        {
            ControllerActionDescriptor actionDescriptor = _context.ActionDescriptor as ControllerActionDescriptor;

            Dictionary<string, string> error = new Dictionary<string, string>
            {
                {"Source", $"{actionDescriptor.ControllerName} / {actionDescriptor.ActionName}"},
                {"Type", _context.Exception.GetType().ToString()},
                {"Message", _context.Exception.Message},
                {"StackTrace", _context.Exception.StackTrace}
            };

            foreach (DictionaryEntry data in _context.Exception.Data) error.Add(data.Key.ToString(), data.Value.ToString());

            string jsonException = JsonConvert.SerializeObject(error, Formatting.Indented);

            return jsonException;
        }

        #region Api Error

        private void ApiException()
        {
            _context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _context.HttpContext.Response.ContentType = "application/json";
            _context.Result = new JsonResult(ExceptionAsJson());
        }

        #endregion Api Error

        #region View-Page Error

        private void ViewPage_Error_In_ContentResult()
        {
            ControllerActionDescriptor actionDescriptor = _context.ActionDescriptor as ControllerActionDescriptor;

            ContentResult cResult = new ContentResult()
            {
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status500InternalServerError,
                Content = ExceptionAsJson()
            };

            _context.ExceptionHandled = true;

            _context.Result = cResult;
        }

        private void ViewPage_Error_In_Response()
        {
            _context.ExceptionHandled = true;

            HttpResponse response = _context.HttpContext.Response;

            response.StatusCode = StatusCodes.Status500InternalServerError;

            response.ContentType = "application/json";

            response.WriteAsync(ExceptionAsJson());
        }

        private void ViewPage_Error_In_View()
        {
            _context.ExceptionHandled = true;

            ControllerActionDescriptor actionDescriptor = _context.ActionDescriptor as ControllerActionDescriptor;

            ViewResult result = new ViewResult() { ViewName = "_Exception" };

            ViewDataDictionary vdd = new ViewDataDictionary(new EmptyModelMetadataProvider(), _context.ModelState)
            {
                Model = new GlobalExceptionViewModel()
                {
                    Source = $"{actionDescriptor.ControllerName}/{actionDescriptor.ActionName}",
                    IsStatusCodeException = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ApplicationException = _context.Exception
                }
            };

            result.ViewData = vdd;
            result.ViewData.Add("EventTime", DateTime.Now);

            _context.Result = result;
        }

        #endregion View-Page Error
    }
}
