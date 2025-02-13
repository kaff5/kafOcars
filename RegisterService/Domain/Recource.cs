namespace KafOCars.RegisterService.Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public string Name { get; set; }            // Имя ресурса (например, "ManageUsers")
    public string Description { get; set; }     // Описание ресурса
    public string Url { get; set; }             // URL ресурса (например, "/api/users")
    public string HttpMethod { get; set; }      // HTTP-метод (например, GET, POST)
    public DateTime CreatedAt { get; set; }     // Дата создания

    // Навигационные свойства
    public ICollection<ResourceRole> RoleResources { get; set; } = new List<ResourceRole>();
}
