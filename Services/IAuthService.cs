namespace LibraryManagementSystem.Services
{
    public interface IAuthService
    {
        (bool success, string? errorMessage) Register(string username, string password);
        (bool success, string? token) Login(string username, string password);
    }
}