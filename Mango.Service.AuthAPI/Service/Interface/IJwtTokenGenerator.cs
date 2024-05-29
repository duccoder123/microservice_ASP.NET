using Mango.Service.AuthAPI.Models;

namespace Mango.Service.AuthAPI.Service.Interface
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
