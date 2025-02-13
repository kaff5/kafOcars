using KafOCars.RegisterService.Domain.Entities;

namespace KafOCars.RegisterService.DataAccess.Repositories.Interfaces;

public interface IUserReadRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task<User?> GetUserByIdAsync(Guid userId);
}