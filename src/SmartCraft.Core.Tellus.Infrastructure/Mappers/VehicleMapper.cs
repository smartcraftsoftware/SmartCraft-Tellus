using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using System.Runtime.CompilerServices;

namespace SmartCraft.Core.Tellus.Infrastructure.Mappers;

public static class VehicleMapper
{
    #region Infra/Domain
    public static Infrastructure.Models.Vehicle ToDataModel(this Domain.Models.Vehicle vehicle)
    {
        return new Infrastructure.Models.Vehicle
        {
            Vin = vehicle.Vin,
            CustomerVehicleName = vehicle.CustomerVehicleName,
            RegistrationNumber = vehicle.RegistrationNumber,
            Brand = vehicle.Brand,
            PossibleFuelTypes = vehicle.PossibleFuelTypes,
            EmissionLevel = vehicle.EmissionLevel,
            TotalFuelTankVolume = vehicle.TotalFuelTankVolume,
            TotalFuelTankCapacityGaseous = vehicle.TotalFuelTankCapacityGaseous,
            TotalBatteryPackCapacity = vehicle.TotalBatteryPackCapacity,
            Type = vehicle.Type,
            TachographType = vehicle.TachographType,
            ProductionDate = vehicle.ProductionDate?.ToDataModel(),
            NoOfAxles = vehicle.NoOfAxles,
            EngineType = vehicle.EngineType,
            GearBoxType = vehicle.GearBoxType
        };
    }

    public static Infrastructure.Models.VehicleProductionDate ToDataModel(this Domain.Models.VehicleProductionDate vehicleProductionDate)
        => new Models.VehicleProductionDate
        {
            Day = vehicleProductionDate.Day,
            Month = vehicleProductionDate.Month,
            Year = vehicleProductionDate.Year,
        };
    #endregion

    #region Man
    public static Domain.Models.Vehicle ToDomainModel(this ManAssetApiResponse apiResponse)
    {
        return new Domain.Models.Vehicle
        {
            ExternalId = apiResponse?.Items?[0].Id,
            Vin = apiResponse?.Items?[0].IdentificationType == "Vin" ? apiResponse?.Items?[0].Identification : "",
            CustomerVehicleName = apiResponse?.Items?[0].IdentificationType,
            RegistrationNumber = apiResponse?.Items?[0].LicensePlate,
            Brand = apiResponse?.Items?[0].Brand,
            EngineType = apiResponse?.Items?[0].Embedded?.MasterData?.EngineType,
            PossibleFuelTypes = new List<string> { apiResponse?.Items?[0].Embedded?.MasterData?.FuelType ?? string.Empty },
            
        };
    }

    public static Domain.Models.StatusReport ToDomainModel(this ManTelematicsApiResponse apiResponse)
    {
        return new Domain.Models.StatusReport
        {
            ExternalId = apiResponse?.AssetId,
            HrTotalVehicleDistance = apiResponse?.Items?[0].State?.Mileage,
            SnapShotData = new Domain.Models.SnapShotData
            {
                Ignition = apiResponse?.Items?[0].State?.Ignition.ToString(),
                FuelLevel1 = apiResponse?.Items?[0].State?.FuelLevel,
                EstimatedDistanceToEmpty = new Domain.Models.DistanceToEmpty
                {
                    BatteryPack = apiResponse?.Items?[0].State?.StateOfCharge
                    
                }
            }
        };
    }
    #endregion

    #region Scania
    public static Vehicle ToDomainModel(this ScaniaVehicle vehiclesApiResponse)
    {
        return new Vehicle
        {
            Vin = vehiclesApiResponse.Vin ?? string.Empty,
            CustomerVehicleName = vehiclesApiResponse.CustomerVehicleName,
            Brand = vehiclesApiResponse.Brand,
            PossibleFuelTypes = vehiclesApiResponse.PossibleFuelTypes,
            EmissionLevel = vehiclesApiResponse.EmissionLevel,
            TotalFuelTankVolume = vehiclesApiResponse.TotalFuelTankVolume,
            NoOfAxles = vehiclesApiResponse.NoOfAxles,
            GearBoxType = vehiclesApiResponse.GearBoxType,
            ProductionDate = vehiclesApiResponse?.ProductionDate?.ToDomainModel()
        };
    }

