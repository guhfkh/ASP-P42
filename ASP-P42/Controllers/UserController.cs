using ASP_P42.Data;
using ASP_P42.Data.Entities;
using ASP_P42.Models.User;
using ASP_P42.Services.Kdf;
using ASP_P42.Services.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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

        public IActionResult SignUp([FromBody]UserSignupFormModel formModel)
        {
            if(formModel == null)
            {
                return BadRequest("data structure non-bindable to model");
            }

            if (!formModel.IsAgree)
            {
                return BadRequest("You should confirm site policy");
            }

            string requiredMessage = " could not be empty";

            if (string.IsNullOrEmpty(formModel.Login))
            {
                return BadRequest(nameof(formModel.Login) + requiredMessage);
            }

            if (string.IsNullOrEmpty(formModel.FullName))
            {
                return BadRequest(nameof(formModel.FullName) + requiredMessage);
            }

            if (string.IsNullOrEmpty(formModel.Phone))
            {
                return BadRequest(nameof(formModel.Phone) + requiredMessage);
            }

            if (string.IsNullOrEmpty(formModel.Email))
            {
                return BadRequest(nameof(formModel.Email) + requiredMessage);
            }

            if (string.IsNullOrEmpty(formModel.Password))
            {
                return BadRequest(nameof(formModel.Password) + requiredMessage);
            }

            if (formModel.Password != formModel.Repeat)
            {
                return BadRequest("Password and Repeat mismatch");
            }

            formModel.FullName = formModel.FullName.Trim();
            if (formModel.FullName.Length < 2)
            {
                return BadRequest(nameof(formModel.FullName) + " too short (2 symbols at least)");
            }

            formModel.Login = formModel.Login.Trim();
            if (formModel.Login.Length < 2)
            {
                return BadRequest(nameof(formModel.Login) + " too short (2 symbols at least)");
            }
            if (formModel.Login.Contains(':'))
            {
                return BadRequest(nameof(formModel.Login) + " could not contain colon (':')");
            }

            formModel.Email = formModel.Email.Trim();
            if (!Regex.IsMatch(
                formModel.Email,
                @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                return BadRequest(nameof(formModel.Email) + " has invalid format");
            }

            if(_dataContext.UserAccesses.Any(ua => ua.Login == formModel.Login))
            {
                return BadRequest(nameof(formModel.Login) + $"{formModel.Login} is already in use");
            }


            Guid userId = Guid.NewGuid();
            _dataContext.UserData.Add(new()
            {
                Id = userId,
                FullName = formModel.FullName,
                Email = formModel.Email,
                Phone = formModel.Phone,
                RegisteredAt = DateTime.Now,
                Birthdate = default,
            });

            string salt = Guid.NewGuid().ToString();
            _dataContext.UserAccesses.Add(new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = _dataContext.UserRoles.First(r => r.Name == "User").Id,
                Login = formModel.Login,
                Salt = salt,
                Dk = _kdfService.Dk(formModel.Password, salt),
            });

            _dataContext.SaveChanges();
            return Json(formModel);
        }

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

            if(login.Contains(':'))
            {
                throw new Exception(
                    "Login must not contain ':' according to RFC 7617");
            }

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