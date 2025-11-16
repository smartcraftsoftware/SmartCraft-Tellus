using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Api.Mappers;
using SmartCraft.Core.Tellus.Domain.Services;
using ILogger = Serilog.ILogger;

namespace SmartCraft.Core.Tellus.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly ICompanyService _companyService;
    public CompanyController(ILogger logger, ICompanyService service)
    {
        _logger = logger.ForContext<CompanyController>();
        _companyService = service;
    }
    /// <summary>
    /// Gets a company
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <returns>A company object</returns>
    /// <response code="200">Returns a company</response>
    /// <response code="404">Could not find specified company</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{companyId}")]
    public async Task<ActionResult<GetCompanyResponse>> Get([FromHeader]Guid tenantId, Guid companyId)
    {
        var company = await _companyService.GetCompanyAsync(companyId, tenantId);
        if(company == null)
        {
            return NotFound("Could not find company.");
        }

        return Ok(company.ToResponseContract());
    }

    /// <summary>
    /// Gets all companies for a single tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<GetCompanyResponse>>> Get([FromHeader]Guid tenantId)
    {
        var companies = await _companyService.GetCompaniesAsync(tenantId);
        if(companies.Count == 0 || companies == null)
        {
            return NotFound("Could not find any companies.");
        }

        return Ok(companies.Select(x => x.ToResponseContract()));
    }

    /// <summary>
    /// Creates a Company
    /// </summary>
    /// <param name="companyRequest"></param>
    /// <param name="tenantId"></param>
    /// <returns>Ok result</returns>
    /// <response code="200">Company has been created</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    public async Task<ActionResult<Guid>> Post([FromHeader]Guid tenantId, [FromBody]AddCompanyRequest companyRequest)
    {
        return Ok(await _companyService.RegisterCompanyAsync(tenantId, companyRequest.ToDomainModel()));
    }

    /// <summary>
    /// Updates an existing company
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <param name="companyRequest"></param>
    /// <returns>Ok result</returns>
    /// <response code="200">Company has been updated</response>
    /// <response code="500">Internal server error</response>
    [HttpPatch("{companyId}")]
    public async Task<ActionResult> Patch([FromHeader]Guid tenantId, Guid companyId, [FromBody]UpdateCompanyRequest companyRequest)
    {
        var company = await _companyService.GetCompanyAsync(companyId, tenantId);
        if (company == null)
        {
            return NotFound("Could not find company.");
        }
        
        var companyUpdateValues = companyRequest.ToDomainModel(companyId);
        var updatedCompany = await _companyService.UpdateCompanyAsync(companyUpdateValues);
        return Ok(updatedCompany);
    }

    /// <summary>
    /// Deletes a company
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <returns>No content result</returns>
    /// <response code="204">Company has been deleted</response>
    /// <response code="404">Could not find specified Company</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{companyId}")]
    public async Task<ActionResult> Delete([FromHeader]Guid tenantId, Guid companyId)
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
}
