using KafOCars.RegisterService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KafOCars.RegisterService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController: Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IRegisterService _registerService;
        private readonly ITokenService _tokenService;

        public RegisterController(ILogger<RegisterController> logger, IRegisterService registerService, ITokenService tokenService)
        {
            _logger = logger;
            _registerService = registerService;
            _tokenService = tokenService;
        }

        public async Task<ActionResult> Register(RegisterRequest request)
        {
            var result = await _registerService.RegisterAsync(request.FirstName,request.SecondName, request.Password, request.Email);

            return Ok(new RegisterResponse
            {
                Success = result.Success,
                Message = result.Message
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Неверные данные запроса."
                });

            var result = await _registerService.LoginAsync(request.Mail, request.Password);

            if (!result.Success)
            {
                // Здесь можно вернуть 400 или 401, в зависимости от логики приложения.
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new LoginResponse
            {
                Success = true,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                Message = "Login successful."
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Неверные данные запроса."
                });

            var result = await _tokenService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                return BadRequest(new RefreshTokenResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new RefreshTokenResponse
            {
                Success = true,
                AccessToken = result.AccessToken,
                Message = result.Message
            });
        }
    
        public async Task<LogoutResponse> Logout(LogoutRequest request)
        {
            var result = await _registerService.LogoutAsync(request.RefreshToken);

            return new LogoutResponse
            {
                Success = result.Success,
                Message = result.Message
            };
        }
    }
    
    
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
    
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    
    public class LoginRequest
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }

 
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string Message { get; set; }
    }
    
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }


    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
