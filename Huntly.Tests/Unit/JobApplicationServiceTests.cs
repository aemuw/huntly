using Huntly.Application.DTOs.JobApplication;
using Huntly.Application.Exceptions;
using Huntly.Application.Services;
using Huntly.Domain.Entities;
using Huntly.Domain.Enums;
using Huntly.Domain.Interfaces;
using Moq;

namespace Huntly.Tests.Unit
{
    public class JobApplicationServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _jobRepoMock;
        private readonly Mock<ICompanyRepository> _companyRepoMock;
        private readonly JobApplicationService _service;

        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _companyId = Guid.NewGuid();

        public JobApplicationServiceTests()
        {
            _jobRepoMock = new Mock<IJobApplicationRepository>();
            _companyRepoMock = new Mock<ICompanyRepository>();

            _service = new JobApplicationService(_jobRepoMock.Object, _companyRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsResponse()
        {
            var company = new Company("GlobalLogic", CompanyType.Outsourse, CompanySize.Large);
            var request = new CreateJobApplicationRequest
            {
                Title = "Junior C# Developer",
                CompanyId = _companyId,
                Priority = Priority.Medium
            };

            _companyRepoMock.Setup(r => r.GetByIdAsync(_companyId)).ReturnsAsync(company);

            var result = await _service.CreateAsync(_userId, request);

            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(ApplicationStatus.Watchlist, result.Status);
        }

        [Fact]
        public async Task CreateAsync_CompanyNotFound_ThrowsNotFoundException()
        {
            var request = new CreateJobApplicationRequest
            {
                Title = "Junior C# Developer",
                CompanyId = _companyId,
                Priority = Priority.Medium
            };

            _companyRepoMock.Setup(r => r.GetByIdAsync(_companyId)).ReturnsAsync((Company?)null);
            
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(_userId, request));
        }

        [Fact]
        public async Task GetByIdAsync_OtherUserJob_ThrowsUnauthorizedException()
        {
            var otherUserId = Guid.NewGuid();
            var job = new JobApplication("Junior", otherUserId, _companyId, Priority.Low);

            _jobRepoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _service.GetByIdAsync(job.Id, _userId));
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((JobApplication?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid(), _userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task ChangeStatusAsync_ValidRequest_ChangesStatus()
        {
            var job = new JobApplication("Junior", _userId, _companyId, Priority.Low);
            var request = new ChangeStatusRequest { Status = ApplicationStatus.Applied };

            _jobRepoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            await _service.ChangeStatusAsync(job.Id, _userId, request);

            Assert.Equal(ApplicationStatus.Applied, job.Status);
            Assert.NotNull(job.AppliedDate);
        }

        [Fact]
        public async Task ChangeStatusAsync_JobNotFound_ThrowsNotFoundException()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((JobApplication?)null);

            await Assert.ThrowsAsync<NotFoundException>(()
                => _service.ChangeStatusAsync(Guid.NewGuid(), _userId, new ChangeStatusRequest { Status = ApplicationStatus.Applied }));
        }

        [Fact]
        public async Task DeleteAsync_ValidRequest_DeletesJob()
        {
            var job = new JobApplication("Junior", _userId, _companyId, Priority.Low);

            _jobRepoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            await _service.DeleteAsync(job.Id, _userId);

            _jobRepoMock.Verify(r => r.DeleteAsync(job.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_OthersJob_ThrowsUnauthorizedException()
        {
            var otherUserId = Guid.NewGuid();
            var job = new JobApplication("Junior", otherUserId, _companyId, Priority.High);

            _jobRepoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _service.DeleteAsync(job.Id, _userId));
        }

        [Fact]
        public async Task DeleteAsync_NotFound_ThrowsNotFoundException()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((JobApplication?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid(), _userId));
        }

        [Fact]
        public async Task UpdateAsync_OtherUsersJob_ThrowsUnauthorizedException()
        {
            var otherUserId = Guid.NewGuid();
            var job = new JobApplication("Junior", otherUserId, _companyId, Priority.Low);

            _jobRepoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            await Assert.ThrowsAsync<UnauthorizedException>(() 
                => _service.UpdateAsync(job.Id, _userId, new UpdateJobApplicationRequest()));
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsNotFoundException()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((JobApplication?)null);

            await Assert.ThrowsAsync<NotFoundException>(() 
                => _service.UpdateAsync(Guid.NewGuid(), _userId, new UpdateJobApplicationRequest()));
        }
    }
}
