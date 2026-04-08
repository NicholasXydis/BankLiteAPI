using System.Text.Json;

namespace BankLite.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var error = new { message = ex.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}

