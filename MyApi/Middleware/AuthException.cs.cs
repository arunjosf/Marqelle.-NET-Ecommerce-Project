using Marqelle.Application.DTO;
using System.Text.Json;

namespace Marqelle.Api.Middleware
{
    public class AuthException
    {
        private readonly RequestDelegate _next;

        public AuthException(RequestDelegate next)
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
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new ApiResponseDto<string>(
                    StatusCodes.Status500InternalServerError,
                    false,
                    ex.Message,
                    null
                );

                var result = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(result);
            }
        }
    }
}