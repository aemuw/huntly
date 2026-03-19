using Huntly.Application.DTOs.Company;
using Huntly.Application.Exceptions;
using Huntly.Application.Services;
using Huntly.Domain.Entities;
using Huntly.Domain.Enums;
using Huntly.Domain.Interfaces;
using Moq;

namespace Huntly.Tests.Unit
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyRepository> _companyRepoMock;
        private readonly CompanyService _service;

        public CompanyServiceTests()
        {
            _companyRepoMock = new Mock<ICompanyRepository>();
            _service = new CompanyService(_companyRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsResponse()
        {
            var request = new CreateCompanyRequest
            {
                Name = "GlobalLogic",
                Type = CompanyType.Outsource,
                Size = CompanySize.Large
            };

            _companyRepoMock.Setup(r => r.ExistsAsync(request.Name)).ReturnsAsync(false);

            var result = await _service.CreateAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _companyRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Company?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsResponse()
        {
            var company = new Company("GlobalLogic", CompanyType.Outsource, CompanySize.Large);

            _companyRepoMock.Setup(r => r.GetByIdAsync(company.Id)).ReturnsAsync(company);

            var result = await _service.GetByIdAsync(company.Id);

            Assert.NotNull(result);
            Assert.Equal(company.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsNotFoundException()
        {
            _companyRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Company?)null);

            await Assert.ThrowsAsync<NotFoundException>(()
                => _service.UpdateAsync(Guid.NewGuid(), new UpdateCompanyRequest()));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCompanies()
        {
            var companies = new List<Company>
            {
                new Company("GlobalLogic", CompanyType.Outsource, CompanySize.Large),
                new Company("Epam", CompanyType.Outsource, CompanySize.Large)
            };

            _companyRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(companies);

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }
    }
}
