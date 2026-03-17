using Huntly.Domain.Entities;

namespace Huntly.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
