using System.Text.Json;
using FluentValidation;

namespace Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            // Grupowanie błędów po nazwie pola
            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key.Replace("Employee.", ""), // usuwamy przedrostek "Employee."
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            var response = new
            {
                title = "One or more validation errors occurred.",
                status = 400,
                errors
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            object response;

            if (env.IsDevelopment())
            {
                response = new { status = 500, message = ex.Message, details = ex.StackTrace };
            }
            else
            {
                response = new { status = 500, message = "An internal server error occurred." };
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}