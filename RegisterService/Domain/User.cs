using SharedEntities.ServiceRegistry;

namespace KafOCars.RegisterService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }

    public string Email { get; set; }
    public int RegionTypeId { get; set; }
    public string PasswordHash { get; set; }    // Хэш пароля
    public bool IsActive { get; set; }          // Флаг активности пользователя
    public DateTime CreatedAt { get; set; }     // Дата создания
    public DateTime? LastLogin { get; set; }    // Дата последнего входа
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public RRegionType RegionType { get; set; }
}
