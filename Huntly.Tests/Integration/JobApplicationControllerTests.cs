using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Huntly.Application.DTOs.Auth;
using Huntly.Application.DTOs.Company;
using Huntly.Application.DTOs.JobApplication;
using Huntly.Domain.Enums;
using Huntly.Application.DTOs;

namespace Huntly.Tests.Integration
{
    public class JobApplicationControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public JobApplicationControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetTokenAsync()
        {
            var request = new RegisterRequest
            {
                FirstName = "Іван",
                LastName = "Іваненко",
                Email = $"{Guid.NewGuid()}@test.com",
                Password = "password!123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/register", request);
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return auth!.Token;
        }

        private void SetToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<Guid> CreateCompanyAsync()
        {
            var request = new CreateCompanyRequest
            {
                Name = $"Company {Guid.NewGuid()}",
                Type = CompanyType.Outsource,
                Size = CompanySize.Large
            };
            var response = await _client.PostAsJsonAsync("/api/Companies", request);
            var company = await response.Content.ReadFromJsonAsync<CompanyResponse>();
            return company!.Id;
        }

        [Fact]
        public async Task Create_ValidRequest_Returns201()
        {
            var token = await GetTokenAsync();
            SetToken(token);

            var companyId = await CreateCompanyAsync();

            var request = new CreateJobApplicationRequest
            {
                Title = "Junior C# Developer",
                CompanyId = companyId,
                Priority = Priority.Medium
            };

            var response = await _client.PostAsJsonAsync("/api/JobApplications", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await ReadJobResponseAsync(response);
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(ApplicationStatus.Watchlist, result.Status);
        }

        [Fact]
        public async Task GetAll_Returns200WithList()
        {
            var token = await GetTokenAsync();
            SetToken(token);
            var companyId = await CreateCompanyAsync();

            await _client.PostAsJsonAsync("/api/JobApplications", new CreateJobApplicationRequest
            {
                Title = "Test Position",
                CompanyId = companyId,
                Priority = Priority.Low
            });

            var response = await _client.GetAsync("/api/JobApplications");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
            var content = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<PagedResponse<JobApplicationResponse>>(content, options);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
        }

        [Fact] 
        public async Task ChangeStatus_ValidRequest_Returns204()
        {
            var token = await GetTokenAsync();
            SetToken(token);
            var companyId = await CreateCompanyAsync();

            var createResponse = await _client.PostAsJsonAsync("/api/JobApplications", new CreateJobApplicationRequest
            {
                Title = "Status Test",
                CompanyId = companyId,
                Priority = Priority.High
            });

            var job = await ReadJobResponseAsync(createResponse);

            var statusRequest = new ChangeStatusRequest
            {
                Status = ApplicationStatus.Applied
            };

            var response = await _client.PutAsJsonAsync($"/api/JobApplications/{job!.Id}/status", statusRequest);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task<JobApplicationResponse?> ReadJobResponseAsync (HttpResponseMessage response)
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter()}
            };

            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<JobApplicationResponse>(content, options);
        }
    }
}
