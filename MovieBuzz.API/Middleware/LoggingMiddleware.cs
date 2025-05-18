using System.Diagnostics;
using MovieBuzz.Services.Monitor;
using Serilog;

namespace MovieBuzz.API.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiMonitoringService _monitor;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(
        RequestDelegate next,
        ApiMonitoringService monitor,
        ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _monitor = monitor;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var statusCode = 200;
        var success = true;

        try
        {
            _logger.LogInformation("Request: {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await _next(context);
            statusCode = context.Response.StatusCode;
            success = statusCode is >= 200 and < 300;
        }
        catch (Exception)
        {
            success = false;
            throw; // Let ExceptionMiddleware handle it
        }
        finally
        {
            sw.Stop();
            _monitor.LogApiRequest(
                context.Request.Path,
                context.Request.Method,
                statusCode,
                sw.ElapsedMilliseconds,
                success);

            _logger.LogInformation("Response: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                sw.ElapsedMilliseconds);
        }
    }
}