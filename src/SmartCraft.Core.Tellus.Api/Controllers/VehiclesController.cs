using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Mappers;
using SmartCraft.Core.Tellus.Domain.Services;

namespace SmartCraft.Core.Tellus.Api.Controllers;

/// <summary>
/// Gets various reports for vehicles
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class VehiclesController(ILogger<VehiclesController> logger, IVehiclesService vehicleService, IEsgService esgService, ITenantService tenantService) : ControllerBase
{
    /// <summary>
    /// Gets a list of vehicles in current users' fleet, based on the vehicle brand
    /// </summary>
    /// <param name="vehicleBrand"></param>
    /// <param name="tenantId"></param>
    /// <returns>A list of vehicle objects</returns>
    /// <response code="200">Returns a vehicle status report</response>
    /// <response code="404">Could not find vehicle</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{vehicleBrand}/vehicles")]
    public async Task<ActionResult<List<GetVehicleResponse>>> GetVehiclesAsync(string vehicleBrand, [FromHeader]Guid tenantId)
    {
        try
        {
            logger.LogInformation("Getting vehicles for tenant {tenantId}", tenantId);
            var tenant = await tenantService.GetTenantAsync(tenantId);
            if(tenant == null)
            {
                logger.LogWarning("Could not find tenant {tenantId}", tenantId);
                return NotFound("Could not find tenant");

            }

            var vehicles = await vehicleService.GetFleetAsync(vehicleBrand, tenant);
            if (vehicles == null)
                return NotFound("Could not find vehicle");

            logger.Log(LogLevel.Information, "Found {vehicleCount} vehicles", vehicles.Count);
            return Ok(vehicles.Select(x => x.ToVehicleResponse()));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error getting vehicles for tenant {tenantId}", tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Gets ESG report for a vehicle
    /// </summary>
    /// <param name="vehicleBrand">Brand of vehicle to fetch</param>
    /// <param name="vinOrId">Vin number or external id of vehicle. 
    /// If excluded, will fetch all vehicles of current user's fleet.</param>
    /// <param name="startTime">Start time of interval</param>
    /// <param name="stopTime">Stop time of interval</param>
    /// <param name="tenantId">Id of tenant</param>
    /// <returns></returns>
    /// <response code="200">Returns a vehicle status report</response>
    /// <response code="404">Could not find vehicle</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{vehicleBrand}/esgreport")]
    public async Task<ActionResult<EsgReportResponse>> GetEsgReportAsync(string vehicleBrand, string? vinOrId, string startTime, string stopTime, [FromHeader] Guid tenantId)
    {
        try
        {
            logger.Log(LogLevel.Information, "Getting ESG report for tenant {tenantId}", tenantId);
            var tenant = await tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                logger.LogWarning("Could not find tenant {tenantId}", tenantId);
                return NotFound("Could not find tenant");
            }

            var vehicle = await esgService.GetEsgReportAsync(vehicleBrand, vinOrId, tenant, startTime, stopTime);
            if (vehicle == null)
            {
                logger.LogWarning("Could not find vehicle {vinOrId}", vinOrId);
                return NotFound("Could not find vehicle");
            }

            logger.Log(LogLevel.Information, "Found vehicle {vinOrId}", vinOrId);
            return Ok(vehicle.ToResponse());
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error getting ESG report for tenant {tenantId}", tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Gets a summarized environmental status report for a vehicle
    /// </summary>
    /// <param name="vehicleBrand">Brand of vehicle to fetch</param>
    /// <param name="vinOrId">Vin number or external id of vehicle.
    /// <param name="startTime">Start time of interval</param>
    /// <param name="stopTime">Stop time of interval</param>
    /// <returns></returns>
    /// <response code="200">Returns a vehicle status report</response>
    /// <response code="404">Could not find vehicle</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{vehicleBrand}/statusreport")]
    public async Task<ActionResult<IntervalStatusReportResponse>> GetVehicleStatusReport(string vehicleBrand, string vinOrId, string startTime, string stopTime, [FromHeader] Guid tenantId)
    {
        try
        {
            logger.Log(LogLevel.Information, "Fetching vehicle statuses for tenant {tenantId}", tenantId);
            if (vinOrId != null)
            {
                logger.Log(LogLevel.Information, "...more specifically for {vehicleBrand} vehicle {vehicleId}", vehicleBrand, vinOrId);
            }

            var tenant = await tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                logger.LogWarning("Could not find tenant {tenantId}", tenantId);
                return NotFound("Could not find tenant");
            }

            var statusReport = await vehicleService.GetIntervalVehicleStatusAsync(vehicleBrand, vinOrId, tenant, startTime, stopTime);
            return Ok(statusReport.ToIntervalRespone());
        }
        catch (HttpRequestException ex)
        {
            logger.Log(LogLevel.Error, $"The vehicle client threw an HTTP request exception: \"{(int)ex.StatusCode} {ex.Message}\"");
            return StatusCode((int)ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error getting vehicle status report for tenant {tenantId}", tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
