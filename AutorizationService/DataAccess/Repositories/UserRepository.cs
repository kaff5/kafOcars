using KafOCars.DataAccess.Contexts;
using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafOCars.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CoreLib.Db.IDbContextFactory<AuthDbContext> _context;

    public UserRepository(CoreLib.Db.IDbContextFactory<AuthDbContext> context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.CreateReadContext().Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);
    }

    public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (token == null) return false;

        token.IsRevoked = true;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }
}