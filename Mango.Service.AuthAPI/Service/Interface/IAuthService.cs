using Mango.Service.AuthAPI.Models.DTOs;

namespace Mango.Service.AuthAPI.Service.Interface
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
