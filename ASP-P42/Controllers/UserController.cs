using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class UserController : Controller
    {
        public IActionResult BasicAuth()
        {
            string authHeader = HttpContext.Request.Headers.Authorization.ToString();
            if(authHeader == string.Empty)
            {
                return Unauthorized("Missing Autorization header");
            }

            string scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
            {
                return Unauthorized("Authorization scheme must be 'Basic '");
            }

            return Json(authHeader);
        }
    }
}
