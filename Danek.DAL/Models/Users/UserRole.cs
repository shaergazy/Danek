using Microsoft.AspNetCore.Identity;

namespace Danek.DAL.Models.Users;

public class UserRole : IdentityUserRole<string>
{
    public User User { get; set; }

    public Role Role { get; set; }
}
