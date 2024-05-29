using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;

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
            TotalBatteryPackCapacity = vehicle.TotalBatteryPackCapacity
        };
    }
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
            RegistrationNumber = vehiclesApiResponse.RegistrationNumber,
            Brand = vehiclesApiResponse.Brand,
            PossibleFuelTypes = vehiclesApiResponse.PossibleFuelTypes,
            EmissionLevel = vehiclesApiResponse.EmissionLevel,
            TotalFuelTankVolume = vehiclesApiResponse.TotalFuelTankVolume,
            TotalFuelTankCapacityGaseous = vehiclesApiResponse.TotalFuelTankCapacityGaseous,
            TotalBatteryPackCapacity = vehiclesApiResponse.TotalBatteryPackCapacity,
        };
    }

    public static StatusReport ToDomainModel(this ScaniaVehicleStatusResponse statusResponse)
    {
        DateTime createdTime;
        DateTime.TryParse(statusResponse.VehicleStatuses?[0].CreatedDateTime, out createdTime);
        DateTime receivedTime;
        DateTime.TryParse(statusResponse.VehicleStatuses?[0].ReceivedDateTime, out receivedTime);
        return new StatusReport
        {
            Vin = statusResponse.VehicleStatuses?[0].Vin,
            CreatedDateTime = createdTime,
            ReceivedDateTime = receivedTime,
            HrTotalVehicleDistance = statusResponse.VehicleStatuses?[0].HrTotalVehicleDistance,
            TotalEngineHours = statusResponse.VehicleStatuses?[0].TotalEngineHours,
            TotalElectricMotorHours = statusResponse.VehicleStatuses?[0].TotalElectricMotorHours,
            EngineTotalFuelUsed = statusResponse.VehicleStatuses?[0].EngineTotalFuelUsed,
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
            RegistrationNumber = vehiclesApiResponse?.RegistrationNumber,
            Brand = vehiclesApiResponse?.Brand,
            PossibleFuelTypes = vehiclesApiResponse?.PossibleFuelTypes,
            EmissionLevel = vehiclesApiResponse?.EmissionLevel,
            TotalFuelTankVolume = vehiclesApiResponse?.TotalFuelTankVolume,
            TotalFuelTankCapacityGaseous = vehiclesApiResponse?.TotalFuelTankCapacityGaseous,
            TotalBatteryPackCapacity = vehiclesApiResponse?.TotalBatteryPackCapacity,
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
