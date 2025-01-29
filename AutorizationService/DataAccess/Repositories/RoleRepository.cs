using Grpc.Core;
using KafOCars.DataAccess.Contexts;
using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafOCars.DataAccess.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }
}
