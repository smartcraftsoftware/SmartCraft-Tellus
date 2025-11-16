using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartCraft.Core.Tellus.Domain.Exceptions;
using System.Net;
using ILogger = Serilog.ILogger;

namespace SmartCraft.Core.Tellus.Api.Filters;

/// <summary>
/// Global exception filter that handles exceptions and returns appropriate HTTP responses
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger _logger;

    public GlobalExceptionFilter(ILogger logger)
    {
        _logger = logger.ForContext<GlobalExceptionFilter>();
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.Error(exception, "An error occurred while processing the request");

        switch (exception)
        {
            case VehicleApiException vehicleApiException:
                HandleVehicleApiException(context, vehicleApiException);
                break;

            case ExternalApiException externalApiException:
                HandleExternalApiException(context, externalApiException);
                break;

            case HttpRequestException httpRequestException:
                HandleHttpRequestException(context, httpRequestException);
                break;

            case KeyNotFoundException keyNotFoundException:
                HandleKeyNotFoundException(context, keyNotFoundException);
                break;

            case InvalidOperationException invalidOperationException:
                HandleInvalidOperationException(context, invalidOperationException);
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                HandleUnauthorizedAccessException(context, unauthorizedAccessException);
                break;

            case ArgumentException argumentException:
                HandleArgumentException(context, argumentException);
                break;

            default:
                HandleGenericException(context, exception);
                break;
        }

        context.ExceptionHandled = true;
    }

    private void HandleVehicleApiException(ExceptionContext context, VehicleApiException exception)
    {
        _logger.Error(
            "Vehicle API error from {VehicleBrand}: {StatusCode} - {Message}",
            exception.VehicleBrand,
            exception.StatusCode,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)exception.StatusCode,
            Title = $"{exception.VehicleBrand} API Error",
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };

        if (!string.IsNullOrWhiteSpace(exception.ResponseContent))
        {
            problemDetails.Extensions["responseContent"] = exception.ResponseContent;
        }

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = (int)exception.StatusCode
        };
    }

    private void HandleExternalApiException(ExceptionContext context, ExternalApiException exception)
    {
        _logger.Error(
            "External API error: {StatusCode} - {Message}",
            exception.StatusCode,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)exception.StatusCode,
            Title = "External API Error",
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };

        if (!string.IsNullOrWhiteSpace(exception.ResponseContent))
        {
            problemDetails.Extensions["responseContent"] = exception.ResponseContent;
        }

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = (int)exception.StatusCode
        };
    }

    private void HandleHttpRequestException(ExceptionContext context, HttpRequestException exception)
    {
        var statusCode = exception.StatusCode ?? HttpStatusCode.InternalServerError;

        _logger.Error(
            "HTTP request error: {StatusCode} - {Message}",
            statusCode,
            exception.Message);

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = "HTTP Request Error",
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        })
        {
            StatusCode = (int)statusCode
        };
    }

    private void HandleKeyNotFoundException(ExceptionContext context, KeyNotFoundException exception)
    {
        _logger.Warning("Resource not found: {Message}", exception.Message);

        context.Result = new NotFoundObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource Not Found",
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        });
    }

    private void HandleInvalidOperationException(ExceptionContext context, InvalidOperationException exception)
    {
        _logger.Warning("Invalid operation: {Message}", exception.Message);

        context.Result = new BadRequestObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid Operation",
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        });
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context, UnauthorizedAccessException exception)
    {
        _logger.Warning("Unauthorized access: {Message}", exception.Message);

        context.Result = new UnauthorizedObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = exception.Message ?? "Authentication required or credentials invalid",
            Instance = context.HttpContext.Request.Path
        });
    }

    private void HandleArgumentException(ExceptionContext context, ArgumentException exception)
    {
        _logger.Warning("Invalid argument: {Message}", exception.Message);

        context.Result = new BadRequestObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid Argument",
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        });
    }

    private void HandleGenericException(ExceptionContext context, Exception exception)
    {
        _logger.Error(exception, "An unexpected error occurred");

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An error occurred while processing the request",
            Instance = context.HttpContext.Request.Path
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}

