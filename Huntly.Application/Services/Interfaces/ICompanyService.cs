using Huntly.Application.DTOs.Company;

namespace Huntly.Application.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<IReadOnlyList<CompanyResponse>> GetAllAsync();
        Task<CompanyResponse?> GetByIdAsync(Guid id);
        Task<CompanyResponse> CreateAsync(CreateCompanyRequest request);
        Task UpdateAsync(Guid id, UpdateCompanyRequest request);
        Task DeleteAsync(Guid id);
    }
}
