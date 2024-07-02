using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Domain.Validators;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Application.Services;
public class EsgService : IEsgService
{
    private Dictionary<string, IVehicleClient> clientDictionary = new Dictionary<string, IVehicleClient>();
    private readonly IRepository<Infrastructure.Models.EsgVehicleReport, VehicleContext> _repository;
    private readonly IEnumerable<IVehicleClient> _clients;

    public EsgService(IRepository<Infrastructure.Models.EsgVehicleReport, VehicleContext> repository, IEnumerable<IVehicleClient> clients)
    {
        _repository = repository;
        _clients = clients;
        Execute();
    }
    public void Execute()
    {
        clientDictionary = _clients.ToDictionary(x => x.VehicleBrand, x => x);
    }

    public async Task<Domain.Models.EsgVehicleReport> GetEsgReportAsync(string vehicleBrand, string? vinNumber, Tenant tenant, string startTime, string stopTime = default)
    {
        (var start, var stop) = ParseAndMatchDateTimeValues(startTime, stopTime);

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var esgReport = await clientDictionary[vehicleBrand.ToLower()].GetEsgReportAsync(vinNumber, tenant, start, stop);
        
        if(esgReport == null)
            return null;

        var validator = new EsgReportValidator();
        var validationResult = validator.Validate(esgReport);

        if (!validationResult.IsValid)
        {
            foreach (var failure in validationResult.Errors)
            {
                Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
            }
            throw new ArgumentException("Invalid Esg Report");
        }

        await _repository.Add(esgReport.ToDataModel(), tenant.Id);
        return esgReport;
    }

    private (string, string) ParseAndMatchDateTimeValues(string startTime, string stopTime)
    {
        if (!DateTime.TryParse(startTime, out var start))
        {
            throw new InvalidOperationException("Invalid start time format");
        }

        if (!DateTime.TryParse(stopTime, out var stop))
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
}
