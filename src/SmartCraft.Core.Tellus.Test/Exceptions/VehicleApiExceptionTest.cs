using FluentAssertions;
using SmartCraft.Core.Tellus.Domain.Exceptions;
using System.Net;

namespace SmartCraft.Core.Tellus.Test.Exceptions;

public class VehicleApiExceptionTest
{
    [Fact]
    public void Constructor_WithAllParameters_SetsAllProperties()
    {
        // Arrange
        var vehicleBrand = "volvo";
        var statusCode = HttpStatusCode.Unauthorized;
        var message = "Invalid credentials";
        var responseContent = "{\"error\": \"auth failed\"}";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new VehicleApiException(
            vehicleBrand,
            statusCode,
            message,
            responseContent,
            innerException
        );

        // Assert
        exception.VehicleBrand.Should().Be(vehicleBrand);
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(message);
        exception.ResponseContent.Should().Be(responseContent);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Constructor_WithoutOptionalParameters_SetsRequiredProperties()
    {
        // Arrange
        var vehicleBrand = "scania";
        var statusCode = HttpStatusCode.NotFound;
        var message = "Vehicle not found";

        // Act
        var exception = new VehicleApiException(
            vehicleBrand,
            statusCode,
            message
        );

        // Assert
        exception.VehicleBrand.Should().Be(vehicleBrand);
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(message);
        exception.ResponseContent.Should().BeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithResponseContent_StoresResponseContent()
    {
        // Arrange
        var responseContent = "{\"error\": {\"code\": 429, \"message\": \"Rate limit exceeded\"}}";

        // Act
        var exception = new VehicleApiException(
            "scania",
            HttpStatusCode.TooManyRequests,
            "Too many requests",
            responseContent
        );

        // Assert
        exception.ResponseContent.Should().Be(responseContent);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    [InlineData(HttpStatusCode.Forbidden, 403)]
    [InlineData(HttpStatusCode.NotFound, 404)]
    [InlineData(HttpStatusCode.TooManyRequests, 429)]
    [InlineData(HttpStatusCode.InternalServerError, 500)]
    [InlineData(HttpStatusCode.BadGateway, 502)]
    [InlineData(HttpStatusCode.ServiceUnavailable, 503)]
    public void Constructor_WithVariousStatusCodes_PreservesStatusCode(HttpStatusCode statusCode, int expectedCode)
    {
        // Act
        var exception = new VehicleApiException(
            "man",
            statusCode,
            "Test error"
        );

        // Assert
        exception.StatusCode.Should().Be(statusCode);
        ((int)exception.StatusCode).Should().Be(expectedCode);
    }

    [Theory]
    [InlineData("volvo")]
    [InlineData("scania")]
    [InlineData("man")]
    [InlineData("daimler")]
    public void Constructor_WithDifferentVehicleBrands_StoresBrandCorrectly(string vehicleBrand)
    {
        // Act
        var exception = new VehicleApiException(
            vehicleBrand,
            HttpStatusCode.BadRequest,
            "Test error"
        );

        // Assert
        exception.VehicleBrand.Should().Be(vehicleBrand);
    }

    [Fact]
    public void Constructor_WithInnerException_PreservesInnerException()
    {
        // Arrange
        var innerException = new HttpRequestException("Connection failed");

        // Act
        var exception = new VehicleApiException(
            "volvo",
            HttpStatusCode.ServiceUnavailable,
            "Service unavailable",
            null,
            innerException
        );

        // Assert
        exception.InnerException.Should().BeSameAs(innerException);
    }
}

