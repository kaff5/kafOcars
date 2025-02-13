using KafOCars.RegisterService.Domain.Entities;

namespace KafOCars.RegisterService.DataAccess.Repositories.Interfaces;

public interface IRoleReadRepository
{
    Task<List<Role>> GetUserRolesAsync(Guid userId);
}