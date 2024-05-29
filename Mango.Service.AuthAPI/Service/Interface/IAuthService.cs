using Mango.Service.AuthAPI.Models.DTOs;

namespace Mango.Service.AuthAPI.Service.Interface
{
    public interface IAuthService
    {
        Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginRequestDto> Login(LoginRequestDto loginRequestDto);
    }
}
