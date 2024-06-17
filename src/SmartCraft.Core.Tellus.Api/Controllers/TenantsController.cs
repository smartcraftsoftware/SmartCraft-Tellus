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
public class TenantsController(ILogger<TenantsController> logger, ITenantService service) : ControllerBase
{
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
            logger.Log(LogLevel.Information, "Getting tenant {tenantId}", tenantId);
            var tenant = await service.GetTenantAsync(tenantId);
            if(tenant == null)
            {
                logger.LogWarning("Could not find tenant {tenantId}", tenantId);
                return NotFound("Could not find tenant.");
            }

            return Ok(tenant);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error getting tenant {tenantId}", tenantId);
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
            var tenant = await service.GetTenantAsync(tenantId);
            if (tenant != null)
            {
                logger.LogWarning("Tenant already exists");
                return BadRequest("Tenant already exists");
            }

            logger.Log(LogLevel.Information, "Registering tenant");
            return Ok(await service.RegisterTenantAsync(tenantId, tenantRequest.ToDomainModel()));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error registering tenant");
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
            logger.Log(LogLevel.Information, "Updating tenant {tenantId}", tenantId);
            var tenantToUpdate = tenantRequest.ToDomainModel();
            var updatedTenant = await service.UpdateTenantAsync(tenantId, tenantToUpdate);
            return Ok(updatedTenant);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error updating tenant {tenantId}", tenantId);
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
            logger.Log(LogLevel.Information, "Deleting tenant {tenantId}", tenantId);
            if(await service.DeleteTenant(tenantId))
            {
                logger.Log(LogLevel.Information, "Tenant {tenantId} has been deleted", tenantId);
                return NoContent();
            }

            logger.LogWarning("Could not find tenant {tenantId}", tenantId);
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error deleting tenant {tenantId}", tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
