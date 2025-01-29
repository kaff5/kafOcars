using KafOCars.DTOs;

namespace KafOCars.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterDto> RegisterAsync(string username, string password, string email);
    Task<LoginDto> LoginAsync(string email, string password);
    Task<RefreshTokenDto> RefreshTokenAsync(string refreshToken);
    Task<LogoutDto> LogoutAsync(string refreshToken);
}