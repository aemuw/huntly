using Huntly.Application.DTOs.Auth;
using Huntly.Application.Exceptions;
using Huntly.Application.Interfaces;
using Huntly.Application.Services.Interfaces;
using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;

namespace Huntly.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.ExistsAsync(request.Email))
                throw new ValidationException("Email вже зайнятий");

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName, request.LastName, request.Email, passwordHash);

            await _userRepository.AddAsync(user);

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token, 
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                throw new UnauthorizedException("Невірний email або пароль");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Невірний email або пароль");

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
