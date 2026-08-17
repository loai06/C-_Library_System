namespace LibraryManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;

        
        private static readonly Dictionary<string, string> _users = new();

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public (bool success, string? errorMessage) Register(string username, string password)
        {
            if (_users.ContainsKey(username))
                return (false, "Username already exists.");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.");

             
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            _users[username] = hashedPassword;

            return (true, null);
        }

        public (bool success, string? token) Login(string username, string password)
        {
            if (!_users.ContainsKey(username))
                return (false, null);

            var hashedPassword = _users[username];
            var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            if (!isValid) return (false, null);

            var token = _tokenService.GenerateToken(username);
            return (true, token);
        }
    }
}