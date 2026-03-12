using Marqelle.Application.DTO;
using System.Text.Json;

namespace Marqelle.Api.Middleware
{
    public class GlobalResponse
    {
        private readonly RequestDelegate _next;

        public GlobalResponse(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted)
                {
                    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                        context.Response.ContentType = "application/json";
                        var response = new ApiResponseDto<object>(
                            StatusCodes.Status401Unauthorized,
                            false,
                            "You are not logged in. Please login to continue.",
                            null
                        );
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        context.Response.ContentType = "application/json";
                        var response = new ApiResponseDto<object>(
                            StatusCodes.Status403Forbidden,
                            false,
                            "You do not have permission to access this resource.",
                            null
                        );
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                }
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

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}