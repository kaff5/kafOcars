using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KafOCars.RegisterService.DataAccess.Repositories.Interfaces;
using KafOCars.RegisterService.Domain.Entities;
using KafOCars.RegisterService.DTOs;
using KafOCars.RegisterService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace KafOCars.AuthorizationService.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IUserWriteRepository _userWriteRepository;
    private readonly IRoleReadRepository _roleReadRepository;

    public TokenService(IConfiguration configuration, IUserReadRepository userReadRepository, 
        IUserWriteRepository userWriteRepository,
        IRoleReadRepository roleReadRepository)
    {
        _configuration = configuration;
        _userReadRepository = userReadRepository;
        _userWriteRepository = userWriteRepository;
        _roleReadRepository = roleReadRepository;
    }

    public string GenerateAccessToken(User user, List<Role> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstname", user.FirstName),
            new Claim("secondName", user.SecondName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }
    
    public async Task<RefreshTokenDto> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntity = await _userReadRepository.GetRefreshTokenAsync(refreshToken);
        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            return new RefreshTokenDto
            {
                Success = false,
                Message = "Invalid or expired refresh token."
            };
        }

        var user = await _userReadRepository.GetUserByIdAsync(tokenEntity.UserId);
        if (user == null || !user.IsActive)
        {
            return new RefreshTokenDto
            {
                Success = false,
                Message = "User not found or inactive."
            };
        }

        var roles = await _roleReadRepository.GetUserRolesAsync(user.Id);
        var newAccessToken = GenerateAccessToken(user, roles);

        return new RefreshTokenDto
        {
            Success = true,
            AccessToken = newAccessToken,
            Message = "Token refreshed successfully."
        };
    }
}