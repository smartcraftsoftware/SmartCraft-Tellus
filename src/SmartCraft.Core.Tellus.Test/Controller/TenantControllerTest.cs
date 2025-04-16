using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Controllers;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Services;
using Serilog;
using Shouldly;

namespace SmartCraft.Core.Tellus.Test.Controller;
public class TenantControllerTest
{
    private readonly Mock<ILogger> loggerMock = new();
    private readonly Mock<ITenantService> tenantServiceMock = new();

    [Fact]
    public async Task Get_Tenant_ReturnsTenant()
    {
        //Arrange
        var controller = CreateController();
        var id = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = id,
            VolvoCredentials = "okokok"
        };
        tenantServiceMock.Setup(x => x.GetTenantAsync(id)).ReturnsAsync(tenant);

        //Act
        var result = await controller.Get(id);

        //Assert
        Assert.IsType<ActionResult<GetTenantResponse>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Get_Tenant_MissingTenant()
    {
        //Arrange
        var controller = CreateController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ReturnsAsync(null as Tenant);

        //Act
        var result = await controller.Get(Guid.NewGuid());

        //Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Get_Tenant_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        tenantServiceMock.Setup(x => x.GetTenantAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => controller.Get(Guid.NewGuid()));

        // Verify
        tenantServiceMock.Verify(service => service.GetTenantAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Post_Tenant_CreatesTenant()
    {
        //Arrange
        var controller = CreateController();
        var tenantRequest = new AddTenantRequest
        {
            VolvoCredentials = "okokok"
        };
        tenantServiceMock.Setup(x => x.RegisterTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>())).ReturnsAsync(Guid.NewGuid());

        //Act
        var result = await controller.Post(Guid.NewGuid(), tenantRequest);

        //Assert
        Assert.IsType<ActionResult<Guid>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.RegisterTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Post_Tenant_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        tenantServiceMock.Setup(x => x.RegisterTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>())).ThrowsAsync(new Exception());


        // Act & Assert
        await Should.ThrowAsync<Exception>(() => controller.Post(Guid.NewGuid(), new AddTenantRequest()));

        // Verify
        tenantServiceMock.Verify(service => service.RegisterTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Patch_Tenant_UpdatesTenant()
    {
        //Arrange
        var controller = CreateController();
        var id = Guid.NewGuid();
        var tenantRequest = new UpdateTenantRequest
        {
            VolvoCredentials = "okokok"
        };
        tenantServiceMock.Setup(x => x.UpdateTenantAsync(id, It.IsAny<Tenant>())).ReturnsAsync(new Tenant { Id = id});

        //Act
        var result = await controller.Patch(id, tenantRequest);

        //Assert
        Assert.IsType<OkObjectResult>(result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.UpdateTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task Patch_Tenant_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        tenantServiceMock.Setup(x => x.UpdateTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>())).ThrowsAsync(new Exception());

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => controller.Patch(Guid.NewGuid(), new UpdateTenantRequest()));

        // Verify
        tenantServiceMock.Verify(service => service.UpdateTenantAsync(It.IsAny<Guid>(), It.IsAny<Tenant>()), Times.Once);

    }

    [Fact]
    public async Task Delete_Tenant_DeletesTenant()
    {
        //Arrange
        var controller = CreateController();
        var id = Guid.NewGuid();
        tenantServiceMock.Setup(x => x.DeleteTenant(id)).ReturnsAsync(true);

        //Act
        var result = await controller.Delete(id);

        //Assert
        Assert.IsType<NoContentResult>(result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.DeleteTenant(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Tenant_MissingTenant()
    {
        //Arrange
        var controller = CreateController();
        tenantServiceMock.Setup(x => x.DeleteTenant(It.IsAny<Guid>())).ReturnsAsync(false);

        //Act
        var result = await controller.Delete(Guid.NewGuid());

        //Assert
        Assert.IsType<NotFoundResult>(result);
        tenantServiceMock.Verify(tenantServiceMock => tenantServiceMock.DeleteTenant(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Tenant_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        tenantServiceMock.Setup(x => x.DeleteTenant(It.IsAny<Guid>())).ThrowsAsync(new Exception());

// Act & Assert
await Should.ThrowAsync<Exception>(() => controller.Delete(Guid.NewGuid()));

// Verify
tenantServiceMock.Verify(service => service.DeleteTenant(It.IsAny<Guid>()), Times.Once);
    }



    private TenantsController CreateController()
    {
        return new TenantsController(loggerMock.Object, tenantServiceMock.Object);
    }
}
