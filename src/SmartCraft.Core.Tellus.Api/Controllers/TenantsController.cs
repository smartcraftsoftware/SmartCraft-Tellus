using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Mappers;
using SmartCraft.Core.Tellus.Domain.Services;

namespace SmartCraft.Core.Tellus.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class TenantsController : ControllerBase
{
    private Serilog.ILogger _logger;
    private readonly ITenantService _tenantService;
    public TenantsController(Serilog.ILogger logger, ITenantService service)
    {
        _logger = logger.ForContext<TenantsController>();
        _tenantService = service;
    }
    /// <summary>
    /// Gets a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns>A tenant object</returns>
    /// <response code="200">Returns a tenant</response>
    /// <response code="404">Could not find specified tenant</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    public async Task<ActionResult<GetTenantResponse>> Get([FromHeader]Guid tenantId)
    {
        try
        {
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if(tenant == null)
            {
                return NotFound("Could not find tenant.");
            }

            return Ok(tenant);
        }
        catch (Exception ex)
        {
            _logger.Error("Error getting {Tenant} with {ErrorMessage}", tenantId, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Creates a tenant
    /// </summary>
    /// <param name="tenantRequest"></param>
    /// <param name="tenantId"></param>
    /// <returns>Ok result</returns>
    /// <response code="200">Tenant has been created</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    public async Task<ActionResult<Guid>> Post([FromHeader]Guid tenantId, [FromBody]AddTenantRequest tenantRequest)
    {
        try
        {
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if (tenant != null)
            {
                return BadRequest("Tenant already exists");
            }

            return Ok(await _tenantService.RegisterTenantAsync(tenantId, tenantRequest.ToDomainModel()));
        }
        catch (Exception ex)
        {
            _logger.Error("Error creating tenant with {ErrorMessage}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="tenantRequest"></param>
    /// <returns>Ok result</returns>
    /// <response code="200">Tenant has been updated</response>
    /// <response code="500">Internal server error</response>
    [HttpPatch]
    public async Task<ActionResult> Patch([FromHeader]Guid tenantId, [FromBody]UpdateTenantRequest tenantRequest)
    {
        try
        {
            var tenantToUpdate = tenantRequest.ToDomainModel();
            var updatedTenant = await _tenantService.UpdateTenantAsync(tenantId, tenantToUpdate);
            return Ok(updatedTenant);
        }
        catch (Exception ex)
        {
            _logger.Error( "Error updating tenant {Tenant} with {ErrorMessage}", tenantId, ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Deletes a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns>No content result</returns>
    /// <response code="204">Tenant has been deleted</response>
    /// <response code="404">Could not find specified tenant</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete]
    public async Task<ActionResult> Delete([FromHeader]Guid tenantId)
    {
        try
        {
            if(await _tenantService.DeleteTenant(tenantId))
            {
                return NoContent();
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.Error("Error deleting {Tenant} with {ErrorMessage}", tenantId, ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
