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
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, error) = await _authService.RegisterAsync(dto.Username,dto.Email , dto.Password);

            if (!success)
                return BadRequest(ApiResponse<object>.FailResponse(error!, 400));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "User registered successfully."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, token) = await _authService.LoginAsync(dto.Username, dto.Password);

            if (!success)
                return Unauthorized(ApiResponse<object>.FailResponse("Invalid username or password.", 401));

            return Ok(ApiResponse<object>.SuccessResponse(new { token }, "Login successful."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var (success, error) = await _authService.ForgotPasswordAsync(dto.Email);

            if (!success)
                return BadRequest(ApiResponse<object>.FailResponse(error!, 400));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "If the email exists, a reset code has been sent."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var (success, error) = await _authService.ResetPasswordAsync(dto.Username, dto.ResetCode, dto.NewPassword);

            if (!success)
                return BadRequest(ApiResponse<object>.FailResponse(error!, 400));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Password reset successfully."));
        }
    }
}