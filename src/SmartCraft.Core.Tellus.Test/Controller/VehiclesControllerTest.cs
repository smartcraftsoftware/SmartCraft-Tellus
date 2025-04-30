using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Controllers;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Services;

namespace SmartCraft.Core.Tellus.Test.Controller;
public class VehiclesControllerTest
{
    private readonly Mock<Serilog.ILogger> loggerMock = new();
    private readonly Mock<IVehiclesService> vehicleServiceMock = new();
    private readonly Mock<IEsgService> esgServiceMock = new();
    private readonly Mock<ICompanyService> tenantServiceMock = new();

    [Fact]
    public async Task Get_Vehicles_ReturnsListOfVehicles()
    {
        // Arrange
        var controller = CreateVehiclesController();
        var tenant = new Company
        {
            Id = Guid.NewGuid(),
            Name = "test",
            TenantId = Guid.NewGuid(),
            VolvoCredentials = ""
        };
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(tenant);
        vehicleServiceMock.Setup(x => x.GetVehiclesAsync(It.IsAny<string>(), null,It.IsAny<Company>())).ReturnsAsync(new List<Vehicle> { new Vehicle(), new Vehicle() });

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", null ,Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.IsType<ActionResult<List<GetVehicleResponse>>>(result);
        Assert.NotNull((result?.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        vehicleServiceMock.Verify(vehicleServiceMock => vehicleServiceMock.GetVehiclesAsync(It.IsAny<string>(), null, It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task Get_Vehicles_MissingTenant()
    {   // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(null as Company);
        esgServiceMock.Setup(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", null, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Get_Vehicles_ThrowsException()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company() { Name = "test" });
        vehicleServiceMock.Setup(x => x.GetVehiclesAsync(It.IsAny<string>(), null, It.IsAny<Company>())).ThrowsAsync(new Exception());

        // Act
        await Should.ThrowAsync<Exception>(async () =>  await controller.GetVehiclesAsync("Volvo", null, Guid.NewGuid(), Guid.NewGuid()));

        // Assert
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        vehicleServiceMock.Verify(vehicleServiceMock => vehicleServiceMock.GetVehiclesAsync(It.IsAny<string>(), null, It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_ReturnsReport()
    {
        //Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company() { Name = "test"});
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new EsgVehicleReport 
        { 
            VehicleEvaluations = new() 
        });

        //Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid());

        //Assert
        Assert.IsType<ActionResult<VehicleEvaluationReportResponse>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_MissingTenant()
    {   
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(null as Company);
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
        .ReturnsAsync(new EsgVehicleReport
        {
            VehicleEvaluations = new()
        });

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Get_EsgReport_VehicleNotFound()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company() { Name = "test" });
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(null as EsgVehicleReport);

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_ThrowsException()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company() { Name = "test" });
        esgServiceMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

        // Act
        await Should.ThrowAsync<Exception>(async ()  =>  await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid()));

        // Assert

        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        esgServiceMock.Verify(esgServiceMock => esgServiceMock.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    private VehiclesController CreateVehiclesController()
    {
        return new VehiclesController(loggerMock.Object, vehicleServiceMock.Object, esgServiceMock.Object, tenantServiceMock.Object);
    }
}
