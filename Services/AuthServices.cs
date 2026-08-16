namespace LibraryManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;

        public AuthService(IConfiguration config, ITokenService tokenService)
        {
            _config = config;
            _tokenService = tokenService;
        }

        public (bool success, string? token) Login(string username, string password)
        {
            var validUsername = _config["AdminCredentials:Username"];
            var validPassword = _config["AdminCredentials:Password"];

            if (username != validUsername || password != validPassword)
                return (false, null);

            var token = _tokenService.GenerateToken(username);
            return (true, token);
        }
    }
}