using System.Net;
using System.Text.Json;
using Fadebook.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred. Please try again later.";

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundEx.Message;
                break;

            case ConflictException conflictEx:
                statusCode = HttpStatusCode.Conflict;
                message = conflictEx.Message;
                break;

            case BadRequestException badRequestEx:
                statusCode = HttpStatusCode.BadRequest;
                message = badRequestEx.Message;
                break;

            case NoUsernameException noUsernameEx:
                statusCode = HttpStatusCode.BadRequest;
                message = noUsernameEx.Message;
                break;

            case KeyNotFoundException keyNotFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = keyNotFoundEx.Message;
                break;

            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                message = validationEx.Message;
                break;

            case ArgumentNullException argNullEx:
                statusCode = HttpStatusCode.BadRequest;
                message = $"Required parameter is missing: {argNullEx.ParamName}";
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                message = argEx.Message;
                break;

            case InvalidOperationException invalidOpEx:
                statusCode = HttpStatusCode.Conflict;
                message = invalidOpEx.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "You are not authorized to perform this action.";
                break;

            case DbUpdateException:
                statusCode = HttpStatusCode.Conflict;
                message = "A database update error occurred.";
                break;

            default:
                // For unhandled exceptions, keep the generic message
                // In production, avoid exposing internal error details
                break;
        }

        var response = new
        {
            status = (int)statusCode,
            message = message,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.Value,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}