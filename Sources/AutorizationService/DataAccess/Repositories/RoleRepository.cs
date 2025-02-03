using Dapper;
using KafOCars.DataAccess.Contexts;
using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace KafOCars.AuthorizationService.DataAccess.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly CoreLib.Db.IDbContextFactory<AuthDbContext> _context;
    private readonly AuthDbContext _writeContext;

    public RoleRepository(CoreLib.Db.IDbContextFactory<AuthDbContext> context)
    {
        _context = context;
        _writeContext = _context.CreateWriteContext();
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.CreateWriteContext().UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }
    
    public async Task<List<Role>> GetUserRolesAsync2(Guid userId)
    {
        using var connection = new NpgsqlConnection(_context.CreateReadContext().Database.GetConnectionString());
        await connection.OpenAsync();

        string sql = @"
            SELECT r.* 
            FROM UserRoles ur
            JOIN Roles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @UserId;";

        var roles = await connection.QueryAsync<Role>(sql, new { UserId = userId });

        return roles.AsList();
    }
}
