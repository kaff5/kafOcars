using KafOCars.RegisterService.Domain.Entities;

namespace KafOCars.RegisterService.DataAccess.Repositories.Interfaces;

public interface IUserWriteRepository
{
    Task AddUserAsync(User user);
    Task SaveRefreshTokenAsync(RefreshToken refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}