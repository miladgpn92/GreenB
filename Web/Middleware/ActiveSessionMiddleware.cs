using Common.Utilities;
using Data.Repositories;
using Entities.User;
using Services.Services.CMS.ActiveSession;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Web.Middleware
{
    public class ActiveSessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRepository<ActiveSession> _activeSessionService;

        public ActiveSessionMiddleware(RequestDelegate next,IRepository<ActiveSession> activeSessionService)
        {
            _next = next;
            _activeSessionService = activeSessionService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var EncyToken = EncryptUtility.Encrypt(token);
                var findUser = await _activeSessionService.TableNoTracking
                    .FirstOrDefaultAsync(a => a.Token == EncyToken);
                    
                if (findUser == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
           

            //if (await tokenRevocationService.IsTokenRevokedAsync(token))
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return;
            //}

            await _next(context);
        }
    }
}