    public static Domain.Models.VehicleProductionDate ToDomainModel(this ApiResponse.VehicleProductionDate productionDate)
        => new Domain.Models.VehicleProductionDate
        {
            Day = productionDate.Day,
            Month = productionDate.Month,
            Year = productionDate.Year
        };

    public static StatusReport ToDomainModel(this ScaniaVehicleStatusResponse statusResponse)
    {

        return new StatusReport
        {
            Vin = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].Vin,
            CreatedDateTime = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].CreatedDateTime,
            ReceivedDateTime = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].ReceivedDateTime,
            HrTotalVehicleDistance = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].HrTotalVehicleDistance,
            TotalEngineHours = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].TotalEngineHours,
            TotalElectricMotorHours = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].TotalElectricMotorHours,
            EngineTotalFuelUsed = statusResponse.VehicleStatusResponse.VehicleStatuses?[0].EngineTotalFuelUsed,
        };
    }

    public static IntervalStatusReport ToIntervalDomainModel(this ScaniaVehicleStatusResponse statusResponse)
    {
        // sort statusResponse VehicleStatus by received date, just in case
        var vehicleStatuses = statusResponse?.VehicleStatusResponse.VehicleStatuses.OrderBy(x => x.CreatedDateTime).ToArray();
        if (vehicleStatuses?.Length == 0)
            throw new InvalidOperationException("Statusresponse is empty");

        var vin = vehicleStatuses?[0].Vin ?? string.Empty;

        var first = vehicleStatuses?[0];
        var last = vehicleStatuses?[^1];

        DateTime firstDate = first?.CreatedDateTime ?? throw new InvalidOperationException();
        DateTime lastDate = last?.CreatedDateTime ?? throw new InvalidOperationException();

        var firstTotalFuelUsed = first?.EngineTotalFuelUsed;
        var lastTotalFuelUsed = last?.EngineTotalFuelUsed;
        var totalFuelUsed = lastTotalFuelUsed - firstTotalFuelUsed;

        var firstHrTotalVehicleDistance = first?.HrTotalVehicleDistance;
        var lastHrTotalVehicleDistance = last?.HrTotalVehicleDistance;
        var totalHrTotalVehicleDistance = lastHrTotalVehicleDistance - firstHrTotalVehicleDistance;

        var firstTotalEngineHours = first?.TotalEngineHours;
        var lastTotalEngineHours = last?.TotalEngineHours;
        var totalEngineHours = lastTotalEngineHours - firstTotalEngineHours;

        var firstTotalElectricMotorHours = first?.TotalElectricMotorHours;
        var lastTotalElectricMotorHours = last?.TotalElectricMotorHours;
        var totalElectricMotorHours = lastTotalElectricMotorHours - firstTotalElectricMotorHours;

        var firstTotalGaseousFuelUsed = first?.TotalGaseousFuelUsed;
        var lastTotalGaseousFuelUsed = last?.TotalGaseousFuelUsed;
        var totalGaseousFuelUsed = lastTotalGaseousFuelUsed - firstTotalGaseousFuelUsed;

        var firstTotalElectricEnergyUsed = first?.TotalElectricEnergyUsed;
        var lastTotalElectricEnergyUsed = last?.TotalElectricEnergyUsed;
        var totalElectricEnergyUsed = lastTotalElectricEnergyUsed - firstTotalElectricEnergyUsed;

        return new IntervalStatusReport
        {
            Vin = vin,
            StartDateTime = firstDate.ToUniversalTime(),
            EndDateTime = lastDate.ToUniversalTime(),
            EngineTotalFuelUsed = totalFuelUsed,
            HrTotalVehicleDistance = totalHrTotalVehicleDistance,
            TotalEngineHours = totalEngineHours,
            TotalElectricMotorHours = totalElectricMotorHours,
            TotalGaseousFuelUsed = totalGaseousFuelUsed,
            TotalElectricEnergyUsed = totalElectricEnergyUsed
        };
    }

    #endregion

    #region Volvo
    public static Vehicle ToDomainModel(this VolvoVehicleResponse vehiclesApiResponse)
    {
        return new Vehicle
        {
            Vin = vehiclesApiResponse?.Vin ?? string.Empty,
            CustomerVehicleName = vehiclesApiResponse?.CustomerVehicleName,
            Brand = vehiclesApiResponse?.Brand,
            PossibleFuelTypes = vehiclesApiResponse?.PossibleFuelTypes,
            EmissionLevel = vehiclesApiResponse?.EmissionLevel,
            TotalFuelTankVolume = vehiclesApiResponse?.TotalFuelTankVolume,
            GearBoxType = vehiclesApiResponse?.GearboxType,
            NoOfAxles = vehiclesApiResponse?.NoOfAxles,
            ProductionDate = vehiclesApiResponse?.ProductionDate?.ToDomainModel(),
            TachographType = vehiclesApiResponse?.TachographType,
            Type = vehiclesApiResponse?.Type,
        };
    }

    public static StatusReport ToDomainModel(this VolvoVehicleStatusResponse statusResponse)
    {
        return new StatusReport
        {
            Vin = statusResponse.VehicleStatus?[0].Vin ?? string.Empty,
            CreatedDateTime = statusResponse?.VehicleStatus?[0].CreatedDateTime,
            ReceivedDateTime = statusResponse?.VehicleStatus?[0].ReceivedDateTime,
            HrTotalVehicleDistance = statusResponse?.VehicleStatus?[0].HrTotalVehicleDistance,
            TotalEngineHours = statusResponse?.VehicleStatus?[0].TotalEngineHours,
            TotalElectricMotorHours = statusResponse?.VehicleStatus?[0].TotalElectricMotorHours,
            EngineTotalFuelUsed = statusResponse?.VehicleStatus?[0].EngineTotalFuelUsed,
            TotalGaseousFuelUsed = statusResponse?.VehicleStatus?[0].TotalGaseousFuelUsed,
            TotalElectricEnergyUsed = statusResponse?.VehicleStatus?[0].TotalElectricEnergyUsed,
            AccumulatedData = statusResponse?.VehicleStatus?[0].AccumulatedData?.ToDomainModel(),
            SnapShotData = statusResponse?.VehicleStatus?[0].SnapShotData?.ToDomainModel()
        };
    }

    public static IntervalStatusReport ToIntervalDomainModel(this VolvoVehicleStatusResponse statusResponse)
    {
        // sort statusResponse VehicleStatus by received date, just in case
        var vehicleStatuses = statusResponse?.VehicleStatus?.OrderBy(x => x.CreatedDateTime).ToArray();
        if (vehicleStatuses?.Length == 0)
            return new IntervalStatusReport();

        var vin = vehicleStatuses?[0].Vin ?? string.Empty;

        var first = vehicleStatuses?.FirstOrDefault();
        var last = vehicleStatuses?.LastOrDefault();

        DateTime firstDate = first?.CreatedDateTime ?? throw new InvalidOperationException();
        DateTime lastDate = last?.CreatedDateTime ?? throw new InvalidOperationException();

        var firstTotalFuelUsed = first?.EngineTotalFuelUsed;
        var lastTotalFuelUsed = last?.EngineTotalFuelUsed;
        var totalFuelUsed = lastTotalFuelUsed - firstTotalFuelUsed;

        var firstHrTotalVehicleDistance = first?.HrTotalVehicleDistance;
        var lastHrTotalVehicleDistance = last?.HrTotalVehicleDistance;
        var totalHrTotalVehicleDistance = lastHrTotalVehicleDistance - firstHrTotalVehicleDistance;

        var firstTotalEngineHours = first?.TotalEngineHours;
        var lastTotalEngineHours = last?.TotalEngineHours;
        var totalEngineHours = lastTotalEngineHours - firstTotalEngineHours;

        var firstTotalElectricMotorHours = first?.TotalElectricMotorHours;
        var lastTotalElectricMotorHours = last?.TotalElectricMotorHours;
        var totalElectricMotorHours = lastTotalElectricMotorHours - firstTotalElectricMotorHours;

        var firstTotalGaseousFuelUsed = first?.TotalGaseousFuelUsed;
        var lastTotalGaseousFuelUsed = last?.TotalGaseousFuelUsed;
        var totalGaseousFuelUsed = lastTotalGaseousFuelUsed - firstTotalGaseousFuelUsed;

        var firstTotalElectricEnergyUsed = first?.TotalElectricEnergyUsed;
        var lastTotalElectricEnergyUsed = last?.TotalElectricEnergyUsed;
        var totalElectricEnergyUsed = lastTotalElectricEnergyUsed - firstTotalElectricEnergyUsed;

        return new IntervalStatusReport
        {
            Vin = vin,
            StartDateTime = firstDate.ToUniversalTime(),
            EndDateTime = lastDate.ToUniversalTime(),
            EngineTotalFuelUsed = totalFuelUsed,
            HrTotalVehicleDistance = totalHrTotalVehicleDistance,
            TotalEngineHours = totalEngineHours,
            TotalElectricMotorHours = totalElectricMotorHours,
            TotalGaseousFuelUsed = totalGaseousFuelUsed,
            TotalElectricEnergyUsed = totalElectricEnergyUsed
        };
    }

    public static Infrastructure.Models.IntervalStatusReport ToDataModel(this IntervalStatusReport intervalStatusReport)
    {
        return new Infrastructure.Models.IntervalStatusReport
        {
            Id = intervalStatusReport.Id,
            StartDateTime = intervalStatusReport.StartDateTime,
            EndDateTime = intervalStatusReport.EndDateTime,
            EngineTotalFuelUsed = intervalStatusReport.EngineTotalFuelUsed,
            HrTotalVehicleDistance = intervalStatusReport.HrTotalVehicleDistance,
            TotalElectricEnergyUsed = intervalStatusReport.TotalElectricEnergyUsed,
            TotalElectricMotorHours = intervalStatusReport.TotalElectricMotorHours,
            TotalEngineHours = intervalStatusReport.TotalEngineHours,
            TotalGaseousFuelUsed = intervalStatusReport.TotalGaseousFuelUsed,
            Vin = intervalStatusReport.Vin,
        };
    }

    private static Domain.Models.AccumulatedData ToDomainModel(this ApiResponse.AccumulatedData accumulatedData)
    {
        return new Domain.Models.AccumulatedData
        {
            FuelConsumptionDuringCruiseActive = accumulatedData.FuelConsumptionDuringCruiseActive,
            FuelConsumptionDuringCruiseActiveGaseous = accumulatedData.FuelConsumptionDuringCruiseActiveGaseous
        };
    }

    private static Domain.Models.SnapShotData ToDomainModel(this ApiResponse.SnapShotData snapShotData)
    {
        return new Domain.Models.SnapShotData
        {
            Ignition = snapShotData.Ignition,
            EngineSpeed = snapShotData.EngineSpeed,
            ElectricMotorSpeed = snapShotData.ElectricMotorSpeed,
            FuelType = snapShotData.FuelType,
            FuelLevel1 = snapShotData.FuelLevel1,
            FuelLevel2 = snapShotData.FuelLevel2,
            CatalystFuelLevel = snapShotData.CatalystFuelLevel,
            EstimatedDistanceToEmpty = snapShotData.EstimatedDistanceToEmpty?.ToDomainModel()
        };
    }

    private static Domain.Models.DistanceToEmpty ToDomainModel(this ApiResponse.DistanceToEmpty distanceToEmpty)
    {
        return new Domain.Models.DistanceToEmpty
        {
            Total = distanceToEmpty.Total,
            Fuel = distanceToEmpty.Fuel,
            Gas = distanceToEmpty.Gas,
            BatteryPack = distanceToEmpty.BatteryPack
        };
    }
    #endregion
}
