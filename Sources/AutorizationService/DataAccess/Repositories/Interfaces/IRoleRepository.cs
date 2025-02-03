using KafOCars.Domain.Entities;

namespace KafOCars.DataAccess.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetUserRolesAsync(Guid userId);
}