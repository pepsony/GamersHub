using Microsoft.AspNetCore.Mvc;

namespace GamersHub.Controllers
{
    public class ErrorController : Controller
    {       

        // Handle 500 Internal Server Error
        [Route("Error/ServerError")]
        public IActionResult ServerError()
        {
            // Return your custom 500 page view
            return View("BadRequestPage");
        }
        

        // Handle other status codes like 404
        [Route("Error/StatusCode/{code}")]
        public IActionResult StatusCodeHandler(int code)
        {
            if (code == 404)
            {
                return View("NotFoundPage");
            }
            // You can handle other codes here like (403, 401, etc.)
            //else if (code == 403)
            //{
            //    return View("ForbiddenPage");
            //}
                // Default fallback
                return View("BadRequestPage");
        }
    }
}
