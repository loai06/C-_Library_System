using Microsoft.AspNetCore.Identity;
using LibraryManagementSystem.Models;
using System.Security.Cryptography;

namespace LibraryManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }


        public async Task<(bool success, string? message)> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
                return (false, "Username already exists.");

            var user = new ApplicationUser { UserName = username, Email = email };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, null);
        }

        public async Task<(bool success, string? token)> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return (false, null);

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid) return (false, null);

            var token = _tokenService.GenerateToken(username);
            return (true, token);
        }

        public async Task<(bool success, string? errorMessage)> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return (false, "Email not found.");

            var resetCode = RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();

            user.ResetCode = resetCode;
            user.ResetCodeExpiration = DateTime.UtcNow.AddMinutes(10);

            await _userManager.UpdateAsync(user);

            var emailBody = $"Your password reset code is: {resetCode}";

            await _emailService.SendEmailAsync(
                email,
                "Password Reset Code",
                emailBody
            );

            return (true, null);
        }

        public async Task<(bool success, string? errorMessage)> ResetPasswordAsync(
     string username,
     string resetCode,
     string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return (false, "Username not found.");

            if (user.ResetCode == null || user.ResetCode != resetCode)
                return (false, "Invalid reset code.");

            if (user.ResetCodeExpiration == null ||
                user.ResetCodeExpiration < DateTime.UtcNow)
                return (false, "Reset code has expired.");

            var passwordHasher = new PasswordHasher<ApplicationUser>();

            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            user.ResetCode = null;
            user.ResetCodeExpiration = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    " ",
                    result.Errors.Select(e => e.Description)
                );

                return (false, errors);
            }

            return (true, null);
        }
    }
}