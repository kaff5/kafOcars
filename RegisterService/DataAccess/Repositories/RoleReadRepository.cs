using KafOCars.DataAccess.Contexts;
using KafOCars.RegisterService.DataAccess.Repositories.Interfaces;
using KafOCars.RegisterService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafOCars.RegisterService.DataAccess.Repositories;

public class RoleReadRepository : IRoleReadRepository
{
    private readonly RegisterDbContext _readContext;

    public RoleReadRepository(CoreLib.Db.IDbContextFactory<RegisterDbContext> contextFactory)
    {
        _readContext = contextFactory.CreateReadContext();
        // Можно установить оптимальные параметры, например, отключить отслеживание
        //_readContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _readContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }
}
