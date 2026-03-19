using System.Net;
using System.Net.Http.Json;
using Huntly.Application.DTOs.Auth;

namespace Huntly.Tests.Integration
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidRequest_Returns201()
        {
            var request = new RegisterRequest
            {
                FirstName = "Іван",
                LastName = "Іваненко",
                Email = "ivan@test.com",
                Password = "password123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FirstName, result.FirstName);
        }

        [Fact]
        public async Task Register_DuplicateEmail_Returns400()
        {
            var request = new RegisterRequest
            {
                FirstName = "Іван",
                LastName = "Іваненко",
                Email = "duplicate@test.com",
                Password = "password123"
            };

            await _client.PostAsJsonAsync("/api/Auth/register", request);
            var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_Returns200()
        {
            var registerRequest = new RegisterRequest
            {
                FirstName = "Іван",
                LastName = "Іваненко",
                Email = "login@test.com",
                Password = "password123"
            };
            await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "login@test.com",
                Password = "password123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task Login_WrongPassword_Returns403()
        {
            var registerRequest = new RegisterRequest
            {
                FirstName = "Іван",
                LastName = "Іваненко",
                Email = "wrongpass@test.com",
                Password = "password123"
            };
            await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "wrongpass@test.com",
                Password = "wrongpass"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
