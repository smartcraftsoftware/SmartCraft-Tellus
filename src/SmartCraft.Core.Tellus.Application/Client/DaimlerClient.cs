﻿using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Infrastructure.Client;

namespace SmartCraft.Core.Tellus.Application.Client;
public class DaimlerClient : IVehicleClient
{
    public string VehicleBrand => "daimler";

    public Task<EsgVehicleReport> GetEsgReportAsync(string? vin, Company tenant, DateTime startTime, DateTime stopTime)
    {
        throw new NotImplementedException();
    }

    public Task<IntervalStatusReport> GetVehicleStatusAsync(string vin, Company tenant, DateTime startTime, DateTime stopTime)
    {
        throw new NotImplementedException();
    }

    public Task<List<Vehicle>> GetVehiclesAsync(Company tenant, string? vin)
    {
        throw new NotImplementedException();
    }

    //public Task<StatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, DateTime startTime, DateTime? stopTime)
    //{
    //    throw new NotImplementedException();
    //}
}
