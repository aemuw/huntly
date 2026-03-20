using Huntly.Application.DTOs.Company;
using Huntly.Application.Exceptions;
using Huntly.Application.Services.Interfaces;
using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;

namespace Huntly.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IReadOnlyList<CompanyResponse>> GetAllAsync()
    {
        var companies = await _companyRepository.GetAllAsync();
        return companies.Select(MapToResponse).ToList();
    }

    public async Task<CompanyResponse?> GetByIdAsync(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        return company is null ? null : MapToResponse(company);
    }

    public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request)
    {
        if (await _companyRepository.ExistsAsync(request.Name))
            throw new ValidationException($"Компанія '{request.Name}' вже існує");

        var company = new Company(request.Name, request.Type, request.Size);

        company.Update(
            request.Name,
            request.Type,
            request.Size,
            request.Website,
            request.LinkedIn,
            request.Notes);

        await _companyRepository.AddAsync(company);
        return MapToResponse(company);
    }

    public async Task UpdateAsync(Guid id, UpdateCompanyRequest request)
    {
        var company = await _companyRepository.GetByIdAsync(id);

        if (company is null)
            throw new NotFoundException("Компанію не знайдено");

        company.Update(
            request.Name,
            request.Type,
            request.Size,
            request.Website,
            request.LinkedIn,
            request.Notes);

        await _companyRepository.UpdateAsync(company);
    }

    private static CompanyResponse MapToResponse(Company company)
        => new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Type = company.Type.ToString(),
            Size = company.Size.ToString(),
            Website = company.Website,
            LinkedIn = company.LinkedIn,
            Notes = company.Notes,
            Technologies = company.Technologies
                .Select(t => t.Name)
                .ToList()
        };

    public async Task DeleteAsync(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company is null)
            throw new NotFoundException("Компанію не знайдено");

        await _companyRepository.DeleteAsync(id);
    }
}