using KafOCars.Domain.Entities;

namespace KafOCars.DataAccess.Repositories.Interfaces;

public interface IUserReadRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task<User?> GetUserByIdAsync(Guid userId);
}