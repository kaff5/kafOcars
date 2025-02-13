namespace KafOCars.RegisterService.DTOs;

public class RefreshTokenDto
{
    public bool Success { get; set; }
    public string AccessToken { get; set; }
    public string Message { get; set; }
}