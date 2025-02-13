using CoreLib.Db;
using KafOCars.RegisterService.DataAccess.Repositories.Interfaces;
using KafOCars.RegisterService.Domain.Entities;
using KafOCars.RegisterService.DTOs;
using KafOCars.RegisterService.Services.Interfaces;

namespace KafOCars.RegisterService.Services;

public class RegisterService: IRegisterService
{
    private readonly ILogger<RegisterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IUserWriteRepository _userWriteRepository;
    private readonly IRoleReadRepository _roleReadRepository;
    private readonly ITokenService _tokenService;

    public RegisterService(
        ILogger<RegisterService> logger,
        IConfiguration configuration,
        IUserReadRepository userReadRepository, 
        IUserWriteRepository userWriteRepository,
        IRoleReadRepository roleReadRepository,
        ITokenService tokenService)
    {
        _logger = logger;
        _configuration = configuration;
        _userReadRepository = userReadRepository;
        _userWriteRepository = userWriteRepository;
        _roleReadRepository = roleReadRepository;
        _tokenService = tokenService;
    }
    
    
    public async Task<RegisterDto> RegisterAsync(string firstName, string secondName, string password, string email)
    {
        var existingUser = await _userReadRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            return new RegisterDto
            {
                Success = false,
                Message = "Email is already in use."
            };
        }

        var hashedPassword = PasswordHasher.HashPassword(password);
        var user = new User
        {
            FirstName = firstName,
            SecondName = secondName,
            Email = email,
            PasswordHash = hashedPassword,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userWriteRepository.AddUserAsync(user);

        return new RegisterDto
        {
            Success = true,
            Message = "User registered successfully."
        };
    }

    public async Task<LoginDto> LoginAsync(string email, string password)
    {
        var user = await _userReadRepository.GetUserByEmailAsync(email);
        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash) || !user.IsActive)
        {
            return new LoginDto
            {
                Success = false,
                Message = "Invalid email, password, or user is inactive."
            };
        }

        var roles = await _roleReadRepository.GetUserRolesAsync(user.Id);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _userWriteRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        });

        return new LoginDto
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Message = "Login successful."
        };
    }
    

    public async Task<LogoutDto> LogoutAsync(string refreshToken)
    {
        var result = await _userWriteRepository.RevokeRefreshTokenAsync(refreshToken);
        if (!result)
        {
            return new LogoutDto
            {
                Success = false,
                Message = "Invalid refresh token."
            };
        }

        return new LogoutDto
        {
            Success = true,
            Message = "Logout successful."
        };
    }

}