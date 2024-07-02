using FluentAssertions;
using Moq;
using SmartCraft.Core.Tellus.Application.Services;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using SmartCraft.Core.Tellus.Infrastructure.Context;

namespace SmartCraft.Core.Tellus.Test.Service;
public class EsgServiceTest
{
    private readonly Mock<IVehicleClient> vehicleClientMock = new();
    private readonly Mock<IRepository<Infrastructure.Models.EsgVehicleReport, VehicleContext>> vehiclesRepositoryMock = new();

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public async Task Get_EsgReportAsync_ReturnsReport(string vehicleBrand)
    {
        //Arrange
        var tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        vehicleClientMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(new EsgVehicleReport() { StartTime = DateTime.UtcNow.AddDays(-3), StopTime = DateTime.UtcNow,  VehicleEvaluations = [] });
        var esgService = CreateService();

        //Act
        var result = await esgService.GetEsgReportAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString());

        //Assert
        result.Should().NotBeNull()
           .And.Subject.As<EsgVehicleReport>()
           .VehicleEvaluations.Should().BeEmpty();
    }

    public static TheoryData<string, string, string> Cases =
    new()
    {
        { "scania",DateTime.UtcNow.AddYears(-2).ToString(), DateTime.UtcNow.ToString() },
        { "volvo", DateTime.UtcNow.ToString(), DateTime.UtcNow.AddHours(-1).ToString() },
        { "scania",DateTime.UtcNow.AddMonths(4).ToString(), DateTime.UtcNow.ToString() },
        { "volvo", DateTime.UtcNow.AddHours(1).ToString(), DateTime.UtcNow.ToString() }

    };
    [Theory, MemberData(nameof(Cases))]
    public async Task Get_EsgReport_Invalidstrings_ThrowsArgumentException(string vehicleBrand, string startTime, string stopTime)
    {
        //Arrange
        var tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        vehicleClientMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(new EsgVehicleReport() { VehicleEvaluations = [] });

        var esgService = CreateService();

        //Act
        Func<Task> act = async () => await esgService.GetEsgReportAsync(vehicleBrand, "vin", tenant, startTime, stopTime);

        //Assert
        var exception = await act.Should().ThrowAsync<InvalidOperationException>();
        vehicleClientMock.Verify(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

    }

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public async Task Get_EsgReport_Fails_ReturnsNull(string vehicleBrand) 
    {
        //Arrange
        var tenant = new Tenant
        {
            Id = Guid.NewGuid()
        };
        vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        vehicleClientMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Tenant>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync((EsgVehicleReport)null);

        var esgService = CreateService();

        //Act
        var result = await esgService.GetEsgReportAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString());

        //Assert
        result.Should().BeNull();

    }

    private EsgService CreateService()
    {
        return new EsgService(vehiclesRepositoryMock.Object, new List<IVehicleClient> { vehicleClientMock.Object });
    }
}
