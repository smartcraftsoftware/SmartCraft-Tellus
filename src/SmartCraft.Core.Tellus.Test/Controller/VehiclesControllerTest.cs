using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Controllers;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Services;

namespace SmartCraft.Core.Tellus.Test.Controller;
public class VehiclesControllerTest
{
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<VehiclesController>> loggerMock = new();
    private readonly Mock<IVehiclesService> vehicleServiceMock = new();
    private readonly Mock<IEsgService> esgServiceMock = new();
    private readonly Mock<ITenantService> tenantServiceMock = new();

    [Fact]
    public async Task Get_Vehicles_ReturnsListOfVehicles()
    {
        // Arrange
        var controller = CreateVehiclesController();
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            VolvoCredentials = ""
        };
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(tenant);
        vehicleServiceMock.Setup(x => x.GetVehiclesAsync(It.IsAny<string>(), null,It.IsAny<Tenant>())).ReturnsAsync(new List<Vehicle> { new Vehicle(), new Vehicle() });

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", null ,Guid.NewGuid());

        // Assert
        Assert.IsType<ActionResult<List<GetVehicleResponse>>>(result);
        Assert.NotNull((result?.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        vehicleServiceMock.Verify(vehicleServiceMock => vehicleServiceMock.GetVehiclesAsync(It.IsAny<string>(), null, It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Get_Vehicles_MissingTenant()
    {   // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(null as Tenant);
        esgServiceMock.Setup(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", null, Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Get_Vehicles_ThrowsException()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        vehicleServiceMock.Setup(x => x.GetVehiclesAsync(It.IsAny<string>(), null, It.IsAny<Tenant>())).ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", null, Guid.NewGuid());

        // Assert
        Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, (result.Result as ObjectResult)?.StatusCode);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        vehicleServiceMock.Verify(vehicleServiceMock => vehicleServiceMock.GetVehiclesAsync(It.IsAny<string>(), null, It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_ReturnsReport()
    {
        //Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new EsgVehicleReport 
        { 
            VehicleEvaluations = new() 
        });

        //Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        //Assert
        Assert.IsType<ActionResult<VehicleEvaluationReportResponse>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_MissingTenant()
    {   // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(null as Tenant);
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new EsgVehicleReport
            {
            VehicleEvaluations = new()
        });

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Get_EsgReport_VehicleNotFound()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(null as EsgVehicleReport);

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_ThrowsException()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        // Assert
        Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, (result.Result as ObjectResult)?.StatusCode);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    private VehiclesController CreateVehiclesController()
    {
        return new VehiclesController(loggerMock.Object, vehicleServiceMock.Object, esgServiceMock.Object, tenantServiceMock.Object);
    }
}
