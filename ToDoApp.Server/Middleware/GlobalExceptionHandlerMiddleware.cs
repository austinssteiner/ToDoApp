using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Server.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "An error occurred while processing your request.";
        var detail = exception.Message;

        // Map exception types to HTTP status codes
        switch (exception)
        {
            case InvalidOperationException invalidOpEx:
                // Determine status code based on exception message content
                if (invalidOpEx.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
                {
                    statusCode = HttpStatusCode.NotFound;
                    title = "Resource not found";
                }
                else if (invalidOpEx.Message.Contains("Invalid username or password", StringComparison.OrdinalIgnoreCase))
                {
                    statusCode = HttpStatusCode.Unauthorized;
                    title = "Authentication failed";
                }
                else if (invalidOpEx.Message.Contains("already taken", StringComparison.OrdinalIgnoreCase) ||
                         invalidOpEx.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Validation error";
                }
                else
                {
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Invalid operation";
                }
                break;

            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Invalid argument";
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                title = "Access denied";
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource not found";
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                title = "An internal server error occurred";
                // Don't expose internal error details in production
                if (!context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    detail = "An error occurred while processing your request. Please try again later.";
                }
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Add exception details in development
        if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            problemDetails.Extensions.Add("exception", exception.GetType().Name);
            problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        return context.Response.WriteAsync(json);
    }
}

