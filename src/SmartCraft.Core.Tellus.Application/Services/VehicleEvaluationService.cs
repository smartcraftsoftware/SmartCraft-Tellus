using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Domain.Validators;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Application.Services;
public class VehicleEvaluationService : IVehicleEvaluationService
{
    private Dictionary<string, IVehicleClient> clientDictionary = new Dictionary<string, IVehicleClient>();
    private readonly IRepository<Infrastructure.Models.VehicleEvaluationReport, VehicleContext> _repository;
    private readonly IEnumerable<IVehicleClient> _clients;

    public VehicleEvaluationService(IRepository<Infrastructure.Models.VehicleEvaluationReport, VehicleContext> repository, IEnumerable<IVehicleClient> clients)
    {
        _repository = repository;
        _clients = clients;
        Execute();
    }
    public void Execute()
    {
        clientDictionary = _clients.ToDictionary(x => x.VehicleBrand, x => x);
    }

    public async Task<Domain.Models.VehicleEvaluationReport> GetVehicleEvaluationReportAsync(string vehicleBrand, string? vinNumber, Tenant tenant, DateTime startTime, DateTime stopTime = default)
    {
        (var start, var stop) = ParseAndMatchDateTimeValues(startTime, stopTime);

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var vehicleEvaluationReport = await clientDictionary[vehicleBrand.ToLower()].GetVehicleEvaluationReportAsync(vinNumber, tenant, start, stop);
        
        if (vehicleEvaluationReport == null)
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.

        var validator = new VehicleEvaluationReportValidator();
        var validationResult = validator.Validate(vehicleEvaluationReport);

        if (!validationResult.IsValid)
        {
            foreach (var failure in validationResult.Errors)
            {
                Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
            }
            throw new ArgumentException("Invalid Esg Report");
        }

        await _repository.Add(vehicleEvaluationReport.ToDataModel(), tenant.Id);
        return vehicleEvaluationReport;
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
}
