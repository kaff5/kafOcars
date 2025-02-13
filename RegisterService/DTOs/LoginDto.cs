namespace KafOCars.RegisterService.DTOs;

public class LoginDto
{
    public bool Success { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Message { get; set; }
}