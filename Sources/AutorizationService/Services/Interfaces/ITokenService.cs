using KafOCars.Domain.Entities;

namespace KafOCars.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, List<Role> roles);
    string GenerateRefreshToken();
}