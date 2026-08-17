using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var (success, error) = _authService.Register(dto.Username, dto.Password);

            if (!success)
                return BadRequest(ApiResponse<object>.FailResponse(error!, 400));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "User registered successfully."));
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var (success, token) = _authService.Login(dto.Username, dto.Password);

            if (!success)
                return Unauthorized(ApiResponse<object>.FailResponse("Invalid username or password.", 401));

            return Ok(ApiResponse<object>.SuccessResponse(new { token }, "Login successful."));
        }
    }
}