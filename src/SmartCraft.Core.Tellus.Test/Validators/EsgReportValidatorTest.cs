using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Validators;

namespace SmartCraft.Core.Tellus.Test.Validators;
public class EsgReportValidatorTest
{
    private readonly EsgReportValidator _esgReportValidator;
    public EsgReportValidatorTest()
    {
        _esgReportValidator = new EsgReportValidator();
    }
    public static TheoryData<List<EsgVehicleReport>> EsgReportCases = new TheoryData<List<EsgVehicleReport>>
    {
        new List<EsgVehicleReport>
        {
            new EsgVehicleReport
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Now.AddDays(-3),
                StopTime = DateTime.Now.Date,
                VehicleEvaluations = new List<VehicleEvaluation>
                {
                    new VehicleEvaluation
                    {
                        Id = Guid.NewGuid(),
                        AvgElectricEnergyConsumption = 10,
                        AvgFuelConsumption = 10,
                        AvgSpeed = 10,
                        Co2Emissions = 10,
                        Co2Saved = null,
                        EngineRunningTime = "10:22:51",
                        FuelConsumptionPerHour = null,
                        TotalGasUsed = 10,
                    },
                    new VehicleEvaluation
                    {
                        Id = Guid.NewGuid(),
                        AvgElectricEnergyConsumption = null,
                        AvgFuelConsumption = null,
                        AvgSpeed = null,
                        Co2Emissions = null,
                        Co2Saved = null,
                        EngineRunningTime = null,
                        FuelConsumptionPerHour = null,
                        TotalGasUsed = null,
                        TotalDistance = null,
                        TotalEngineTime = null,
                        TotalFuelConsumption = null,
                        Vin = null
                    }
                }
            },
            new EsgVehicleReport
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Now.AddMonths(-3),
                StopTime = DateTime.Now.Date,
                VehicleEvaluations = new List<VehicleEvaluation>()
            }
        },

    };

    [Theory, MemberData(nameof(EsgReportCases))]
    public void CreateEsgReport_ShouldPass(List<EsgVehicleReport> esgVehicleReports)
    {       
        foreach(var vehicleReport in esgVehicleReports)
        {
            //Act
            var validatedResult = _esgReportValidator.Validate(vehicleReport);

            //Assert
            Assert.True(validatedResult.IsValid);
        }
    }

    [Theory]
    [InlineData("102251")]
    [InlineData("10:22:511")]
    [InlineData("10:22:51:1")]
    [InlineData("10:22:51:")]
    [InlineData("10:222:51")]
    [InlineData("100:22:51")]
    public void CreateEsgReport_InvalidEngineRunningTime_ShouldFail(string engineRunningTime)
    {
        //Arrange
        var report = new EsgVehicleReport
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.Now.AddDays(-3),
            StopTime = DateTime.Now.Date,
            VehicleEvaluations = new List<VehicleEvaluation>
            {
                new VehicleEvaluation
                {
                    EngineRunningTime = engineRunningTime
                }
            }
        };

        //Act
        var validatedResult = _esgReportValidator.Validate(report);

        //Assert
        Assert.False(validatedResult.IsValid);
    }
}
