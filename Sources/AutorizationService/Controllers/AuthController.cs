using System.Threading.Tasks;
using Grpc.Core;
using KafOCars.Authorization;
using KafOCars.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace KafOCars.AuthorizationService.Controllers
{
    public class AuthServiceController : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthServiceController> _logger;
        private readonly IAuthService _authService;

        public AuthServiceController(ILogger<AuthServiceController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Register request received for username: {Username}", request.Username);

            var result = await _authService.RegisterAsync(request.Username, request.Password, request.Email);

            return new RegisterResponse
            {
                Success = result.Success,
                Message = result.Message
            };
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var result = await _authService.LoginAsync(request.Mail, request.Password);

            if (result.Success)
            {
                return new LoginResponse
                {
                    Success = true,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    Message = "Login successful."
                };
            }

            return new LoginResponse
            {
                Success = false,
                Message = result.Message
            };
        }

        public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            return new RefreshTokenResponse
            {
                Success = result.Success,
                AccessToken = result.AccessToken,
                Message = result.Message
            };
        }

        public override async Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
        {
            var result = await _authService.LogoutAsync(request.RefreshToken);

            return new LogoutResponse
            {
                Success = result.Success,
                Message = result.Message
            };
        }
    }
}
