using Newtonsoft.Json;

namespace Web.Middleware
{
    public class Error401Middleware
    {
        private readonly RequestDelegate _next;

        public Error401Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 302 && context.Request.Path.StartsWithSegments("/api"))
            {

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Unauthorized"
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                return;

            }

 
        }
    }
}
