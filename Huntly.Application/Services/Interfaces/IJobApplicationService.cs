using Huntly.Application.DTOs;
using Huntly.Application.DTOs.JobApplication;

namespace Huntly.Application.Services.Interfaces
{
    public interface IJobApplicationService
    {
        Task<PagedResponse<JobApplicationResponse>> GetPagedByUserIdAsync(Guid userId, int page, int pageSize);
        Task<JobApplicationResponse?> GetByIdAsync(Guid id, Guid userId);
        Task<JobApplicationResponse> CreateAsync(Guid userId, CreateJobApplicationRequest request);
        Task UpdateAsync(Guid id, Guid userId, UpdateJobApplicationRequest request);
        Task ChangeStatusAsync(Guid id, Guid userId, ChangeStatusRequest request);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
