using Huntly.Application.DTOs.Auth;
using Huntly.Application.Exceptions;
using Huntly.Application.Interfaces;
using Huntly.Application.Services;
using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;
using Moq;

namespace Huntly.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtServiceMock = new Mock<IJwtService>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_ReturnsAuthResponse()
        {
            var request = new RegisterRequest
            {
                FirstName = "Іван",
                LastName = "Іванченко",
                Email = "ivan@test.com",
                Password = "test_password!0"
            };

            _userRepositoryMock.Setup(r => r.ExistsAsync(request.Email)).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.Hash(request.Password)).Returns("hashed_password");
            _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("jwt_token");

            var result = await _authService.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.Equal("jwt_token", result.Token);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FirstName, result.FirstName);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ThrowsValidationException()
        {
            var request = new RegisterRequest { Email = "exists@test.com" };

            _userRepositoryMock.Setup(r => r.ExistsAsync(request.Email)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ValidationException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
        {
            var request = new LoginRequest
            {
                Email = "iv@test.com",
                Password = "password!"
            };

            var user = new User("Iv", "Bob", request.Email, "hashed_password");

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(true);
            _jwtServiceMock.Setup(j => j.GenerateToken(user)).Returns("jwt_token");

            var result = await _authService.LoginAsync(request);

            Assert.NotNull(request);
            Assert.Equal("jwt_token", result.Token);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedException()
        {
            var request = new LoginRequest { Email = "notfound@test.com" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedException()
        {
            var request = new LoginRequest
            {
                Email = "ivan@test.com",
                Password = "WrongPassword"
            };

            var user = new User("Ivan", "Ivanov", request.Email, "hashed_password");

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash));

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
        }
    }
}
