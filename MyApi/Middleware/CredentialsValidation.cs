using Marqelle.Application.DTO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Marqelle.Api.Middleware
{
    public class CredentialValidation
    {
        private readonly RequestDelegate _next;

        public CredentialValidation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/usersAuth/register") ||
                context.Request.Path.StartsWithSegments("/api/usersAuth/login"))
            {
                if (!context.Request.HasFormContentType)
                {
                    await WriteResponse(context, StatusCodes.Status400BadRequest, "Request must be form-data.");
                    return;
                }

                var form = await context.Request.ReadFormAsync();


                if (context.Request.Path.StartsWithSegments("/api/usersAuth/register"))
                {
                    var firstName = form["FirstName"].ToString();
                    var lastName = form["LastName"].ToString();
                    var email = form["Email"].ToString();
                    var password = form["Password"].ToString();

                    if (string.IsNullOrWhiteSpace(firstName))
                    {
                        await WriteResponse(context, 400, "First name is required");
                        return;
                    }

                    if (!Regex.IsMatch(firstName, @"^[A-Za-z][A-Za-z\s]*$"))
                    {
                        await WriteResponse(context, 400, "First name must start with a letter and contain only letters and spaces");
                        return;
                    }


                    if (string.IsNullOrWhiteSpace(lastName))
                    {
                        await WriteResponse(context, 400, "Last name is required");
                        return;
                    }

                    if (!Regex.IsMatch(lastName, @"^[A-Za-z][A-Za-z\s]*$"))
                    {
                        await WriteResponse(context, 400, "Last name must start with a letter and contain only letters and spaces");
                        return;
                    }

                    if (!Regex.IsMatch(email ?? "", @"^[A-Za-z][A-Za-z0-9._%+-]*@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
                    {
                        await WriteResponse(context, 400, "Invalid email format");
                        return;
                    }

                    if (!IsStrongPassword(password))
                    {
                        await WriteResponse(context, 400,
                            "Password must be at least 8 characters and contain uppercase, lowercase, number and special character.");
                        return;
                    }
                }

        
                if (context.Request.Path.StartsWithSegments("/api/usersAuth/login"))
                {
                    var email = form["Email"].ToString();
                    var password = form["Password"].ToString();

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        await WriteResponse(context, 400, "Email is required");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(password))
                    {
                        await WriteResponse(context, 400, "Password is required");
                        return;
                    }

                    if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        await WriteResponse(context, 400, "Invalid email format");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = Regex.IsMatch(password, "[A-Z]");
            bool hasLower = Regex.IsMatch(password, "[a-z]");
            bool hasDigit = Regex.IsMatch(password, "[0-9]");
            bool hasSpecial = Regex.IsMatch(password, @"[\W_]");

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private async Task WriteResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponseDto<object>(
                statusCode,
                false,
                message,
                null
            );

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}