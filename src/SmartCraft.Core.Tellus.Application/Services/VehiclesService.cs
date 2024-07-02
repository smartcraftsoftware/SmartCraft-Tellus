﻿using SmartCraft.Core.Tellus.Domain.Models;
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

    public async Task<Domain.Models.StatusReport> GetVehicleStatusAsync(string vehicleBrand, string vinNumber, Tenant tenant, string startTime, string stopTime)
    {
        (var start, var stop) = ParseAndMatchDateTimeValues(startTime, stopTime);

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        return await clientDictionary[vehicleBrand.ToLower()].GetVehicleStatusAsync(vinNumber, tenant, start, stop);
    }

    public async Task<List<Vehicle>> GetFleetAsync(string vehicleBrand, Tenant tenant)
    {
        if (!MatchKeyvalue(vehicleBrand.ToLower()))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicles = await clientDictionary[vehicleBrand.ToLower()].GetVehiclesAsync(tenant);
        return vehicles;
    }

    public async Task<IntervalStatusReport> GetIntervalVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, string startTime, string stopTime)
    {
        (var start, var stop) = ParseAndMatchDateTimeValues(startTime, stopTime);

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicleIntervalStatus = await clientDictionary[vehicleBrand.ToLower()].GetIntervalStatusReportAsync(vin, tenant, start, stop);

        await _intervalStatusRepository.Add(vehicleIntervalStatus.ToDataModel(), tenant.Id);

        return vehicleIntervalStatus;
    }

    private (string, string) ParseAndMatchDateTimeValues(string startTime, string stopTime)
    {
        if (!DateTime.TryParse(startTime, null, System.Globalization.DateTimeStyles.RoundtripKind, out var start))
        {
            throw new InvalidOperationException("Invalid start time format");
        }

        if (!DateTime.TryParse(stopTime, null, System.Globalization.DateTimeStyles.RoundtripKind, out var stop))
        {
            throw new InvalidOperationException("Invalid stop time format");
        }

        start = start.ToUniversalTime();
        stop = stop.ToUniversalTime();

        if (start.Kind != DateTimeKind.Utc && start.Kind != DateTimeKind.Local)
        {
            throw new InvalidOperationException("Invalid start time zone");
        }

        if (stop.Kind != DateTimeKind.Utc && stop.Kind != DateTimeKind.Local)
        {
            throw new InvalidOperationException("Invalid stop time zone");
        }

        if (start > stop)
        {
            throw new InvalidOperationException("Start time cannot be after stop time");
        }

        if (start > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Start time cannot be greater than current time");
        }

        if (stop > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Stop time cannot be greater than current time");
        }

        if (start < DateTime.UtcNow.AddMonths(-3))
        {
            throw new InvalidOperationException("Start time cannot be older than 3 months");
        }

        return (start.ToString("yyyy-MM-ddTHH:mm:ssZ"), stop.ToString("yyyy-MM-ddTHH:mm:ssZ"));
    }

    private bool MatchKeyvalue(string vehicleBrand)
    {
        return clientDictionary.ContainsKey(vehicleBrand);
    }
};

