using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Domain.Entities;
using KafOCars.DTOs;
using KafOCars.Services.Interfaces;
using KafOCars.Utils;

namespace KafOCars.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;

    public AuthService(
        ILogger<AuthService> logger,
        IConfiguration configuration,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService)
    {
        _logger = logger;
        _configuration = configuration;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
    }

    public async Task<RegisterDto> RegisterAsync(string username, string password, string email)
    {
        _logger.LogInformation("Attempting to register user: {Username}", username);

        var existingUser = await _userRepository.GetUserByEmailAsync(email);
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
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddUserAsync(user);

        return new RegisterDto
        {
            Success = true,
            Message = "User registered successfully."
        };
    }

    public async Task<LoginDto> LoginAsync(string email, string password)
    {
        _logger.LogInformation("Attempting to login user with email: {Email}", email);

        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash) || !user.IsActive)
        {
            return new LoginDto
            {
                Success = false,
                Message = "Invalid email, password, or user is inactive."
            };
        }

        var roles = await _roleRepository.GetUserRolesAsync(user.Id);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _userRepository.SaveRefreshTokenAsync(new RefreshToken
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

    public async Task<RefreshTokenDto> RefreshTokenAsync(string refreshToken)
    {
        _logger.LogInformation("Attempting to refresh token.");

        var tokenEntity = await _userRepository.GetRefreshTokenAsync(refreshToken);
        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            return new RefreshTokenDto
            {
                Success = false,
                Message = "Invalid or expired refresh token."
            };
        }

        var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);
        if (user == null || !user.IsActive)
        {
            return new RefreshTokenDto
            {
                Success = false,
                Message = "User not found or inactive."
            };
        }

        var roles = await _roleRepository.GetUserRolesAsync(user.Id);
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);

        return new RefreshTokenDto
        {
            Success = true,
            AccessToken = newAccessToken,
            Message = "Token refreshed successfully."
        };
    }

    public async Task<LogoutDto> LogoutAsync(string refreshToken)
    {
        _logger.LogInformation("Attempting to logout user.");

        var result = await _userRepository.RevokeRefreshTokenAsync(refreshToken);
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