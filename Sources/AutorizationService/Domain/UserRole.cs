namespace KafOCars.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }            // Ссылка на пользователя
    public User User { get; set; }              // Навигационное свойство

    public Guid RoleId { get; set; }            // Ссылка на роль
    public Role Role { get; set; }              // Навигационное свойство

    public DateTime AssignedAt { get; set; }    // Дата назначения роли
}
