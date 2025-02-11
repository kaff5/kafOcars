using KafOCars.DataAccess.Contexts;
using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace KafOCars.DataAccess.Repositories
{
    public class UserReadRepository : IUserReadRepository
    {
        private readonly RegisterDbContext _readContext;

        public UserReadRepository(CoreLib.Db.IDbContextFactory<RegisterDbContext> contextFactory)
        {
            _readContext = contextFactory.CreateReadContext();
            // Можно установить оптимальные параметры, например, отключить отслеживание
            //_readContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _readContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return await _readContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _readContext.Users.FindAsync(userId);
        }
    }
}