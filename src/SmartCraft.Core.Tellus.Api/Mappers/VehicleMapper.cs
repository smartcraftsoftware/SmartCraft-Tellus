using SmartCraft.Core.Tellus.Api.Contracts.Responses;

namespace SmartCraft.Core.Tellus.Api.Mappers;

public static class VehicleMapper
{
    public static GetVehicleResponse ToVehicleResponse(this Domain.Models.Vehicle vehicle)
    {
        return new GetVehicleResponse()
        {
            Vin = vehicle.Vin,
            ExternalId = vehicle.ExternalId,
            CustomerVehicleName = vehicle.CustomerVehicleName,
            RegistrationNumber = vehicle.RegistrationNumber,
            Brand = vehicle.Brand,
            PossibleFuelTypes = vehicle.PossibleFuelTypes,
            EmissionLevel = vehicle.EmissionLevel,
            TotalFuelTankVolume = vehicle.TotalFuelTankVolume,
            TotalFuelTankCapacityGaseous = vehicle.TotalFuelTankCapacityGaseous,
            TotalBatteryPackCapacity = vehicle.TotalBatteryPackCapacity,
        };
    }
}
