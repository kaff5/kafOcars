using KafOCars.RegisterService.Domain.Entities;
using KafOCars.RegisterService.DTOs;

namespace KafOCars.RegisterService.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, List<Role> roles);
    Task<RefreshTokenDto> RefreshTokenAsync(string refreshToken);
    string GenerateRefreshToken();
}