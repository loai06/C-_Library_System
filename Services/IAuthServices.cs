namespace LibraryManagementSystem.Services
{
    public interface IAuthService
    {
        (bool success, string? token) Login(string username, string password);
    }
}