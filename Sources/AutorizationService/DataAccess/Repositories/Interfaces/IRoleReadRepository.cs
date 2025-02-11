using KafOCars.Domain.Entities;

namespace KafOCars.DataAccess.Repositories.Interfaces;

public interface IRoleReadRepository
{
    Task<List<Role>> GetUserRolesAsync(Guid userId);
}