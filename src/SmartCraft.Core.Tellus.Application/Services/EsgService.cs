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

    public async Task<Domain.Models.EsgVehicleReport> GetEsgReportAsync(string vehicleBrand, string? vinNumber, Tenant tenant, DateTime startTime, DateTime stopTime = default)
    {
        if(stopTime == default)
            stopTime = DateTime.Now;

        if(!MatchDateTimeValue(startTime, stopTime))
            throw new ArgumentException("Invalid date time values");

        if (!MatchKeyvalue(vehicleBrand))
            throw new KeyNotFoundException($"Vehicle brand {vehicleBrand} not found");

        var esgReport = await clientDictionary[vehicleBrand.ToLower()].GetEsgReportAsync(vinNumber, tenant, startTime, stopTime);
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

    private bool MatchDateTimeValue(DateTime startTime, DateTime stopTime)
    {
        if(startTime > stopTime)
            return false;
        if(startTime > DateTime.Now)
            return false;
        if (stopTime > DateTime.Now)
            return false;
        if(startTime < DateTime.Now.AddMonths(-3))
            return false;

        return true;
    }

    private bool MatchKeyvalue(string vehicleBrand)
    {
        return clientDictionary.ContainsKey(vehicleBrand);
    }
}
