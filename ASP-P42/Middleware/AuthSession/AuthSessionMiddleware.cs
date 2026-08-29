using ASP_P42.Data;
using ASP_P42.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_P42.Middlewere.AuthSession
{
    public class AuthSessionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            DataContext dataContext    
        )
        {
            string authKey = "userAccessId";
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove(authKey);
                context.Response.Redirect(context.Request.Path);
                return;
            }

            context.Items.Add("ItemKey", "Item Value");

            if (context.Session.Keys.Contains(authKey))
            {
                string userAccessId = context.Session.GetString(authKey)!;

                UserAccess? userAccess = dataContext
                    .UserAccesses
                    .Include(ua => ua.UserRole)
                    .Include(ua => ua.UserData)
                    .AsNoTracking()
                    .FirstOrDefault(ua => ua.Id.ToString() == userAccessId);

                if (userAccess != null)
                {
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.Name, userAccess.UserData.FullName),
                            new(ClaimTypes.Email, userAccess.UserData.Email),
                            new(ClaimTypes.NameIdentifier, userAccess.Login),
                            new(ClaimTypes.Sid, userAccess.Id.ToString()),
                        ],
                        nameof(AuthSessionMiddleware)
                            )
                        );
                }
            }

            await _next(context);
        }
    }
}