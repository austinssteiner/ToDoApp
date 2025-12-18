using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ToDoApp.Server.Middleware;

/// <summary>
/// Logs inbound requests and outbound responses with correlation IDs for traceability.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = EnsureCorrelationId(context);
        var stopwatch = Stopwatch.StartNew();
        var pathWithQuery = $"{context.Request.Path}{context.Request.QueryString}";

        _logger.LogInformation(
            "Incoming request {Method} {Path} from {RemoteIp} (CorrelationId: {CorrelationId})",
            context.Request.Method,
            pathWithQuery,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            correlationId);

        try
        {
            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Completed request {Method} {Path} with {StatusCode} in {ElapsedMs} ms (CorrelationId: {CorrelationId})",
                context.Request.Method,
                pathWithQuery,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path} after {ElapsedMs} ms (CorrelationId: {CorrelationId})",
                context.Request.Method,
                pathWithQuery,
                stopwatch.ElapsedMilliseconds,
                correlationId);
            throw;
        }
    }

    private static string EnsureCorrelationId(HttpContext context)
    {
        const string headerName = "X-Correlation-Id";
        if (context.Request.Headers.TryGetValue(headerName, out var suppliedId) && !string.IsNullOrWhiteSpace(suppliedId))
        {
            context.Response.Headers[headerName] = suppliedId.ToString();
            return suppliedId.ToString();
        }

        var generated = Guid.NewGuid().ToString();
        context.Response.Headers[headerName] = generated;
        return generated;
    }
}
