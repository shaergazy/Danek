using Microsoft.AspNetCore.Identity;

namespace Danek.DAL.Models.Users;

public class Role : IdentityRole
{
    public ICollection<UserRole> Users { get; set; }
}
