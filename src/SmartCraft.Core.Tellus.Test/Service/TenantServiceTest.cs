using Moq;
using SmartCraft.Core.Tellus.Application.Services;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Test.Service;
public class companyServiceTest
{
    public Mock<ICompanyRepository<Infrastructure.Models.Company, CompanyContext>> repositoryMock;
    CompanyService service;
    public companyServiceTest()
    {
        repositoryMock = new Mock<ICompanyRepository<Infrastructure.Models.Company, CompanyContext>>();
        service = new CompanyService(repositoryMock.Object);
    }

    [Fact]
    public async Task GetcompanyAsync_Success_Returnscompany()
    {
        // Arrange
        var company = new Infrastructure.Models.Company
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DaimlerToken = "Daim",
            ManToken = "Japp",
            ScaniaClientId = "Taragona",
            ScaniaSecretKey = "Dubbel Nougat"
        };
        repositoryMock.Setup(x => x.Get(company.Id)).ReturnsAsync(company);

        //Act
        var result = await service.GetCompanyAsync(company.Id, company.TenantId);

        //Assert
        Assert.Equal(company.Id, result?.Id);
        Assert.Equal(company.DaimlerToken, result?.DaimlerToken);
        Assert.Equal(company.ManToken, result?.ManToken);
        Assert.Equal(company.ScaniaClientId, result?.ScaniaClientId);
        Assert.Equal(company.ScaniaSecretKey, result?.ScaniaSecretKey);
        repositoryMock.Verify(x => x.Get(company.Id), Times.Once);
    }

    [Fact]
    public void GetcompanyAsync_Fails_companyNotExists()
    {
        // Arrange
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(null as Infrastructure.Models.Company);

        //Act and Assert
        Task<InvalidOperationException> exc = Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetCompanyAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task RegistercompanyAsync_Success_ReturnsGuid()
    {
        //Arrange
        var company = new Domain.Models.Company()
        {
            TenantId = Guid.NewGuid(),
        };
        repositoryMock.Setup(x => x.Add(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);

        //Act
        var result = await service.RegisterCompanyAsync(company.TenantId, company);

        //Assert
        repositoryMock.Verify(x => x.Add(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void RegistercompanyAsync_Fail_ThrowsException()
    {
        //Arrange
        var company = new Infrastructure.Models.Company()
        {
            Id = Guid.NewGuid(),
        };
        repositoryMock.Setup(x => x.Add(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>())).ThrowsAsync(new Exception());

        //Act and Assert
        Task<Exception> exc = Assert.ThrowsAsync<Exception>(async () => await service.RegisterCompanyAsync(company.Id, company.ToDomainModel()));
        repositoryMock.Verify(x => x.Add(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>()),Times.Once);
    }

    [Fact]
    public async Task UpdatecompanyAsync_Success_Updatescompany()
    {
        //Arrange
        var companyId = Guid.NewGuid();
        var company = new Domain.Models.Company()
        {
            Id = companyId,
            DaimlerToken = "Not Updated"
        };

        var updatedcompany = new Infrastructure.Models.Company()
        {
            Id = companyId,
            DaimlerToken = "Updated"
        };

        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(company.ToDataModel());
        repositoryMock.Setup(x => x.Update(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>())).ReturnsAsync(updatedcompany);

        //Act
        var result = await service.UpdateCompanyAsync(company);

        //Assert
        Assert.Equal(updatedcompany.Id, result.Id);
        Assert.Equal(updatedcompany.DaimlerToken, result.DaimlerToken);
        Assert.NotEqual(company.DaimlerToken, result.DaimlerToken);
        repositoryMock.Verify(x => x.Update(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>()), Times.Once);
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void UpdatecompanyAsync_Fails_companyNotExists()
    {
        //Arrange
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync((Infrastructure.Models.Company?)null);
        repositoryMock.Setup(x => x.Update(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>())).ThrowsAsync(new InvalidOperationException());

        //Act and Assert
        Task<InvalidOperationException> exc = Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateCompanyAsync(new Domain.Models.Company()));
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Infrastructure.Models.Company>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Deletecompany_Success_ReturnsTrue()
    {
        //Arrange
        var companyId = Guid.NewGuid();
        repositoryMock.Setup(x => x.Delete(companyId)).ReturnsAsync(true);

        //Act
        var result = await service.DeleteCompany(companyId);

        //Assert
        Assert.True(result);
        repositoryMock.Verify(x => x.Delete(companyId), Times.Once);
    }

    [Fact]
    public async Task Deletecompany_Fails_ReturnsFalse()
    {
        //Arrange
        var companyId = Guid.NewGuid();
        repositoryMock.Setup(x => x.Delete(companyId)).ReturnsAsync(false);

        //Act
        var result = await service.DeleteCompany(companyId);

        //Assert
        Assert.False(result);
        repositoryMock.Verify(x => x.Delete(companyId), Times.Once);
    }
}
