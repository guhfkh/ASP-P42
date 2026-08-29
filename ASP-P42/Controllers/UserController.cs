using ASP_P42.Data;
using ASP_P42.Data.Entities;
using ASP_P42.Services.Kdf;
using ASP_P42.Services.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace ASP_P42.Controllers
{
    public class UserController(
            DataContext dataContext,
            IKdfService kdfService,
            ITimeService timeService
        ) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;
        private readonly ITimeService _timeService = timeService;
        public IActionResult BasicAuth()
        {
            UserAccess? userAccess;
            try
            {
                userAccess = AuthenticateUser();
            }
            catch ( Exception ex )
            {
                return BadRequest(ex.Message);
            }

            if ( userAccess == null )
            {
                return Unauthorized(
                        "Credentials rejected: chech login and password");
            }

            HttpContext.Session.SetString(
                "userAccessId",
                userAccess.Id.ToString()
                );

            return Ok();
        }
    
        public IActionResult BasicAuthJwt()
        {
            UserAccess? userAccess;
            try
            {
                userAccess = AuthenticateUser();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            if (userAccess == null)
            {
                return Unauthorized(
                        "Credentials rejected: chech login and password");
            }

            var header = new
            {
                alg = "HS256",
                typ = "JWT",
            };

            long time = _timeService.GetTimestamp();
            var payload = new
            {
                sub = userAccess.Login,
                iat = time,
                exp = time + 1_000_000,
                name = userAccess.UserData.FullName,
                email = userAccess.UserData.Email,
            };

            string body = Base64UrlTextEncoder.Encode(
                    Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(header)
                    ))
                    + "." +
                    Base64UrlTextEncoder.Encode(
                    Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(payload)
                    )
                );

            string signature = Base64UrlTextEncoder.Encode(
                    System.Security.Cryptography.HMACSHA256.HashData(
                        Encoding.UTF8.GetBytes("secret"),
                        Encoding.UTF8.GetBytes(body)
                    ));

            return Ok(body + "." + signature);
        }

        private UserAccess? AuthenticateUser()
        {
            string authHeader = HttpContext.Request.Headers.Authorization.ToString();
            if (authHeader == string.Empty)
            {
                throw new Exception("Missing Autorization header");
            }

            string scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
            {
                throw new Exception(
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
                throw new Exception(
                    "Authorization credentials must be valid Base64::section 4");
            }

            string userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(rawData);
            }
            catch (Exception)
            {
                throw new Exception(
                    "User-pass must be valid UTF8 string");
            }

            string[] parts = userPass.Split(':', 2);

            if (parts.Length != 2)
            {
                throw new Exception(
                    "User-pass must be concatenated by ':'");
            }
            string login = parts[0];
            string password = parts[1];

            if (_dataContext
                .UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .AsNoTracking()
                .FirstOrDefault(ua => ua.Login == login)
                is UserAccess userAccess)
            {
                string dk = _kdfService.Dk(password, userAccess.Salt);
                if (dk == userAccess.Dk)
                {
                    return userAccess;
                }
            }
            return null;
        }
    }
}