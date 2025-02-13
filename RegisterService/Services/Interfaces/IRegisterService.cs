using KafOCars.RegisterService.DTOs;

namespace KafOCars.RegisterService.Services.Interfaces;

public interface IRegisterService
{
    Task<RegisterDto> RegisterAsync(string firstName, string secondName, string password, string email);
    Task<LoginDto> LoginAsync(string email, string password);
    Task<LogoutDto> LogoutAsync(string refreshToken);
}