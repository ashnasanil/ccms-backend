using System.Net;
using System.Text.Json;

namespace CCMS.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            Error = "An internal server error occurred.",
            Message = exception.Message, // In production, consider hiding detailed message
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        // You can add specific exception type handling here for NotFound, BadRequest, etc.
        // e.g., if (exception is ValidationException) { context.Response.StatusCode = 400; response.StatusCode = 400; response.Message = ... }

        context.Response.StatusCode = response.StatusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
