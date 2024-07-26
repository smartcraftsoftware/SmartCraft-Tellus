using Serilog;
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
    private readonly ILogger _logger;
    public VehiclesService(IRepository<Infrastructure.Models.Vehicle, VehicleContext> repository, IRepository<Infrastructure.Models.IntervalStatusReport, VehicleContext> intervalStatusRepository, IEnumerable<IVehicleClient> clients, ILogger logger) 
    {
        _logger = logger.ForContext<VehiclesService>();
        _repository = repository;
        _intervalStatusRepository = intervalStatusRepository;
        _clients = clients;
        Execute();
    } 
    public void Execute()
    {
        clientDictionary = _clients.ToDictionary(x => x.VehicleBrand, x => x);
    }

    //public async Task<Domain.Models.StatusReport> GetVehicleStatusAsync(string vehicleBrand, string vinNumber, Tenant tenant, DateTime startTime, DateTime stopTime)
    //{
    //    (var start, var stop) = ParseAndMatchDateTimeValues(startTime, stopTime);
    //
    //    if (!MatchKeyvalue(vehicleBrand))
    //        throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");
    //
    //    return await clientDictionary[vehicleBrand.ToLower()].GetVehicleStatusAsync(vinNumber, tenant, start, stop);
    //}

    public async Task<List<Vehicle>> GetVehiclesAsync(string vehicleBrand, Tenant tenant)
    {
        if (!MatchKeyvalue(vehicleBrand.ToLower()))
        {
            _logger.Warning("{Tenant} tried to access vehicle with {BrandName", tenant.Id, vehicleBrand);
            throw new InvalidOperationException($"Vehicle brand {vehicleBrand} not found");
        }

        var vehicles = await clientDictionary[vehicleBrand.ToLower()].GetVehiclesAsync(tenant);
        return vehicles;
    }

    public async Task<IntervalStatusReport> GetVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        (bool valid, string message) = ParseAndMatchDateTimeValues(startTime, stopTime);
        if (!valid)
            throw new InvalidOperationException(message);

        if (!MatchKeyvalue(vehicleBrand)) 
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");
        

        var vehicleIntervalStatus = await clientDictionary[vehicleBrand.ToLower()].GetVehicleStatusAsync(vin, tenant, startTime, stopTime);

        await _intervalStatusRepository.Add(vehicleIntervalStatus.ToDataModel(), tenant.Id);

        return vehicleIntervalStatus;
    }

    private (bool, string) ParseAndMatchDateTimeValues(DateTime startTime, DateTime stopTime)
    {
        if (startTime.Kind == DateTimeKind.Unspecified)
            return (false, "Start time needs to have timezone specified");

        if (stopTime.Kind == DateTimeKind.Unspecified)
            return (false, "Stop time needs to have timezone specified");

        if (startTime > stopTime)
        {
            return (false, "Start time cannot be after stop time");
        }

        var utcStartTime = startTime.ToUniversalTime();
        if (utcStartTime > DateTime.UtcNow)
        {
            return (false, "Start time cannot be greater than current time");
        }

        var utcStopTime = stopTime.ToUniversalTime();
        if (utcStopTime > DateTime.UtcNow)
        {
            return (false, "Stop time cannot be greater than current time");
        }

        if (utcStartTime < DateTime.UtcNow.AddMonths(-3))
        {
            return (false, "Start time cannot be older than 3 months");
        }

        return (true, "A-Ok");
    }

    private bool MatchKeyvalue(string vehicleBrand)
    {
        return clientDictionary.ContainsKey(vehicleBrand);
    }
};

