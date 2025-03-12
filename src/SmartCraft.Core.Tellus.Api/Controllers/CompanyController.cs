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
public class CompanyController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly ICompanyService _companyService;
    public CompanyController(Serilog.ILogger logger, ICompanyService service)
    {
        _logger = logger.ForContext<CompanyController>();
        _companyService = service;
    }
    /// <summary>
    /// Gets a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <returns>A tenant object</returns>
    /// <response code="200">Returns a tenant</response>
    /// <response code="404">Could not find specified tenant</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{companyId}")]
    public async Task<ActionResult<GetCompanyResponse>> Get([FromHeader]Guid tenantId, Guid companyId)
    {
        try
        {
            var tenant = await _companyService.GetCompanyAsync(companyId, tenantId);
            if(tenant == null)
            {
                return NotFound("Could not find company.");
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
    /// Creates a Company
    /// </summary>
    /// <param name="tenantRequest"></param>
    /// <param name="tenantId"></param>
    /// <returns>Ok result</returns>
    /// <response code="200">Tenant has been created</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    public async Task<ActionResult<Guid>> Post([FromHeader]Guid tenantId, [FromBody]AddCompanyRequest tenantRequest)
    {
        try
        {
            return Ok(await _companyService.RegisterCompanyAsync(tenantId, tenantRequest.ToDomainModel()));
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
    /// <param name="companyRequest"></param>
    /// <returns>Ok result</returns>
    /// <response code="200">Tenant has been updated</response>
    /// <response code="500">Internal server error</response>
    [HttpPatch]
    public async Task<ActionResult> Patch([FromHeader]Guid tenantId, [FromBody]UpdateCompanyRequest companyRequest)
    {
        try
        {
            var companyToUpdate = companyRequest.ToDomainModel();
            var updatedCompany = await _companyService.UpdateCompanyAsync(tenantId, companyToUpdate);
            return Ok(updatedCompany);
        }
        catch (Exception ex)
        {
            _logger.Error( "Error updating Company with {Exception}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Deletes a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <returns>No content result</returns>
    /// <response code="204">Tenant has been deleted</response>
    /// <response code="404">Could not find specified tenant</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete]
    public async Task<ActionResult> Delete([FromHeader] Guid tenantId, Guid companyId)
    {
        try
        {
            var company = await _companyService.GetCompanyAsync(companyId, tenantId);
            if (company == null)
            {
                return NotFound("Could not find company.");
            }

            if (await _companyService.DeleteCompany(companyId))
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
