namespace KafOCars.Domain.Entities;

public class ResourceRole
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; }
}
