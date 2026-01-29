using Microsoft.AspNetCore.Identity;

namespace Danek.DAL.Models.Users;

public class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string TelegramUsername { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public ICollection<UserRole> Roles { get; set; }
}
