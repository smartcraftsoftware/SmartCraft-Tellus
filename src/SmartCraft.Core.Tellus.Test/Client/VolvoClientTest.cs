using Moq.Protected;
using Moq;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using System.Net;
using System.Text.Json;
using SmartCraft.Core.Tellus.Domain.Models;
using FluentAssertions;

namespace SmartCraft.Core.Tellus.Test.Client;
public class VolvoClientTest
{
    private readonly Mock<HttpMessageHandler> handlerMock;
    public VolvoClientTest()
    {
        handlerMock = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public async Task GetEsgReportAsync_Returns_Report()
    {
        // Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = "credentials"
        };
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new VolvoUtilizationScoreApiResponse()
                {
                    VuScoreResponse = new Infrastructure.ApiResponse.VolvoUtilizationScore()
                    {
                        StartTime = "2021-01-01",
                        StopTime = "2021-01-02",
                        Fleet = new FleetResponse(),
                        Vehicles = new UtilizationVehicle[1]
                        {
                            new UtilizationVehicle()
                            {
                                Vin = "thisisavin"
                            }
                        }
                    }
                }
                ))
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateVolvoClient(httpClient);

        // Act
        var result = await client.GetEsgReportAsync("vin", tenant, DateTime.UtcNow, DateTime.UtcNow);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EsgVehicleReport>();
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetEsgReport_Throws_HttpRequestException(HttpStatusCode statusCode)
    {
        // Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = "credentials"
        };
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new VolvoUtilizationScoreApiResponse()
                {
                    VuScoreResponse = new Infrastructure.ApiResponse.VolvoUtilizationScore()
                    {
                        StartTime = "2021-01-01",
                        StopTime = "2021-01-02",
                        Fleet = new FleetResponse(),
                        Vehicles =
                        [
                            new UtilizationVehicle()
                            {
                                Vin = "thisisavin"
                            }
                        ]
                    }
                }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateVolvoClient(httpClient);

        //Act
        var action = () => client.GetEsgReportAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        //Assert
        var exception = await action.Should().ThrowAsync<HttpRequestException>();
        exception.And.StatusCode.Should().Be(statusCode);
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetVehiclesAsync_Returns_List_Of_Vehicles()
    {
        //Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = "credentials"
        };
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new VolvoVehiclesApiResponse 
                { 
                    Vehicle = new VolvoVehicleResponse[]
                    {
                        new VolvoVehicleResponse { Vin = "vin1" },
                        new VolvoVehicleResponse { Vin = "vin2" }
                    } 
                    }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateVolvoClient(httpClient);

        //Act
        var result = await client.GetVehiclesAsync(tenant);

        //Assert
        result.Should().NotBeNull()
            .And.BeOfType<List<Vehicle>>()
            .Which.Count.Should().Be(2);
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetVehiclesAsync_Throws_HttpRequestException(HttpStatusCode statusCode)
    {
        //Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = "credentials"
        };
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new List<Vehicle> { new Vehicle() { Vin = "vin1" }, new Vehicle() { Vin = "vin2" } }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateVolvoClient(httpClient);

        //Act
        var action = () => client.GetEsgReportAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        //Assert
        var exception = await action.Should().ThrowAsync<HttpRequestException>();
        exception.And.StatusCode.Should().Be(statusCode);
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

        [Fact]
    public async Task GetVehicleStatusAsync_Returns_StatusReport()
    {
        //Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = "credentials"
        };
        VehicleStatus vehicleStatus = new VehicleStatus()
        {
            Vin = "thisisavin",
        };
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleStatusResponse
                {
                    VehicleStatuses = new VehicleStatus[]
            {
                new VehicleStatus
                {
                    Vin = "thisisavin"
                }
            }
                }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateVolvoClient(httpClient);

        //Act
        var result = await client.GetVehicleStatusAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        //Assert
        result.Should().NotBeNull()
            .And.BeOfType<StatusReport>();
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetVehicleStatusAsync_Throws_HttpRequestException(HttpStatusCode statusCode)
    {
        //Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = "credentials"
        };
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleStatusResponse
                {
                    VehicleStatuses = new VehicleStatus[]
                    {
                        new VehicleStatus
                        {
                            Vin = "thisisavin"
                        }
                    }
                }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateVolvoClient(httpClient);

        //Act
        var action = () => client.GetVehicleStatusAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        //Assert
        var exception = await action.Should().ThrowAsync<HttpRequestException>();
        exception.And.StatusCode.Should().Be(statusCode);
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private VolvoClient CreateVolvoClient(HttpClient httpClient)
    {
        return new VolvoClient(httpClient);
    }
}
