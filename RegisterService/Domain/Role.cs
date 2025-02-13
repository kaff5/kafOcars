namespace KafOCars.RegisterService.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<ResourceRole> RoleResources { get; set; } = new List<ResourceRole>();
}
