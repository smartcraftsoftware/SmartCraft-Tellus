using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Serilog;
using SmartCraft.Core.Tellus.Api.Filters;
using SmartCraft.Core.Tellus.Domain.Exceptions;
using System.Net;

namespace SmartCraft.Core.Tellus.Test.Filters;

public class GlobalExceptionFilterTest
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly GlobalExceptionFilter _filter;
    private readonly DefaultHttpContext _httpContext;
    private readonly ExceptionContext _exceptionContext;

    public GlobalExceptionFilterTest()
    {
        _mockLogger = new Mock<ILogger>();
        _mockLogger.Setup(x => x.ForContext<GlobalExceptionFilter>()).Returns(_mockLogger.Object);
        
        _filter = new GlobalExceptionFilter(_mockLogger.Object);
        _httpContext = new DefaultHttpContext();
        
        var actionContext = new ActionContext(
            _httpContext,
            new RouteData(),
            new ActionDescriptor()
        );
        
        _exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
    }

    [Fact]
    public void OnException_WithVehicleApiException_ReturnsCorrectStatusCodeAndProblemDetails()
    {
        // Arrange
        var exception = new VehicleApiException(
            vehicleBrand: "volvo",
            statusCode: HttpStatusCode.Unauthorized,
            message: "Invalid credentials",
            responseContent: "{\"error\": \"auth failed\"}"
        );
        _exceptionContext.Exception = exception;
        _httpContext.Request.Path = "/api/vehicles";

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        _exceptionContext.ExceptionHandled.Should().BeTrue();
        var result = _exceptionContext.Result as ObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(401);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(401);
        problemDetails.Title.Should().Be("volvo API Error");
        problemDetails.Detail.Should().Be("Invalid credentials");
        problemDetails.Instance.Should().Be("/api/vehicles");
        problemDetails.Extensions["responseContent"].Should().Be("{\"error\": \"auth failed\"}");
    }

    [Fact]
    public void OnException_WithVehicleApiException_404NotFound_ReturnsNotFound()
    {
        // Arrange
        var exception = new VehicleApiException(
            vehicleBrand: "scania",
            statusCode: HttpStatusCode.NotFound,
            message: "Vehicle not found"
        );
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as ObjectResult;
        result!.StatusCode.Should().Be(404);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("scania API Error");
    }

    [Fact]
    public void OnException_WithVehicleApiException_429TooManyRequests_ReturnsTooManyRequests()
    {
        // Arrange
        var exception = new VehicleApiException(
            vehicleBrand: "scania",
            statusCode: HttpStatusCode.TooManyRequests,
            message: "Rate limit exceeded",
            responseContent: "{\"message\": \"Too many requests\"}"
        );
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as ObjectResult;
        result!.StatusCode.Should().Be(429);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Status.Should().Be(429);
    }

    [Fact]
    public void OnException_WithExternalApiException_ReturnsCorrectStatusCode()
    {
        // Arrange
        var exception = new ExternalApiException(
            statusCode: HttpStatusCode.BadGateway,
            message: "External service unavailable"
        );
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as ObjectResult;
        result!.StatusCode.Should().Be(502);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("External API Error");
        problemDetails.Detail.Should().Be("External service unavailable");
    }

    [Fact]
    public void OnException_WithHttpRequestException_ReturnsCorrectStatusCode()
    {
        // Arrange
        var exception = new HttpRequestException(
            "Connection failed",
            null,
            HttpStatusCode.ServiceUnavailable
        );
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as ObjectResult;
        result!.StatusCode.Should().Be(503);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("HTTP Request Error");
    }

    [Fact]
    public void OnException_WithKeyNotFoundException_Returns404()
    {
        // Arrange
        var exception = new KeyNotFoundException("Vehicle brand not found");
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as NotFoundObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(404);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Resource Not Found");
        problemDetails.Detail.Should().Be("Vehicle brand not found");
    }

    [Fact]
    public void OnException_WithInvalidOperationException_Returns400()
    {
        // Arrange
        var exception = new InvalidOperationException("Start time cannot be after stop time");
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(400);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Invalid Operation");
        problemDetails.Detail.Should().Be("Start time cannot be after stop time");
    }

    [Fact]
    public void OnException_WithUnauthorizedAccessException_Returns401()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Invalid token");
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as UnauthorizedObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(401);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public void OnException_WithArgumentException_Returns400()
    {
        // Arrange
        var exception = new ArgumentException("Invalid Esg Report");
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as BadRequestObjectResult;
        result!.StatusCode.Should().Be(400);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Invalid Argument");
    }

    [Fact]
    public void OnException_WithGenericException_Returns500()
    {
        // Arrange
        var exception = new Exception("Unexpected error");
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result as ObjectResult;
        result!.StatusCode.Should().Be(500);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Internal Server Error");
        problemDetails.Detail.Should().Be("An error occurred while processing the request");
    }

    [Fact]
    public void OnException_AlwaysMarksExceptionAsHandled()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        _exceptionContext.ExceptionHandled.Should().BeTrue();
    }

    [Fact]
    public void OnException_LogsAllExceptions()
    {
        // Arrange
        var exception = new VehicleApiException(
            "volvo",
            HttpStatusCode.InternalServerError,
            "Test error"
        );
        _exceptionContext.Exception = exception;

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        _mockLogger.Verify(
            x => x.Error(
                It.IsAny<Exception>(),
                It.IsAny<string>()),
            Times.AtLeastOnce);
    }
}

