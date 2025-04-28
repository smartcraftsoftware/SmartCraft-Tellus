using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Controllers;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Services;

namespace SmartCraft.Core.Tellus.Test.Controller;
public class CompanyControllerTest
{
    private readonly Mock<Serilog.ILogger> loggerMock = new();
    private readonly Mock<ICompanyService> companyServiceMock = new();

    [Fact]
    public async Task Get_company_Returnscompany()
    {
        //Arrange
        var controller = CreateController();
        var companyId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = "test",
            TenantId = companyId,
            VolvoCredentials = "okokok"
        };
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(company);

        //Act
        var result = await controller.Get(companyId, companyId);

        //Assert
        Assert.IsType<ActionResult<GetCompanyResponse>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Get_company_Missingcompany()
    {
        //Arrange
        var controller = CreateController();
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(null as Company);

        //Act
        var result = await controller.Get(Guid.NewGuid(), Guid.NewGuid());

        //Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Get_company_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new Exception());

        //Act & Assert
        var result = await controller.Get(Guid.NewGuid(), Guid.NewGuid()).ShouldThrowAsync<Exception>();
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Post_company_Createscompany()
    {
        //Arrange
        var controller = CreateController();
        var companyRequest = new AddCompanyRequest
        {
            Name = "test",
            VolvoCredentials = "okokok"
        };
        companyServiceMock.Setup(x => x.RegisterCompanyAsync(It.IsAny<Guid>(), It.IsAny<Company>())).ReturnsAsync(Guid.NewGuid());

        //Act
        var result = await controller.Post(Guid.NewGuid(), companyRequest);

        //Assert
        Assert.IsType<ActionResult<Guid>>(result);
        Assert.NotNull((result.Result as OkObjectResult)?.Value);
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.RegisterCompanyAsync(It.IsAny<Guid>(), It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task Post_company_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        companyServiceMock.Setup(x => x.RegisterCompanyAsync(It.IsAny<Guid>(), It.IsAny<Company>())).ThrowsAsync(new Exception());

        //Act & Assert
        var result = await controller.Post(Guid.NewGuid(), new AddCompanyRequest { Name = "test"}).ShouldThrowAsync<Exception>();
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.RegisterCompanyAsync(It.IsAny<Guid>(), It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task Patch_company_Updatescompany()
    {
        //Arrange
        var controller = CreateController();
        var tenantId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var companyRequest = new UpdateCompanyRequest
        {
            VolvoCredentials = "okokok"
        };
        companyServiceMock.Setup(x => x.UpdateCompanyAsync(It.IsAny<Company>())).ReturnsAsync(new Company { Id = companyId, Name = "test"});
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company { Id = companyId, TenantId = tenantId, Name = "test", });

        //Act
        var result = await controller.Patch(tenantId, companyId, companyRequest);

        //Assert
        Assert.IsType<OkObjectResult>(result);
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.UpdateCompanyAsync(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task Patch_company_ThrowsException()
    {
        //Arrange
        var controller = CreateController();
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company { Id = Guid.NewGuid(), Name = "test" });
        companyServiceMock.Setup(x => x.UpdateCompanyAsync(It.IsAny<Company>())).ThrowsAsync(new Exception());

        //Act & Assert
        var result = await controller.Patch(Guid.NewGuid(), Guid.NewGuid(), new UpdateCompanyRequest()).ShouldThrowAsync<Exception>();
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.UpdateCompanyAsync(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task Delete_company_Deletescompany()
    {
        //Arrange
        var controller = CreateController();
        var tenantId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        companyServiceMock.Setup(x => x.DeleteCompany(companyId)).ReturnsAsync(true);
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company { Id = companyId, TenantId = tenantId, Name = "test", });

        //Act
        var result = await controller.Delete(tenantId, companyId);

        //Assert
        Assert.IsType<NoContentResult>(result);
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.DeleteCompany(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Delete_company_Missingcompany()
    {
        //Arrange
        var controller = CreateController();
        companyServiceMock.Setup(x => x.DeleteCompany(It.IsAny<Guid>())).ReturnsAsync(false);
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Company { Id = Guid.NewGuid(), Name = "test", });

        //Act
        var result = await controller.Delete(Guid.NewGuid(), Guid.NewGuid());

        //Assert
        Assert.IsType<NotFoundResult>(result);
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.DeleteCompany(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Delete_company_ThrowsException()
    {
        //Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "test",
            TenantId = Guid.NewGuid(),
        };
        var controller = CreateController();
        companyServiceMock.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(company);
        companyServiceMock.Setup(x => x.DeleteCompany(It.IsAny<Guid>())).ThrowsAsync(new Exception());


        //Act
        await Should.ThrowAsync<Exception>(async ()  => await controller.Delete(company.TenantId, company.Id));

        //Assert
        companyServiceMock.Verify(companyServiceMock => companyServiceMock.DeleteCompany(It.IsAny<Guid>()), Times.Once);
    }



    private CompanyController CreateController()
    {
        return new CompanyController(loggerMock.Object, companyServiceMock.Object);
    }
}
