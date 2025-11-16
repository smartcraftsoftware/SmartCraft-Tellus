using FluentAssertions;
using SmartCraft.Core.Tellus.Domain.Exceptions;
using SmartCraft.Core.Tellus.Domain.Utility;
using System.Net;

namespace SmartCraft.Core.Tellus.Test.Utility;

public class HttpResponseHelperTest
{
    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithSuccessStatusCode_DoesNotThrow()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Success")
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("volvo");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_With200_DoesNotThrow()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act & Assert
        await response.Invoking(async r => 
            await r.EnsureSuccessOrThrowVehicleApiException("volvo"))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_With201_DoesNotThrow()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Created);

        // Act & Assert
        await response.Invoking(async r => 
            await r.EnsureSuccessOrThrowVehicleApiException("scania"))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_With204_DoesNotThrow()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act & Assert
        await response.Invoking(async r => 
            await r.EnsureSuccessOrThrowVehicleApiException("man"))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithUnauthorized_ThrowsVehicleApiException()
    {
        // Arrange
        var errorContent = "{\"error\": \"Invalid credentials\"}";
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(errorContent)
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("volvo");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        exception.Which.VehicleBrand.Should().Be("volvo");
        exception.Which.ResponseContent.Should().Be(errorContent);
        exception.Which.Message.Should().Contain("401");
        exception.Which.Message.Should().Contain("volvo");
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithNotFound_ThrowsWithCorrectStatusCode()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Vehicle not found")
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("scania");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Which.VehicleBrand.Should().Be("scania");
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithTooManyRequests_PreservesStatusCode()
    {
        // Arrange
        var errorContent = "{\"message\": \"Rate limit exceeded\"}";
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(errorContent)
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("scania");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        exception.Which.ResponseContent.Should().Be(errorContent);
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithBadRequest_ThrowsWithContent()
    {
        // Arrange
        var errorContent = "{\"error\": \"Invalid request parameters\"}";
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(errorContent)
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("man");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.ResponseContent.Should().Be(errorContent);
        exception.Which.Message.Should().Contain(errorContent);
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithEmptyContent_CreatesMessageWithReasonPhrase()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(""),
            ReasonPhrase = "Internal Server Error"
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("volvo");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.Message.Should().Contain("Internal Server Error");
        exception.Which.Message.Should().Contain("500");
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithNullContent_HandlesGracefully()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("")
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("daimler");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "volvo")]
    [InlineData(HttpStatusCode.Unauthorized, "scania")]
    [InlineData(HttpStatusCode.Forbidden, "man")]
    [InlineData(HttpStatusCode.NotFound, "daimler")]
    [InlineData(HttpStatusCode.InternalServerError, "volvo")]
    [InlineData(HttpStatusCode.BadGateway, "scania")]
    [InlineData(HttpStatusCode.ServiceUnavailable, "man")]
    public async Task EnsureSuccessOrThrowVehicleApiException_WithVariousCodes_PreservesStatusAndBrand(
        HttpStatusCode statusCode, 
        string vehicleBrand)
    {
        // Arrange
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent("Error message")
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException(vehicleBrand);

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.StatusCode.Should().Be(statusCode);
        exception.Which.VehicleBrand.Should().Be(vehicleBrand);
    }

    [Fact]
    public void GetErrorMessageFromContent_WithEmptyContent_ReturnsDefaultMessage()
    {
        // Arrange
        var content = "";
        var statusCode = HttpStatusCode.BadRequest;

        // Act
        var result = HttpResponseHelper.GetErrorMessageFromContent(content, statusCode);

        // Assert
        result.Should().Contain("400");
        result.Should().Contain("Bad");
    }

    [Fact]
    public void GetErrorMessageFromContent_WithMessageField_ExtractsMessage()
    {
        // Arrange
        var content = "{\"message\": \"Invalid credentials provided\"}";
        var statusCode = HttpStatusCode.Unauthorized;

        // Act
        var result = HttpResponseHelper.GetErrorMessageFromContent(content, statusCode);

        // Assert
        result.Should().Be("Invalid credentials provided");
    }

    [Fact]
    public void GetErrorMessageFromContent_WithErrorField_ExtractsError()
    {
        // Arrange
        var content = "{\"error\": \"Resource not found\"}";
        var statusCode = HttpStatusCode.NotFound;

        // Act
        var result = HttpResponseHelper.GetErrorMessageFromContent(content, statusCode);

        // Assert
        result.Should().Be("Resource not found");
    }

    [Fact]
    public void GetErrorMessageFromContent_WithNoStructuredError_ReturnsRawContent()
    {
        // Arrange
        var content = "Plain text error message";
        var statusCode = HttpStatusCode.InternalServerError;

        // Act
        var result = HttpResponseHelper.GetErrorMessageFromContent(content, statusCode);

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public void GetErrorMessageFromContent_WithMalformedJson_ReturnsRawContent()
    {
        // Arrange
        var content = "{malformed json";
        var statusCode = HttpStatusCode.BadRequest;

        // Act
        var result = HttpResponseHelper.GetErrorMessageFromContent(content, statusCode);

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_MessageContainsStatusCodeNumber()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("Access denied")
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("volvo");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.Message.Should().Contain("403");
    }

    [Fact]
    public async Task EnsureSuccessOrThrowVehicleApiException_MessageContainsVehicleBrand()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad request")
        };

        // Act
        var act = async () => await response.EnsureSuccessOrThrowVehicleApiException("scania");

        // Assert
        var exception = await act.Should().ThrowAsync<VehicleApiException>();
        exception.Which.Message.Should().Contain("scania");
    }
}

