using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult StatusCodeHandler(int statusCode)
        {
            ViewBag.ErrorMessage = statusCode switch
            {
                404 => "Requested resource can not be found",
                _ => ViewBag.ErrorMessage
            };
            
            return View("NotFound");
        }
        
        [Route("/Error")]
        public IActionResult Error()
        {
            var details = HttpContext.Features
                .Get<IExceptionHandlerPathFeature>();
        
            ViewBag.ExPath = details.Path;
            ViewBag.ExMessage = details.Error.Message;
            ViewBag.StackTrace = details.Error.StackTrace;
            
            return View("Error");
        }
        
    }
}