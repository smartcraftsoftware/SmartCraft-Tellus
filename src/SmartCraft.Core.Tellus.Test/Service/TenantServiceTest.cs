using Moq;
using SmartCraft.Core.Tellus.Application.Services;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Test.Service;
public class TenantServiceTest
{
    public Mock<IRepository<Infrastructure.Models.Tenant, TenantContext>> repositoryMock;
    TenantService service;
    public TenantServiceTest()
    {
        repositoryMock = new Mock<IRepository<Infrastructure.Models.Tenant, TenantContext>>();
        service = new TenantService(repositoryMock.Object);
    }

    [Fact]
    public async Task GetTenantAsync_Success_ReturnsTenant()
    {
        // Arrange
        var tenant = new Infrastructure.Models.Tenant
        {
            Id = Guid.NewGuid(),
            DaimlerToken = "Daim",
            ManToken = "Japp",
            ScaniaClientId = "Taragona",
            ScaniaSecretKey = "Dubbel Nougat"
        };
        repositoryMock.Setup(x => x.Get(tenant.Id)).ReturnsAsync(tenant);

        //Act
        var result = await service.GetTenantAsync(tenant.Id);

        //Assert
        Assert.Equal(tenant.Id, result?.Id);
        Assert.Equal(tenant.DaimlerToken, result?.DaimlerToken);
        Assert.Equal(tenant.ManToken, result?.ManToken);
        Assert.Equal(tenant.ScaniaClientId, result?.ScaniaClientId);
        Assert.Equal(tenant.ScaniaSecretKey, result?.ScaniaSecretKey);
        repositoryMock.Verify(x => x.Get(tenant.Id), Times.Once);
    }

    [Fact]
    public void GetTenantAsync_Fails_TenantNotExists()
    {
        // Arrange
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(null as Infrastructure.Models.Tenant);

        //Act and Assert
        Task<NullReferenceException> exc = Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetTenantAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task RegisterTenantAsync_Success_ReturnsGuid()
    {
        //Arrange
        var tenant = new Infrastructure.Models.Tenant()
        {
            Id = Guid.NewGuid(),
        };
        repositoryMock.Setup(x => x.Add(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);

        //Act
        var result = await service.RegisterTenantAsync(tenant.Id, tenant.ToDomainModel());

        //Assert
        Assert.Equal(tenant.Id, result);
        repositoryMock.Verify(x => x.Add(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void RegisterTenantAsync_Fail_ThrowsException()
    {
        //Arrange
        var tenant = new Infrastructure.Models.Tenant()
        {
            Id = Guid.NewGuid(),
        };
        repositoryMock.Setup(x => x.Add(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>())).ThrowsAsync(new Exception());

        //Act and Assert
        Task<Exception> exc = Assert.ThrowsAsync<Exception>(async () => await service.RegisterTenantAsync(tenant.Id, tenant.ToDomainModel()));
        repositoryMock.Verify(x => x.Add(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>()),Times.Once);
    }

    [Fact]
    public async Task UpdateTenantAsync_Success_UpdatesTenant()
    {
        //Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new Domain.Models.Tenant()
        {
            Id = tenantId,
            DaimlerToken = "Not Updated"
        };

        var updatedTenant = new Infrastructure.Models.Tenant()
        {
            Id = tenantId,
            DaimlerToken = "Updated"
        };

        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(tenant.ToDataModel());
        repositoryMock.Setup(x => x.Update(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>())).ReturnsAsync(updatedTenant);

        //Act
        var result = await service.UpdateTenantAsync(tenantId, tenant);

        //Assert
        Assert.Equal(updatedTenant.Id, result.Id);
        Assert.Equal(updatedTenant.DaimlerToken, result.DaimlerToken);
        Assert.NotEqual(tenant.DaimlerToken, result.DaimlerToken);
        repositoryMock.Verify(x => x.Update(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>()), Times.Once);
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void UpdateTenantAsync_Fails_TenantNotExists()
    {
        //Arrange
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync((Infrastructure.Models.Tenant?)null);
        repositoryMock.Setup(x => x.Update(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

        //Act and Assert
        Task<NullReferenceException> exc = Assert.ThrowsAsync<NullReferenceException>(async () => await service.UpdateTenantAsync(Guid.NewGuid(), new Domain.Models.Tenant()));
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Infrastructure.Models.Tenant>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTenant_Success_ReturnsTrue()
    {
        //Arrange
        var tenantId = Guid.NewGuid();
        repositoryMock.Setup(x => x.Delete(tenantId)).ReturnsAsync(true);

        //Act
        var result = await service.DeleteTenant(tenantId);

        //Assert
        Assert.True(result);
        repositoryMock.Verify(x => x.Delete(tenantId), Times.Once);
    }

    [Fact]
    public async Task DeleteTenant_Fails_ReturnsFalse()
    {
        //Arrange
        var tenantId = Guid.NewGuid();
        repositoryMock.Setup(x => x.Delete(tenantId)).ReturnsAsync(false);

        //Act
        var result = await service.DeleteTenant(tenantId);

        //Assert
        Assert.False(result);
        repositoryMock.Verify(x => x.Delete(tenantId), Times.Once);
    }
}
