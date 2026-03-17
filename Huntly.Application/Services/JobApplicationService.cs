using Huntly.Application.DTOs.JobApplication;
using Huntly.Application.Exceptions;
using Huntly.Application.Services.Interfaces;
using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;

namespace Huntly.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _jobRepository;
        private readonly ICompanyRepository _companyRepository;

        public JobApplicationService(IJobApplicationRepository jobRepository, ICompanyRepository companyRepository)
        {
            _jobRepository = jobRepository;
            _companyRepository = companyRepository;
        }

        public async Task<IReadOnlyList<JobApplicationResponse>> GetByUserIdAsync(Guid userId)
        {
            var jobs = await _jobRepository.GetByUserIdAsync(userId);
            return jobs.Select(MapToResponse).ToList();
        }

        public async Task<JobApplicationResponse?> GetByIdAsync(Guid id, Guid userId)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job is null)
                return null;

            if (job.UserId != userId)
                throw new UnauthorizedException("Доступ заборонено");

            return MapToResponse(job);
        }

        public async Task<JobApplicationResponse?> CreateAsync(Guid userId, CreateJobApplicationRequest request)
        {
            var company = await _companyRepository.GetByIdAsync(request.CompanyId);

            if (company is null)
                throw new NotFoundException("Компанію не знайдено");

            var job = new JobApplication(
                request.Title,
                userId,
                request.CompanyId,
                request.Priority);

            job.Update(
                request.Title,
                request.Description,
                request.JobUrl,
                request.Priority,
                request.SalaryFrom,
                request.SalaryTo,
                request.DueDate,
                request.Notes);

            await _jobRepository.AddAsync(job);
            return MapToResponse(job);
        }

        public async Task UpdateAsync(Guid id, Guid userId, UpdateJobApplicationRequest request)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job is null)
                throw new NotFoundException("Заяву не знайдено");

            if (job.UserId != userId)
                throw new UnauthorizedException("Доступ заборонено");

            job.Update(
            request.Title,
            request.Description,
            request.JobUrl,
            request.Priority,
            request.SalaryFrom,
            request.SalaryTo,
            request.DueDate,
            request.Notes);

            await _jobRepository.UpdateAsync(job);
        }

        public async Task ChangeStatusAsync(Guid id, Guid userId, ChangeStatusRequest request)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job is null)
                throw new NotFoundException("Заявку не знайдено");

            if (job.UserId != userId)
                throw new UnauthorizedException("Доступ заборонено");

            job.ChangeStatus(request.Status);
            await _jobRepository.UpdateAsync(job);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job is null)
                throw new NotFoundException("Заявку не знайдено");

            if (job.UserId != userId)
                throw new UnauthorizedException("Доступ заборонено");

            await _jobRepository.DeleteAsync(id);
        }

        private static JobApplicationResponse MapToResponse(JobApplication job)
            => new JobApplicationResponse
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                JobUrl = job.JobUrl,
                Status = job.Status,
                StatusDisplay = job.Status.ToString(),
                Priority = job.Priority,
                SalaryFrom = job.SalaryFrom,
                SalaryTo = job.SalaryTo,
                AppliedDate = job.AppliedDate,
                DueDate = job.DueDate,
                Notes = job.Notes,
                CompanyId = job.CompanyId,
                CompanyName = job.Company?.Name ?? string.Empty,
                CreatedAt = job.CreatedAt
            };
    }
}
