using Huntly.Domain.Entities;

namespace Huntly.Infrastructure.Auth
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
