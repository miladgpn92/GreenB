using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
namespace WebFramework.Middlewares
{
   public class LangCookieMiddleware
    {
        private readonly RequestDelegate _next;
      

        public LangCookieMiddleware(RequestDelegate next)
        {
            _next = next;

      
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (!httpContext.Request.Cookies.ContainsKey(".AspNetCore.Culture"))
            {
                httpContext.Request.Headers["Accept-Language"] = "fa-IR";
                httpContext.Response.Cookies.Append(".AspNetCore.Culture", "c=fa-IR|uic=fa-IR");
               
            }
            await _next(httpContext); // calling next middleware

        }
    }

    public static class LangCookieMiddlewareExtensions
    {
        public static IApplicationBuilder UseLangCookieMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LangCookieMiddleware>();
        }
    }
}
