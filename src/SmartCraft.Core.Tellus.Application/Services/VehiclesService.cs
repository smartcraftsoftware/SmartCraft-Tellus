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

    public async Task<List<Vehicle>> GetVehiclesAsync(string vehicleBrand, string? vin, Tenant tenant)
    {
        if (!MatchKeyvalue(vehicleBrand.ToLower()))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicles = await clientDictionary[vehicleBrand.ToLower()].GetVehiclesAsync(tenant, vin);
        return vehicles;
    }

    public async Task<IntervalStatusReport> GetVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        (var start, var stop) = ParseAndMatchDateTimeValues(startTime, stopTime);

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicleIntervalStatus = await clientDictionary[vehicleBrand.ToLower()].GetVehicleStatusAsync(vin, tenant, start, stop);

        await _intervalStatusRepository.Add(vehicleIntervalStatus.ToDataModel(), tenant.Id);

        return vehicleIntervalStatus;
    }

    private (DateTime, DateTime) ParseAndMatchDateTimeValues(DateTime startTime, DateTime stopTime)
    {
        if (startTime.Kind == DateTimeKind.Unspecified)
            throw new InvalidOperationException("Start time needs to have timezone specified");
        if (stopTime.Kind == DateTimeKind.Unspecified)
            throw new InvalidOperationException("Stop time needs to have timezone specified");

        if (startTime > stopTime)
        {
            throw new InvalidOperationException("Start time cannot be after stop time");
        }

        var utcStartTime = startTime.ToUniversalTime();
        if (utcStartTime > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Start time cannot be greater than current time");
        }

        var utcStopTime = stopTime.ToUniversalTime();
        if (utcStopTime > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Stop time cannot be greater than current time");
        }

        if (utcStartTime < DateTime.UtcNow.AddMonths(-3))
        {
            throw new InvalidOperationException("Start time cannot be older than 3 months");
        }

        return (startTime, stopTime);
    }

    private bool MatchKeyvalue(string vehicleBrand)
    {
        return clientDictionary.ContainsKey(vehicleBrand);
    }
};

