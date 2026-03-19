using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Huntly.Application.DTOs.Auth;
using Huntly.Application.DTOs.Company;
using Huntly.Domain.Enums;

namespace Huntly.Tests.Integration
{
    public class CompaniesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CompaniesControllerTests(CustomWebApplicationFactory factory)
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

        [Fact]
        public async Task GetAll_WithToken_Returns200()
        {
            var token = await GetTokenAsync();
            SetToken(token);

            var response = await _client.GetAsync("/api/Companies");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithoutToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/api/Companies");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Create_ValidRequest_Returns201()
        {
            var token = await GetTokenAsync();
            SetToken(token);

            var request = new CreateCompanyRequest
            {
                Name = "GlobalLogic",
                Type = CompanyType.Outsource,
                Size = CompanySize.Large
            };

            var response = await _client.PostAsJsonAsync("/api/Companies", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<CompanyResponse>();
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
        }

        [Fact]
        public async Task Create_DuplicateName_Returns400()
        {
            var token = await GetTokenAsync();
            SetToken(token);

            var request = new CreateCompanyRequest
            {
                Name = "Duplicate Company",
                Type = CompanyType.Product,
                Size = CompanySize.Small
            };

            await _client.PostAsJsonAsync("/api/Companies", request);
            var response = await _client.PostAsJsonAsync("/api/Companies", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
