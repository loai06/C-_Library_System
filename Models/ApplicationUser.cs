using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
       public string? ResetCode { get; set; }

       public DateTime? ResetCodeExpiration { get; set; }
    }
}