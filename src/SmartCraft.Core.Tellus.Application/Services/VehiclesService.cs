using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Application.Services;

public class VehiclesService : IVehiclesService
{
    private Dictionary<string, IVehicleClient> clientDictionary = new Dictionary<string, IVehicleClient>();
    private readonly IRepository<Infrastructure.Models.Vehicle, VehicleContext> _repository;
    private readonly IRepository<Infrastructure.Models.IntervalStatusReport, VehicleContext> _intervalStatusRepository;
    private readonly IEnumerable<IVehicleClient> _clients;
    public VehiclesService(IRepository<Infrastructure.Models.Vehicle, VehicleContext> repository, IRepository<Infrastructure.Models.IntervalStatusReport, VehicleContext> intervalStatusRepository, IEnumerable<IVehicleClient> clients) 
    {
        _repository = repository;
        _intervalStatusRepository = intervalStatusRepository;
        _clients = clients;
        Execute();
    }
    public void Execute()
    {
        clientDictionary = _clients.ToDictionary(x => x.VehicleBrand, x => x);
    }

    public async Task<Domain.Models.StatusReport> GetVehicleStatusAsync(string vehicleBrand, string vinNumber, Tenant tenant, DateTime startTime, DateTime stopTime = default)
    {
        if(stopTime == default)
            stopTime = DateTime.Now;

        if(!MatchDateTimeValue(startTime, stopTime))
            throw new ArgumentException("Invalid date time values");

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        return await clientDictionary[vehicleBrand.ToLower()].GetVehicleStatusAsync(vinNumber, tenant, startTime, stopTime);
    }

    public async Task<List<Vehicle>> GetFleetAsync(string vehicleBrand, Tenant tenant)
    {
        if (!MatchKeyvalue(vehicleBrand.ToLower()))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicles = await clientDictionary[vehicleBrand.ToLower()].GetVehiclesAsync(tenant);
        return vehicles;
    }

    private bool MatchDateTimeValue(DateTime startTime, DateTime stopTime)
    {
        if (startTime > stopTime)
            return false;
        if (startTime > DateTime.Now)
            return false;
        if (stopTime > DateTime.Now)
            return false;
        if (startTime < DateTime.Now.AddMonths(-3))
            return false;

        return true;
    }

    private bool MatchKeyvalue(string vehicleBrand)
    {
        return clientDictionary.ContainsKey(vehicleBrand);
    }

    public async Task<IntervalStatusReport> GetIntervalVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        if(stopTime == default)
            stopTime = DateTime.Now;

        if(!MatchDateTimeValue(startTime, stopTime))
            throw new ArgumentException("Invalid date time values");

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicleIntervalStatus = await clientDictionary[vehicleBrand.ToLower()].GetIntervalStatusReportAsync(vin, tenant, startTime, stopTime);

        await _intervalStatusRepository.Add(vehicleIntervalStatus.ToDataModel(), tenant.Id);

        return vehicleIntervalStatus;
    }
};

