using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Controllers;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Services;

namespace SmartCraft.Core.Tellus.Test.Controller;
public class VehiclesControllerTest
{
    private readonly Mock<ILogger> loggerMock = new();
    private readonly Mock<IVehiclesService> vehicleServiceMock = new();
    private readonly Mock<IVehicleEvaluationService> evaluationServiceMock = new();
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
        vehicleServiceMock.Setup(x => x.GetVehiclesAsync(It.IsAny<string>(), It.IsAny<Tenant>())).ReturnsAsync(new List<Vehicle> { new Vehicle(), new Vehicle() });

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", Guid.NewGuid());

        // Assert
        Assert.IsType<ActionResult<List<GetVehicleResponse>>>(result);
        Assert.NotNull((result?.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        vehicleServiceMock.Verify(vehicleServiceMock => vehicleServiceMock.GetVehiclesAsync(It.IsAny<string>(), It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Get_Vehicles_MissingTenant()
    {   // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(null as Tenant);
        evaluationServiceMock.Setup(evaluationServiceMock => evaluationServiceMock.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));

        // Act
        var result = await controller.GetVehiclesAsync("Volvo", Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        evaluationServiceMock.Verify(evaluationServiceMock => evaluationServiceMock.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Get_Vehicles_ThrowsException()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        vehicleServiceMock.Setup(x => x.GetVehiclesAsync(It.IsAny<string>(), It.IsAny<Tenant>())).ThrowsAsync(new HttpRequestException("Error handling request"));

        // Act 
        Func<Task> act = async () => await controller.GetVehiclesAsync("Volvo", Guid.NewGuid());

        // Assert
        var exception = await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Error handling request");
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        vehicleServiceMock.Verify(vehicleServiceMock => vehicleServiceMock.GetVehiclesAsync(It.IsAny<string>(), It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_ReturnsReport()
    {
        //Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        evaluationServiceMock.Setup(x => x.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new VehicleEvaluationReport 
        { 
            VehicleEvaluations = new() 
        });

        //Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        //Assert
        Assert.IsType<ActionResult<VehicleEvaluationReportResponse>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        evaluationServiceMock.Verify(evaluationServiceMock => evaluationServiceMock.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_MissingTenant()
    {   // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(null as Tenant);
        evaluationServiceMock.Setup(x => x.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new VehicleEvaluationReport
            {
            VehicleEvaluations = new()
        });

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        evaluationServiceMock.Verify(evaluationServiceMock => evaluationServiceMock.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Get_EsgReport_VehicleNotFound()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        evaluationServiceMock.Setup(x => x.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(null as VehicleEvaluationReport);

        // Act
        var result = await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        evaluationServiceMock.Verify(evaluationServiceMock => evaluationServiceMock.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Get_EsgReport_ThrowsException()
    {
        // Arrange
        var controller = CreateVehiclesController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(new Tenant());
        evaluationServiceMock.Setup(x => x.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new HttpRequestException("Wrong!"));

        // Act
        Func<Task> act = async () => await controller.GetReportAsync("Volvo", "vin", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid());

        // Assert
        var exception = await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Wrong!");
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
        evaluationServiceMock.Verify(evaluationServiceMock => evaluationServiceMock.GetVehicleEvaluationReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    private VehiclesController CreateVehiclesController()
    {
        return new VehiclesController(loggerMock.Object, vehicleServiceMock.Object, evaluationServiceMock.Object, tenantServiceMock.Object);
    }
}
