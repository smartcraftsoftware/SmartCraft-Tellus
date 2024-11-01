using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Api.Mappers;

public static class VehicleMapper
{
    public static GetVehicleResponse ToVehicleResponse(this Vehicle vehicle)
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
            GearBoxType = vehicle.GearBoxType,
            NoOfAxles = vehicle.NoOfAxles,
            ProductionDate = vehicle?.ProductionDate?.ToResponse()
        };
    }

    public static ProductionDateResponse ToResponse(this VehicleProductionDate productionDate)
        => new ProductionDateResponse
        {
            Day = productionDate.Day,
            Month = productionDate.Month,
            Year = productionDate.Year,
        };
}
