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
        var tenant = new Company
        {
            Id = Guid.NewGuid()
        };
        vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        vehicleClientMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                  .ReturnsAsync(new EsgVehicleReport() { StartTime = DateTime.UtcNow.AddDays(-3), StopTime = DateTime.UtcNow,  VehicleEvaluations = [] });
        var esgService = CreateService();

        //Act
        var result = await esgService.GetEsgReportAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow, DateTime.UtcNow);

        //Assert
        result.Should().NotBeNull()
           .And.Subject.As<EsgVehicleReport>()
           .VehicleEvaluations.Should().BeEmpty();
    }

    public static TheoryData<string, DateTime, DateTime> Cases =
    new()
    {
        { "scania",DateTime.UtcNow.AddYears(-2), DateTime.UtcNow },
        { "volvo", DateTime.UtcNow, DateTime.UtcNow.AddHours(-1) },
        { "scania",DateTime.UtcNow.AddMonths(4), DateTime.UtcNow },
        { "volvo", DateTime.UtcNow.AddHours(1), DateTime.UtcNow },
        { "volvo", DateTime.Parse("2024-06-28"), DateTime.UtcNow },
        { "volvo", DateTime.UtcNow, DateTime.Parse("2024-06-28") }

    };
    [Theory, MemberData(nameof(Cases))]
    public async Task Get_EsgReport_InvalidDateTimes_ThrowsInvalidOperationException(string vehicleBrand, DateTime startTime, DateTime stopTime)
    {
        //Arrange
        var tenant = new Company
        {
            Id = Guid.NewGuid()
        };
        vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        vehicleClientMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                  .ReturnsAsync(new EsgVehicleReport() { VehicleEvaluations = [] });

        var esgService = CreateService();

        //Act
        Func<Task> act = async () => await esgService.GetEsgReportAsync(vehicleBrand, "vin", tenant, startTime, stopTime);

        //Assert
        var exception = await act.Should().ThrowAsync<InvalidOperationException>();
        vehicleClientMock.Verify(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);

    }

    [Theory]
    [InlineData("scania")]
    [InlineData("volvo")]
    public async Task Get_EsgReport_Fails_ReturnsNull(string vehicleBrand) 
    {
        //Arrange
        var tenant = new Company
        {
            Id = Guid.NewGuid()
        };
        vehicleClientMock.Setup(x => x.VehicleBrand).Returns(vehicleBrand);
        vehicleClientMock.Setup(x => x.GetEsgReportAsync(It.IsAny<string>(), It.IsAny<Company>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                  .ReturnsAsync(null as EsgVehicleReport);

        var esgService = CreateService();

        //Act
        var result = await esgService.GetEsgReportAsync(vehicleBrand, "vin", tenant, DateTime.UtcNow, DateTime.UtcNow);

        //Assert
        result.Should().BeNull();

    }

    private EsgService CreateService()
    {
        return new EsgService(vehiclesRepositoryMock.Object, new List<IVehicleClient> { vehicleClientMock.Object });
    }
}
