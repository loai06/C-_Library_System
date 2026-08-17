namespace LibraryManagementSystem.Services
{
    public interface IAuthService
    {
       Task <(bool success, string? message)> RegisterAsync(string username, string email, string password);
       Task <(bool success,  string? token)> LoginAsync(string username, string password);
       Task <(bool success,  string? errorMessage)> ForgotPasswordAsync(string email);
       Task<(bool success, string? errorMessage)> ResetPasswordAsync(string username, string resetCode, string newPassword);

    }
}