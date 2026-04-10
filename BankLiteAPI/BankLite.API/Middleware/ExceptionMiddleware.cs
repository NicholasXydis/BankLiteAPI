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
                context.Response.ContentType = "application/json";
                var error = new { message = ex.Message };

                context.Response.StatusCode = ex switch
                {
                    InvalidOperationException => 400,
                    UnauthorizedAccessException => 401,
                    _ => 500
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}

