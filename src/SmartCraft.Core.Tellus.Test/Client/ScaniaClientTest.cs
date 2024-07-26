﻿using Moq;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using System.Net;
using FluentAssertions;
using SmartCraft.Core.Tellus.Domain.Models;
using Moq.Protected;
using System.Text.Json;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using Serilog;

namespace SmartCraft.Core.Tellus.Test.Client;
public class ScaniaClientTest
{
    private readonly Mock<HttpMessageHandler> handlerMock;
    private readonly Mock<ILogger> loggerMock;

    public ScaniaClientTest()
    {
        handlerMock = new Mock<HttpMessageHandler>();
        loggerMock = new Mock<ILogger>();
        loggerMock.Setup(x => x.ForContext<ScaniaClient>()).Returns(loggerMock.Object);
    }

    [Fact]
    public async Task GetVehicleEvaluationReportAsync_Returns_Report()
    {
        // Arrange
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"
        };

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
            });

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("/cs/vehicle/reports/VehicleEvaluationReport/v2")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleEvaluationApiResponse
                {
                    VehicleList = [new EvaluationVehicle { Vin = "vin" }]
                }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        // Act
        var result = await client.GetVehicleEvaluationReportAsync("vin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        // Assert
        result.Should().NotBeNull().And.BeOfType<VehicleEvaluationReport>();
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Exactly(3),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetVehicleEvaluationReportAsync_Throws_HttpStatusCodeExceptions(HttpStatusCode statusCode)
    {
        // Arrange
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"
        };
        handlerMock.Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                   req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
           });
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleEvaluationApiResponse()
                { VehicleList = [new EvaluationVehicle { Vin = "vin" }] }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        //Act
        Func<Task> action = () => client.GetVehicleEvaluationReportAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
        
        //Assert
        var exception = await action.Should().ThrowAsync<HttpRequestException>();
        exception.And.Message.Should().Contain(statusCode.ToString());
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
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"

        };
        handlerMock.Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                   req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
           });

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>
            ("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("rfms4/vehicles") &&
                    req.Method == HttpMethod.Get), 
                    ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehiclesApiResponse { VehicleResponse = new VehicleResponse
                {
                    Vehicles = new ScaniaVehicle[] 
                    {
                        new ScaniaVehicle { Vin = "vin1" },
                        new ScaniaVehicle { Vin = "vin2"}
                    }
                } }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        //Act
        var result = await client.GetVehiclesAsync(tenant);

        //Assert
        result.Should().NotBeNull().
            And.BeOfType<List<Vehicle>>().
            And.HaveCount(2);
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Exactly(3), 
            ItExpr.IsAny<HttpRequestMessage>(), 
            ItExpr.IsAny<CancellationToken>() 
        );
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetVechiclesAsync_Throws_HttpRequestException(HttpStatusCode statusCode)
    {
        //Arrange
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"
        };
        handlerMock.Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                   req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
           });

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("rfms4/vehicles") &&
                    req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new List<Vehicle> { new Vehicle() { Vin = "vin1" }, new Vehicle() { Vin = "vin2" } }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        //Act
        var result = () => client.GetVehiclesAsync(tenant);
        
        //Assert
        var exception = await result.Should().ThrowAsync<HttpRequestException>();
        exception.And.Message.Should().Contain(statusCode.ToString());
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync",
            Times.Exactly(3),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetVehicleStatusAsync_Returns_StatusReport()
    {
        //Arrange
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"
        };
        VehicleStatus vehicleStatus = new VehicleStatus()
        {
            Vin = "thisisavin",
        };
        handlerMock.Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                   req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
           });

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("rfms4/vehiclestatuses") &&
                    req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleStatusResponse
                {
                    VehicleStatusResponse = new ScaniaVehicleStatus
                    {
                        VehicleStatuses =
                        [
                            new VehicleStatus
                            {
                                Vin = "thisisavin",
                                CreatedDateTime = DateTime.UtcNow
                            }
                        ]
                    }
                }))
            });
        
        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        //Act
        var result = await client.GetVehicleStatusAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        //Assert
        result.Should().NotBeNull().
            And.BeOfType<IntervalStatusReport>().
            Which.Vin.Should().Be("thisisavin");
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync", 
            Times.Exactly(3), 
            ItExpr.IsAny<HttpRequestMessage>(), 
            ItExpr.IsAny<CancellationToken>() 
        );
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetVehicleStatusAsync_Throws_HttpStatusCodeExceptions(HttpStatusCode statusCode)
    {
        //Arrange
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"
        };
        VehicleStatus vehicleStatus = new VehicleStatus()
        {
            Vin = "thisisavin",
        };
        handlerMock.Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                   req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
           });

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleStatusResponse
                {
                    VehicleStatusResponse = new ScaniaVehicleStatus
                    {
                        VehicleStatuses =
                        [
                            new VehicleStatus
                            {
                                Vin = "thisisavin"
                            }
                        ]
                    }
        
                }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        //Act
        var action = () => client.GetVehicleStatusAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        //Assert
        var exception = await action.Should().ThrowAsync<HttpRequestException>();
        exception.And.Message.Should().Contain(statusCode.ToString());
        handlerMock.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync", 
            Times.Once(), 
            ItExpr.IsAny<HttpRequestMessage>(), 
            ItExpr.IsAny<CancellationToken>() 
        );
    }

    [Fact]
    public async Task GetVehicleStatusAsync_MissingToken_ThrowsException()
    {
        //Arrange
        Tenant tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            ScaniaClientId = "1234",
            ScaniaSecretKey = "5678"
        };
        VehicleStatus vehicleStatus = new VehicleStatus()
        {
            Vin = "thisisavin",
        };
        handlerMock.Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/clientid2challenge") &&
                   req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.BadRequest,
               Content = new StringContent("{\"challenge\": \"fake-challenge\"}")
           });

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("auth/response2token") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"token\": \"fake-token\"}")
            });
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>
            ("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("rfms4/vehicles") &&
                    req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new ScaniaVehicleStatusResponse
                {
                    VehicleStatusResponse = new ScaniaVehicleStatus
                    {
                        VehicleStatuses =
                        [
                            new VehicleStatus
                            {
                                Vin = "thisisavin"
                            }
                        ]
                    }


                }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);

        //Act
        var action = async () => await client.GetVehicleStatusAsync("thisisavin", tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
        
        //Assert
        var exception = await action.Should().ThrowAsync<HttpRequestException>();
        exception.And.Message.Should().Contain(HttpStatusCode.BadRequest.ToString());
    }

    private ScaniaClient CreateClient(HttpClient client)
    {
        return new ScaniaClient(client, loggerMock.Object);
    }
}

