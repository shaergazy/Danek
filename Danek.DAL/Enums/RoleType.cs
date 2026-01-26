using System.ComponentModel;

namespace Danek.DAL.Enums;

public enum RoleType
{
    [Description("User")]
    User = 1,
    [Description("Admin")]
    Admin = 2,
}