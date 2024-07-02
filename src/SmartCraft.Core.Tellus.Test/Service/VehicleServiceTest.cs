using Moq;
using SmartCraft.Core.Tellus.Application.Services;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using System.Text.Json;

namespace SmartCraft.Core.Tellus.Test.Service;

public class VehicleServiceTest
{
    private readonly Mock<IVehicleClient> _vehicleClientMock;
    private readonly Mock<IRepository<Infrastructure.Models.Vehicle, VehicleContext>> _vehiclesRepositoryMock;
    private readonly Mock<IRepository<Infrastructure.Models.IntervalStatusReport, VehicleContext>> _statusRepositoryMock;
    public VehicleServiceTest()
    {
        _vehicleClientMock = new Mock<IVehicleClient>();
        _vehiclesRepositoryMock = new Mock<IRepository<Infrastructure.Models.Vehicle, VehicleContext>>();
        _statusRepositoryMock = new Mock<IRepository<Infrastructure.Models.IntervalStatusReport, VehicleContext>>();
    }

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public async Task GetVehicleStatusAsync_Returns_StatusReport(string vehicleBrand)
    {
        // Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        var vin = "thisisvin";
        _vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        _vehicleClientMock.Setup(x => x.GetVehicleStatusAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>()))
          .ReturnsAsync(new StatusReport() { Vin = vin });
        VehiclesService _vehiclesService = new VehiclesService(_vehiclesRepositoryMock.Object, _statusRepositoryMock.Object, new List<IVehicleClient> { _vehicleClientMock.Object });

        // Act
        var result = await _vehiclesService.GetVehicleStatusAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(vin, result.Vin);
    }

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public void GetVehicleStatusAsync_Returns_UnsuccessfulResponseStatusCode(string vehicleBrand)
    {
        // Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        _vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        _vehicleClientMock.Setup(x => x.GetVehicleStatusAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new HttpRequestException());
        VehiclesService _vehiclesService = new VehiclesService(_vehiclesRepositoryMock.Object, _statusRepositoryMock.Object, new List<IVehicleClient> { _vehicleClientMock.Object });
       
        // Assert and Act
        Task<HttpRequestException> task = Assert.ThrowsAsync<HttpRequestException>(() => _vehiclesService.GetVehicleStatusAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString()));
    }

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public void GetVechicleStatusAsync_Throws_Json_Exception(string vehicleBrand)
    {
        // Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        _vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        _vehicleClientMock.Setup(x => x.GetVehicleStatusAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new JsonException());
        VehiclesService _vehiclesService = new VehiclesService(_vehiclesRepositoryMock.Object, _statusRepositoryMock.Object, new List<IVehicleClient> { _vehicleClientMock.Object });


        // Assert
        Task<JsonException> task = Assert.ThrowsAsync<JsonException>(() => _vehiclesService.GetVehicleStatusAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString()));
    }

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public async Task GetFleetAsync_Returns_ListOfVehicles(string vehicleBrand)
    {
        // Arrange
        Tenant tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        _vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        _vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        _vehicleClientMock.Setup(x => x.GetVehiclesAsync(It.IsAny<Tenant>()))
                  .ReturnsAsync([new() { Vin = "thisisvin" }]);
        VehiclesService _vehiclesService = new VehiclesService(_vehiclesRepositoryMock.Object, _statusRepositoryMock.Object, new List<IVehicleClient> { _vehicleClientMock.Object });


        // Act
        var result = await _vehiclesService.GetFleetAsync(vehicleBrand, tenant);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("thisisvin", result.First().Vin);
    }
}
