using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Mappers;
using SmartCraft.Core.Tellus.Domain.Services;
using Serilog;

namespace SmartCraft.Core.Tellus.Api.Controllers;

/// <summary>
/// Gets various reports for vehicles
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private Serilog.ILogger _logger;
    private readonly IVehiclesService _vehicleService;
    private readonly IEsgService _esgService;
    private readonly ICompanyService _tenantService;
    public VehiclesController(Serilog.ILogger logger, IVehiclesService vehicleService, IEsgService esgService, ICompanyService tenantService)
    {
        _logger = logger.ForContext<VehiclesController>();
        _vehicleService = vehicleService;
        _esgService = esgService;
        _tenantService = tenantService;
    }
    /// <summary>
    /// Gets a list of vehicles in current users' fleet, based on the vehicle brand
    /// </summary>
    /// <param name="vehicleBrand"></param>
    /// <param name="vin"></param>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <returns>A list of vehicle objects</returns>
    /// <response code="200">Returns a vehicle status report</response>
    /// <response code="404">Could not find vehicle</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{companyId}/{vehicleBrand}/vehicles")]
    public async Task<ActionResult<List<GetVehicleResponse>>> GetVehiclesAsync(string vehicleBrand, string? vin, [FromHeader]Guid tenantId, Guid companyId)
    {
        try
        {
            var company = await _tenantService.GetCompanyAsync(companyId, tenantId);
            if (company == null)
            {
                return NotFound("Could not find tenant");
            }

            var vehicles = await _vehicleService.GetVehiclesAsync(vehicleBrand, vin, company);
            if (vehicles == null || vehicles.Count == 0)
                return NoContent();


            return Ok(vehicles.Select(x => x.ToVehicleResponse()));
        }
        catch (Exception ex)
        {
            _logger.Error("Error getting vehicles for tenant {tenantId} with {ErrorMessage}", tenantId, ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Returns reports for vehicles between two dates, without timestamps.
    /// </summary>
    /// <param name="vehicleBrand">Brand of vehicle to fetch</param>
    /// <param name="vinOrId">Vin number or external id of vehicle. 
    /// If excluded, will fetch all vehicles of current user's fleet.</param>
    /// <param name="startTime">Start time of interval (yyyy-MM-dd)</param>
    /// <param name="stopTime">Stop time of interval (yyyy-MM-dd)</param>
    /// <param name="tenantId">Id of tenant</param>
    /// <param name="companyId"></param>
    /// <returns></returns>
    /// <response code="200">Returns a vehicle status report</response>
    /// <response code="404">Could not find vehicle</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{companyId}/{vehicleBrand}/report")]
    public async Task<ActionResult<VehicleEvaluationReportResponse>> GetReportAsync(string vehicleBrand, string? vinOrId, DateTime startTime, DateTime stopTime, [FromHeader] Guid tenantId, Guid companyId)
    {
        try
        {
            var company = await _tenantService.GetCompanyAsync(companyId, tenantId);
            if (company == null)
            {
                return NotFound("Could not find tenant");
            }
            var vehicle = await _esgService.GetEsgReportAsync(vehicleBrand, vinOrId, company, startTime, stopTime);
            if (vehicle == null)
            {
                return NoContent();
            }

            return Ok(vehicle.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.Error("Error getting report for {Tenant} with {ErrorMessage}", tenantId, ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Gets a summarized environmental status report for a vehicle between two timestamps
    /// </summary>
    /// <param name="vehicleBrand">Brand of vehicle to fetch</param>
    /// <param name="vinOrId">Vin number or external id of vehicle.</param>
    /// <param name="startTime">Start time of interval (yyyy-MM-dd hh:mm:ss)</param>
    /// <param name="stopTime">Stop time of interval (yyyy-MM-dd hh:mm:ssZ)</param>
    /// <param name="tenantId">Stop time of interval (yyyy-MM-dd hh:mm:ssZ)</param>
    /// <param name="companyId"></param>
    /// <returns></returns>
    /// <response code="200">Returns a vehicle status report</response>
    /// <response code="404">Could not find vehicle</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{companyId}/{vehicleBrand}/vehiclestatus")]
    public async Task<ActionResult<IntervalStatusReportResponse>> GetVehicleStatusReport(string vehicleBrand, DateTime startTime, DateTime stopTime, [FromHeader] Guid tenantId, Guid companyId, string vinOrId = "")
    {
        try
        {
            var company = await _tenantService.GetCompanyAsync(companyId, tenantId);
            if (company == null)
            {
                return NotFound("Could not find tenant");
            }

            var statusReport = await _vehicleService.GetVehicleStatusAsync(vehicleBrand, vinOrId, company, startTime, stopTime);
            return Ok(statusReport.ToIntervalRespone());
        }
        catch (HttpRequestException ex)
        {
            _logger.Error("The vehicle client threw an HTTP request {Exception}", ex);
            return StatusCode((int)ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error("Error getting vehicle status report for {Tenant} with {ErrorMessage}", tenantId, ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
