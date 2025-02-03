using System;
using System.Threading.Tasks;
using KafOCars.DataAccess.Contexts;
using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafOCars.AuthorizationService.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CoreLib.Db.IDbContextFactory<AuthDbContext> _context;
    private readonly AuthDbContext _writeContext;

    public UserRepository(CoreLib.Db.IDbContextFactory<AuthDbContext> context)
    {
        _context = context;
        _writeContext = _context.CreateWriteContext();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.CreateReadContext().Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        await _writeContext.Users.AddAsync(user);
        await _writeContext.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        return await _context.CreateReadContext().RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);
    }

    public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _writeContext.RefreshTokens.AddAsync(refreshToken);
        await _writeContext.SaveChangesAsync();
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.CreateReadContext().RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (token == null) return false;

        token.IsRevoked = true;
        _writeContext.RefreshTokens.Update(token);
        await _writeContext.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _context.CreateReadContext().Users.FindAsync(userId);
    }
}