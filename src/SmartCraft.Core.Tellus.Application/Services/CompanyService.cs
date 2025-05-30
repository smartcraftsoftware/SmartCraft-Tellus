﻿using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Application.Services;

public class CompanyService(ICompanyRepository<Infrastructure.Models.Company, CompanyContext> repository) : ICompanyService
{
    public async Task<Company?> GetCompanyAsync(Guid companyId, Guid tenantId)
    {
        var company = await repository.Get(companyId);
        if (company == null)
            return null;
        if(company.TenantId != tenantId)
            return null;
        return company.ToDomainModel();
    }
    public async Task<List<Company>> GetCompaniesAsync(Guid tenantId)
    {
        var companies = await repository.GetAll(tenantId);
        return companies.Select(x => x.ToDomainModel()).ToList();
    }

    public async Task<Guid> RegisterCompanyAsync(Guid tenantId, Company company)
    {
        var companyToAdd = company.ToCreateCompanyModel(tenantId);
        await repository.Add(companyToAdd, tenantId);
        return companyToAdd.Id;
    }

    public async Task<Company> UpdateCompanyAsync(Company company)
    {
        var existingCompany = await repository.Get(company.Id);
        if (existingCompany == null)
        {
            throw new InvalidOperationException("TenantCompany not found");
        }

        if(company.Name != null)
            existingCompany.Name = company.Name;
        if (company.DaimlerToken != null)
            existingCompany.DaimlerToken = company.DaimlerToken;
        if (company.ManToken != null)
            existingCompany.ManToken = company.ManToken;
        if (company.ScaniaClientId != null)
            existingCompany.ScaniaClientId = company.ScaniaClientId;
        if (company.ScaniaSecretKey != null)
            existingCompany.ScaniaSecretKey = company.ScaniaSecretKey;
        if (company.VolvoCredentials != null)
            existingCompany.VolvoCredentials = company.VolvoCredentials;

        var updatedCompany = await repository.Update(existingCompany, existingCompany.TenantId);

        return updatedCompany.ToDomainModel();
    }

    public async Task<bool> DeleteCompany(Guid companyId)
    {
       return await repository.Delete(companyId);
    }
}

