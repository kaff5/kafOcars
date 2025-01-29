using KafOCars.Domain.Entities;

namespace KafOCars.DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    Task SaveRefreshTokenAsync(RefreshToken refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    Task<User> GetUserByIdAsync(Guid userId);
}