using KafOCars.DataAccess.Contexts;
using KafOCars.RegisterService.DataAccess.Repositories.Interfaces;
using KafOCars.RegisterService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafOCars.RegisterService.DataAccess.Repositories
{
    public class UserWriteRepository : IUserWriteRepository
    {
        private readonly RegisterDbContext _writeContext;
        private bool _disposed;

        public UserWriteRepository(CoreLib.Db.IDbContextFactory<RegisterDbContext> contextFactory)
        {
            _writeContext = contextFactory.CreateWriteContext();
        }

        public async Task AddUserAsync(User user)
        {
            await _writeContext.Users.AddAsync(user);
            await _writeContext.SaveChangesAsync();
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _writeContext.RefreshTokens.AddAsync(refreshToken);
            await _writeContext.SaveChangesAsync();
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _writeContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (token == null)
                return false;

            token.IsRevoked = true;
            _writeContext.RefreshTokens.Update(token);
            await _writeContext.SaveChangesAsync();
            return true;
        }
    }
}