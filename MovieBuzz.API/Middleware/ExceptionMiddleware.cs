using System.Net;
using Serilog;
using MovieBuzz.Core.Exceptions;

namespace MovieBuzz.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Global exception caught");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        var statusCode = ex switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            BusinessRuleException or ConflictException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(new
        {
            Success = false,
            Status = statusCode.ToString(),
            ex.Message
        });
    }
}