using ASP_P42.Data;
using ASP_P42.Data.Entities;
using ASP_P42.Services.Kdf;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ASP_P42.Controllers
{
    public class UserController(
            DataContext dataContext,
            IKdfService kdfService
        ) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;
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
                return Unauthorized(
                    "Authorization scheme must be 'Basic '");
            }

            string credentials = authHeader[scheme.Length..];

            byte[] rawData;
            try
            {
                rawData = Convert.FromBase64String(credentials);
            }
            catch (Exception)
            {
                return Unauthorized(
                    "Authorization credentials must be valid Base64::section 4");
            }

            string userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(rawData);
            }
            catch (Exception)
            {
                return Unauthorized(
                    "User-pass must be valid UTF8 string");
            }

            string[] parts = userPass.Split(':', 2);

            if (parts.Length != 2)
            {
                return Unauthorized(
                    "User-pass must be concatenated by ':'");
            }
            string login = parts[0];
            string password = parts[1];

            if (_dataContext
                .UserAccesses
                .FirstOrDefault(ua => ua.Login == login)
                is UserAccess userAccess)
            {
                string dk = _kdfService.Dk(password, userAccess.Salt);
                if (dk == userAccess.Dk)
                {
                    HttpContext.Session.SetString(
                        "userAccessId",
                        userAccess.Id.ToString()
                        );

                    return Ok();
                }
            }

            return Unauthorized(
                    "Credentials rejected: chech login and password");
        }
    }
}
